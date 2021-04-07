using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MechanicChecker.Models;

namespace MechanicChecker
{
    public class EcommerceHelper
    {
        public static async Task<List<SellerProduct>> GetProductsFromEcommerceSite(string siteType, string query)
        {
            switch(siteType)
            {
                case "0":
                    return await GetAllEcommerceProducts(query);
                case "2":
                    return await GetEbayProducts(query);
                default:
                    return new List<SellerProduct>();
            }

        }

        public static async Task<List<SellerProduct>> GetAllEcommerceProducts(string query)
        {
            List<SellerProduct> list = new List<SellerProduct>();

            await Task.WhenAll(
                    // call all ecommerce apis
                    GetEbayProducts(query)
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

        public static async Task<List<SellerProduct>> GetEbayProducts(string query)
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

            string ebayFindingAPIEndpoint = "https://svcs.ebay.com/services/search/FindingService/v1?";

            /*
             * Query parameters for Ebay Finding API
             */

            // App ID / SECURITY-APPNAME
            string ebayAPIKey = Startup.Configuration.GetSection("APIKeys")["Ebay"];

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

            List<SellerProduct> list = new List<SellerProduct>(); 
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
                catch(NullReferenceException e)
                { }

                if (jsonEbayItems is object) // check if any items were returned
                {
                    list = new List<SellerProduct>();

                    Seller ebaySeller = new Seller()
                    {
                        SellerId = 1,
                        UserName = "MajorRetailer_Ebay",
                        FirstName = "Pierre",
                        LastName = "Omidyar",
                        Email = "mechaniccheckerebay@gmail.com",
                        PasswordHash = null,
                        AccountType = "Store",
                        IsApproved = true,
                        CompanyName = "Ebay",
                        BusinessPhone = "8869619253",
                        CompanyLogoUrl = "https://s3.amazonaws.com/mechanic.checker/seller/ebay-logo-1-1200x630-margin.png",
                        WebsiteUrl = "https://www.ebay.ca/",
                        Application = "This Major Retailer is approved automatically.",
                        ApplicationDate = Convert.ToDateTime("2021-04-02 21:08:47"),
                        ApprovalDate = Convert.ToDateTime("2021-04-02 21:08:47")
                    };

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
            return list;
        }

    }
}
