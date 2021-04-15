using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MechanicChecker
{
    public class EcommerceHelper
    {
        public static async Task<List<SellerProduct>> GetProductsFromEcommerceSite(SellerContext contextSeller, ExternalAPIsContext contextAPIs, string siteType, string query)
        {
            switch (siteType)
            {
                case "0":
                    return await GetAllEcommerceProducts(contextSeller, contextAPIs, query);
                case "2":
                    return await GetEbayProducts(contextSeller, contextAPIs, query);
                case "3":
                    return await GetAmazonProducts(contextSeller, contextAPIs, query);
                case "4":
                    return await GetAlibabaProducts(contextSeller, contextAPIs, query);
                default:
                    return new List<SellerProduct>();
            }

        }

        public static async Task<List<SellerProduct>> GetAllEcommerceProducts(SellerContext contextSeller, ExternalAPIsContext contextAPIs, string query)
        {
            List<SellerProduct> list = new List<SellerProduct>();

            await Task.WhenAll(
                    // call all ecommerce apis
                    GetEbayProducts(contextSeller, contextAPIs, query),
                    GetAmazonProducts(contextSeller, contextAPIs, query),
                    GetAlibabaProducts(contextSeller, contextAPIs, query)
                    )
                    // add to output list whenever an api call is done
                    .ContinueWith(apiRes =>
                    {
                        foreach (var apiItems in apiRes.Result)
                        {
                            list.AddRange(apiItems);
                        }
                    });

            return list;
        }

        public static async Task<List<SellerProduct>> GetAlibabaProducts(SellerContext contextSeller, ExternalAPIsContext contextAPIs, string query)
        {
            /* 
             * Send search request for products using the RapidAPI Axesso Alibaba API. 
             * 
             * RapidAPI Axesso Alibaba API Documentation: https://rapidapi.com/axesso/api/axesso-alibaba-data-service
             * RapidAPI Axesso API General Documentation (Alibaba): http://api-doc.axesso.de/#api-Alibaba
             * Access to use any RapidAPI API requires registration to RapidAPI: https://rapidapi.com/
             * 
             * The specific API function used is searchByKeyword: 
             * https://rapidapi.com/axesso/api/axesso-alibaba-data-service?endpoint=apiendpoint_ee6cc691-5c7b-4c24-8dfc-7ce1d446cb6b
             * 
             * There are 2 API Keys required:
             * x-rapidapi-key
             * x-rapidapi-host
             */

            Seller alibabaSeller = contextSeller.GetSellerByCompanyName("Alibaba");
            string apiKeyOwner = Startup.Configuration.GetSection("APIKeyOwners")["Alibaba"];
            ExternalAPIs alibabaAPI = contextAPIs.GetApiByService("RapidAPI Alibaba", apiKeyOwner);
            List<SellerProduct> list = new List<SellerProduct>(); // assume no items will be returned

            if (alibabaAPI.IsEnabled && alibabaAPI.Quota > 0)
            {
                string alibabaRapidAPIEndpoint = "https://axesso-alibaba-data-service.p.rapidapi.com/alb/alibaba-search-by-keyword?";

                string rapidAPIKey = alibabaAPI.APIKey;
                string rapidAPIHost = alibabaAPI.APIHost;

                string automotiveKeyword = "automotive";

                Dictionary<string, string> alibabaParameters = new Dictionary<string, string>()
                {
                    //{"x-rapidapi-key", rapidAPIKey},
                    //{"x-rapidapi-host", rapidAPIHost},
                    //{"domainCode", "ca"},
                    {"keyword", automotiveKeyword + " " + query},
                    {"sortBy", "best_match"},
                    {"page", "1"},
                    {"type", "text"}
                };
                FormUrlEncodedContent encodedAlibabaParameters = new FormUrlEncodedContent(alibabaParameters);

                string url = alibabaRapidAPIEndpoint + encodedAlibabaParameters.ReadAsStringAsync().Result;

                // get amazon products
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", rapidAPIKey);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", rapidAPIHost);
                client.DefaultRequestHeaders.Add("useQueryString", "true");
                HttpResponseMessage response = await client.GetAsync(url);
                contextAPIs.activateAPI("RapidAPI Alibaba", apiKeyOwner); // reduce quota by 1

                /* Parse JSON request to SellerProducts */
                if (response.IsSuccessStatusCode)
                {
                    /*
                     * Newtonsoft.Json.Linq
                     * JToken Hierarchy
                     * Refer to: 
                     * https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm
                     * 
                     * JToken is abstract. Allows ability to handle any type of JSON input.
                     */
                    string jsonAlibabaRes = await response.Content.ReadAsStringAsync();
                    JToken jsonAlibabaResObj = JsonConvert.DeserializeObject<JToken>(jsonAlibabaRes);
                    JToken jsonAlibabaItems = null;

                    try
                    {
                        jsonAlibabaItems = jsonAlibabaResObj["foundProducts"];
                    }
                    catch (NullReferenceException e)
                    { }

                    if (jsonAlibabaItems is object) // check if any items were returned
                    {
                        int alibabaItemId = 1;
                        /*
                         * JObject is use here instead of JToken since we know we're iterating through item objects.
                         * JToken would also work, but JObject is more specific.
                         */
                        foreach (JObject item in jsonAlibabaItems)
                        {
                            try
                            {   // Alibaba API doesn't give us product image url or price (even if the product has a price)
                                LocalProduct localProduct = new LocalProduct()
                                {
                                    LocalProductId = alibabaItemId,
                                    Category = "Item",
                                    Title = item.SelectToken("productName").ToString(),
                                    Price = "",
                                    Description = item.SelectToken("supplierName").ToString(),
                                    ImageUrl = alibabaSeller.CompanyLogoUrl,
                                    sellerId = alibabaSeller.SellerId.ToString(),
                                    ProductUrl = "https:" + item.SelectToken("url").ToString(),
                                    IsVisible = true,
                                    IsQuote = false
                                };

                                Seller seller = alibabaSeller;
                                SellerAddress sellerAddress = new SellerAddress();

                                list.Add(new SellerProduct(localProduct, seller, sellerAddress));

                                alibabaItemId += 1;
                            }
                            catch (NullReferenceException e)
                            { continue; }
                        }
                    }
                }
            }

            return list;
        }

        public static async Task<List<SellerProduct>> GetAmazonProducts(SellerContext contextSeller, ExternalAPIsContext contextAPIs, string query)
        {
            /* 
             * Send search request for products using the RapidAPI Axesso Amazon API. 
             * 
             * RapidAPI Axesso Amazon API Documentation: https://rapidapi.com/axesso/api/axesso-amazon-data-service1
             * RapidAPI Axesso API General Documentation (Amazon): http://api-doc.axesso.de/#api-Amazon
             * Access to use any RapidAPI API requires registration to RapidAPI: https://rapidapi.com/
             * 
             * The specific API function used is searchByKeywordAsin: 
             * https://rapidapi.com/axesso/api/axesso-amazon-data-service1?endpoint=apiendpoint_9cca468a-ea41-4ab8-9412-605b486a6111
             * 
             * There are 2 API Keys required:
             * x-rapidapi-key
             * x-rapidapi-host
             */

            Seller amazonSeller = contextSeller.GetSellerByCompanyName("Amazon");
            string apiKeyOwner = Startup.Configuration.GetSection("APIKeyOwners")["Amazon"];
            ExternalAPIs amazonAPI = contextAPIs.GetApiByService("RapidAPI Amazon", apiKeyOwner);
            List<SellerProduct> list = new List<SellerProduct>(); // assume no items will be returned

            if (amazonAPI.IsEnabled && amazonAPI.Quota > 0)
            {
                string amazonRapidAPIEndpoint = "https://axesso-axesso-amazon-data-service-v1.p.rapidapi.com/amz/amazon-search-by-keyword-asin?";

                string rapidAPIKey = amazonAPI.APIKey;
                string rapidAPIHost = amazonAPI.APIHost;

                string automotiveKeyword = "automotive";

                Dictionary<string, string> amazonParameters = new Dictionary<string, string>()
                {
                    //{"x-rapidapi-key", rapidAPIKey},
                    //{"x-rapidapi-host", rapidAPIHost},
                    {"domainCode", "ca"},
                    {"keyword", automotiveKeyword + " " + query},
                    {"sortBy", "relevanceblender"},
                    {"page", "1"},
                };
                FormUrlEncodedContent encodedAmazonParameters = new FormUrlEncodedContent(amazonParameters);

                string url = amazonRapidAPIEndpoint + encodedAmazonParameters.ReadAsStringAsync().Result;

                // get amazon products
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", rapidAPIKey);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", rapidAPIHost);
                client.DefaultRequestHeaders.Add("useQueryString", "true");
                HttpResponseMessage response = await client.GetAsync(url);
                contextAPIs.activateAPI("RapidAPI Amazon", apiKeyOwner); // reduce quota by 1

                /* Parse JSON request to SellerProducts */
                if (response.IsSuccessStatusCode)
                {
                    /*
                     * Newtonsoft.Json.Linq
                     * JToken Hierarchy
                     * Refer to: 
                     * https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm
                     * 
                     * JToken is abstract. Allows ability to handle any type of JSON input.
                     */
                    string jsonAmazonRes = await response.Content.ReadAsStringAsync();
                    JToken jsonAmazonResObj = JsonConvert.DeserializeObject<JToken>(jsonAmazonRes);
                    JToken jsonAmazonItems = null;
                    try
                    {
                        jsonAmazonItems = jsonAmazonResObj["searchProductDetails"];

                        // void the returned items if there is nothing automobile-related in them
                        string topCategory = jsonAmazonResObj.SelectToken("categories[0]").ToString();
                        if (!topCategory.Contains(automotiveKeyword, StringComparison.OrdinalIgnoreCase))
                        {
                            jsonAmazonItems = null;
                        }
                    }
                    catch (NullReferenceException e)
                    { }

                    if (jsonAmazonItems is object) // check if any items were returned
                    {
                        int amazonItemId = 1;
                        /*
                         * JObject is use here instead of JToken since we know we're iterating through item objects.
                         * JToken would also work, but JObject is more specific.
                         */
                        foreach (JObject item in jsonAmazonItems)
                        {
                            try
                            {
                                LocalProduct localProduct = new LocalProduct()
                                {
                                    LocalProductId = amazonItemId,
                                    Category = "Item",
                                    Title = item.SelectToken("productDescription").ToString(),
                                    Price = item.SelectToken("price").ToString(),
                                    Description = item.SelectToken("productRating").ToString(),
                                    ImageUrl = item.SelectToken("imgUrl").ToString(),
                                    sellerId = amazonSeller.SellerId.ToString(),
                                    ProductUrl = "https://amazon.ca/" + item.SelectToken("dpUrl").ToString(),
                                    IsVisible = true,
                                    IsQuote = false
                                };

                                if (localProduct.Description is null)
                                {
                                    localProduct.Description = "";
                                }

                                Seller seller = amazonSeller;
                                SellerAddress sellerAddress = new SellerAddress();

                                list.Add(new SellerProduct(localProduct, seller, sellerAddress));

                                amazonItemId += 1;
                            }
                            catch (NullReferenceException e)
                            { continue; }
                        }
                    }
                }
            }

            return list;
        }

        public static async Task<List<SellerProduct>> GetEbayProducts(SellerContext contextSeller, ExternalAPIsContext contextAPIs, string query)
        {
            /* 
             * Send search request for products using the Ebay Finding API.
             * 5000 API calls can be made per day.
             * Refer to: https://developer.ebay.com/support/api-call-limits
             * 
             * Ebay Finding API Documentation: https://developer.ebay.com/devzone/finding/CallRef/index.html
             * The specific API function used is findItemsAdvanced: https://developer.ebay.com/devzone/finding/CallRef/findItemsAdvanced.html
             * Access to use any of Ebay's API requires registration to the Ebay's developers program: https://developer.ebay.com/
             * 
             * There are 2 types of Application Keys environments: Sandbox and Production.
             * Production application keys are required to get Ebay products from their live site.
             * The App ID, e.g. SECURITY-APPNAME, is the API key needed for the Ebay Finding API.
             */

            Seller ebaySeller = contextSeller.GetSellerByCompanyName("Ebay");
            string apiKeyOwner = Startup.Configuration.GetSection("APIKeyOwners")["Ebay"];
            ExternalAPIs ebayAPI = contextAPIs.GetApiByService("DeveloperAPI Ebay", apiKeyOwner);
            List<SellerProduct> list = new List<SellerProduct>();

            if (ebayAPI.IsEnabled && ebayAPI.Quota > 0)
            {
                string ebayFindingAPIEndpoint = "https://svcs.ebay.com/services/search/FindingService/v1?";

                /*
                 * Query parameters for Ebay Finding API
                 */

                // App ID / SECURITY-APPNAME
                string ebayAPIKey = ebayAPI.APIKey;

                /*
                 * EBAY-ENCA refers to Ebay Canada
                 * Refer to: https://developer.ebay.com/devzone/finding/CallRef/Enums/GlobalIdList.html
                 */
                string globalId = "EBAY-ENCA";

                /*
                 * The specific API function used is findItemsAdvanced
                 * Refer to: https://developer.ebay.com/devzone/finding/CallRef/findItemsAdvanced.html
                 */
                string operationName = "findItemsAdvanced";

                // version of the Ebay Finding API to use
                string serviceVersion = "1.13.0";

                /*
                 * How the items returned in the JSON are sorted in the items array
                 * Refer to: https://developer.ebay.com/devzone/finding/CallRef/extra/fnditmsadvncd.rqst.srtordr.html
                 */
                string sortOrder = "BestMatch";

                /*
                 * Determines the subset of items returned.
                 * Refer to: https://developer.ebay.com/devzone/finding/CallRef/types/PaginationInput.html
                 */
                string paginationInputPageNumber = (1).ToString();
                string paginationInputEntriesPerPage = (100).ToString();

                /*
                 * 6000 refers to the Automotive category when the context is Ebay Canada
                 * Refer to: https://ir.ebaystatic.com/pictures/aw/pics/catchanges/2021/CA_Category_Changes_May2021.pdf
                 * Pg. 21
                 */
                string categoryId = (6000).ToString();

                /*
                 * Option to use the searcy query on item descriptions
                 * Refer to: https://developer.ebay.com/devzone/finding/CallRef/findItemsAdvanced.html#sampledescription
                 */
                string descriptionSearch = (true).ToString();

                Dictionary<string, string> ebayParameters = new Dictionary<string, string>()
                {
                    {"SECURITY-APPNAME", ebayAPIKey},
                    {"GLOBAL-ID", globalId},
                    {"OPERATION-NAME", operationName},
                    {"SERVICE-VERSION", serviceVersion},
                    {"RESPONSE-DATA-FORMAT", "JSON"},
                    {"keywords", query},
                    {"sortOrder", sortOrder},
                    {"paginationInput.pageNumber", paginationInputPageNumber},
                    {"paginationInput.entriesPerPage", paginationInputEntriesPerPage},
                    {"categoryId", categoryId},
                    {"descriptionSearch", descriptionSearch},

                    /*
                     * Parameters that can be used to further filter items.
                     * The additional parameters fall under the itemFilter option.
                     * Refer to: https://developer.ebay.com/devzone/finding/CallRef/types/ItemFilterType.html
                     * 
                     * listedIn:
                     * Restrict listings based on location specified
                     * 
                     * currency:
                     * Restrict listings based on currency listed
                     * Refer to: https://developer.ebay.com/devzone/finding/CallRef/Enums/currencyIdList.html
                     * 
                     * listingType:
                     * Restrict listings based on type of listing, e.g. fixed price, auction
                     */
                    {"itemFilter(0).name", "listedIn"},
                    {"itemFilter(0).value", globalId},
                    {"itemFilter(1).name", "currency"},
                    {"itemFilter(1).value", "CAD"},
                    {"itemFilter(2).name", "listingType"},
                    {"itemFilter(2).value", "fixedPrice"},

                    /*
                     * Parameters that can be used to get additional information from the API call.
                     * The additional parameters fall under the outputSelector option.
                     * Refer to: https://developer.ebay.com/devzone/finding/CallRef/types/OutputSelectorType.html
                     * 
                     * sellerInfo:
                     * Additional info about the seller for each item.
                     * 
                     * pictureURLLarge:
                     * Provides URL images with size 400x400
                     */
                    {"outputSelector(0)", "sellerInfo"},
                    {"outputSelector(1)", "pictureURLLarge"}
                };
                FormUrlEncodedContent encodedEbayParameters = new FormUrlEncodedContent(ebayParameters);

                string url = ebayFindingAPIEndpoint + encodedEbayParameters.ReadAsStringAsync().Result;

                // get ebay products
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                contextAPIs.activateAPI("DeveloperAPI Ebay", apiKeyOwner); // reduce quota by 1

                /* Parse JSON request to SellerProducts */
                if (response.IsSuccessStatusCode)
                {
                    /*
                     * Newtonsoft.Json.Linq
                     * JToken Hierarchy
                     * Refer to: 
                     * https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm
                     * 
                     * JToken is abstract. Allows ability to handle any type of JSON input.
                     */
                    string jsonEbayRes = await response.Content.ReadAsStringAsync();
                    JToken jsonEbayResObj = JsonConvert.DeserializeObject<JToken>(jsonEbayRes);
                    JToken jsonEbayItems = null;
                    try
                    {
                        jsonEbayItems = jsonEbayResObj["findItemsAdvancedResponse"][0]["searchResult"][0]["item"];
                    }
                    catch (NullReferenceException e)
                    { }

                    if (jsonEbayItems is object) // check if any items were returned
                    {
                        int ebayItemId = 1;
                        /*
                         * JObject is use here instead of JToken since we know we're iterating through item objects.
                         * JToken would also work, but JObject is more specific.
                         */
                        foreach (JObject item in jsonEbayItems)
                        {

                            try
                            {
                                LocalProduct localProduct = new LocalProduct()
                                {
                                    LocalProductId = ebayItemId,
                                    Category = "Item",
                                    Title = item.SelectToken("title[0]").ToString(),
                                    Price = item.SelectToken("sellingStatus[0].convertedCurrentPrice[0].__value__").ToString(),
                                    Description = item.SelectToken("primaryCategory[0].categoryName[0]").ToString(),
                                    ImageUrl = item.SelectToken("galleryURL[0]").ToString(),
                                    sellerId = ebaySeller.SellerId.ToString(),
                                    ProductUrl = item.SelectToken("viewItemURL[0]").ToString(),
                                    IsVisible = true,
                                    IsQuote = false
                                };
                                Seller seller = ebaySeller;
                                SellerAddress sellerAddress = new SellerAddress();

                                list.Add(new SellerProduct(localProduct, seller, sellerAddress));

                                ebayItemId += 1;
                            }
                            catch (NullReferenceException e)
                            { continue; }
                        }
                    }
                }
            }

            return list;
        }

    }
}
