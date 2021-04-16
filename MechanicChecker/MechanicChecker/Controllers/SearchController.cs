using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MechanicChecker.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            return View(context.GetAllProducts());
        }

        public async Task<ViewResult> SearchLocalSellersProducts(string homeAddress, string vendorFilter, string query, string filterType, double minPrice, double maxPrice, string seller)
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            SellerContext contextSeller = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
            ExternalAPIsContext contextAPIs = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.ExternalAPIsContext)) as ExternalAPIsContext;

            List<SellerProduct> listOfQueriedProducts = new List<SellerProduct>(); ;
            List<SellerProduct> queryFilteredResults = new List<SellerProduct>();
            List<SellerProduct> resultsWithAddress = new List<SellerProduct>();
            List<SellerProduct> ecommerceProducts = new List<SellerProduct>();

            var allSellersProducts = context.GetAllSellerProducts();

            if (!string.IsNullOrEmpty(homeAddress) || !string.IsNullOrEmpty(query))
            {
                // get local seller products
                if (query != null && vendorFilter != null && (Convert.ToInt32(vendorFilter) == 0 || Convert.ToInt32(vendorFilter) == 1))
                {

                    queryFilteredResults = allSellersProducts.Where(
                                       product =>
                                       product.localProduct.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                                       ).ToList();
                }

                // get major retailer ecommerce store products
                if (query != null && vendorFilter != null && Convert.ToInt32(vendorFilter) >= 0)
                {
                    ecommerceProducts = await EcommerceHelper.GetProductsFromEcommerceSite(contextSeller, contextAPIs, vendorFilter, query);
                }

                //check if home address is there
                if (homeAddress != null && Regex.Match(homeAddress, @"^([ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ])\ ?([0-9][ABCEGHJKLMNPRSTVWXYZ][0-9])$", RegexOptions.IgnoreCase).Success)
                {
                    //if query is present, then take query filtered result, else take all products
                    List<SellerProduct> items = new List<SellerProduct>();
                    items = query != null ? queryFilteredResults : allSellersProducts.ToList();
                    //sorting first by distance then by address
                    items = items.OrderBy(x => x.distance).ThenBy(i => i.sellerAddress.Address).ToList();
                    //loop all the products
                    Parallel.ForEach<SellerProduct>(items, (item) =>
                    {
                        //for each product, call google api to calculate distance from the user entered location
                        var distance = GoogleMapsHelper.CalculateDistance(contextAPIs, homeAddress, item.sellerAddress.Address);
                        //set returned value to the property
                        item.distance = distance;

                        resultsWithAddress.Add(item);
                    });

                    listOfQueriedProducts = resultsWithAddress;
                }
                else
                {
                    listOfQueriedProducts = queryFilteredResults;
                }

                // combine the local sellers and major retailer ecommerce store products results
                listOfQueriedProducts.AddRange(ecommerceProducts);
            }

            IEnumerable<SellerProduct> listOfProducts = listOfQueriedProducts.AsQueryable();

            switch (filterType)
            {
                case "atoz":
                    listOfProducts = listOfProducts.OrderBy(p => p.localProduct.Title);
                    break;
                case "ztoa":
                    listOfProducts = listOfProducts.OrderByDescending(p => p.localProduct.Title);
                    break;
                case "lowtohigh":
                    listOfProducts = listOfProducts.Where(p => !p.localProduct.Price.Equals("")).OrderByDescending(p =>
                    Convert.ToDouble(p.localProduct.Price));
                    break;
                case "hightolow":
                    listOfProducts = listOfProducts.Where(p => !p.localProduct.Price.Equals("")).OrderBy(p =>
                    Convert.ToDouble(p.localProduct.Price));
                    break;
                case "parts":
                    listOfProducts = listOfProducts.Where(p => p.localProduct.Category.Contains("Item"));
                    break;
                case "services":
                    listOfProducts = listOfProducts.Where(p => p.localProduct.Category.Contains("Service"));
                    break;
                case "price":
                    listOfProducts = listOfProducts.Where(p => !p.localProduct.Price.Equals("")
                    && Convert.ToDouble(p.localProduct.Price) > minPrice &&
                    Convert.ToDouble(p.localProduct.Price) < maxPrice);
                    break;
                case "seller":
                    listOfProducts = listOfProducts.Where(p => p.seller.CompanyName.Equals(seller));
                    break;
                case "quote":
                    listOfProducts = listOfProducts.Where(p => p.localProduct.IsQuote.Equals(true));
                    break;
                default:
                    listOfProducts = listOfProducts;
                    break;
            }

            ViewBag.PostalCode = homeAddress;
            ViewBag.SearchQuery = query;
            ViewBag.VendorFilter = vendorFilter;
            ViewBag.FilterType = filterType;

            return View("SearchResultsList", listOfProducts.ToList());
        }

        [HttpPost]
        public ActionResult Details(string spr)
        {
            //deserilizing the product from JSON to SellerProduct
            SellerProduct product = JsonConvert.DeserializeObject<SellerProduct>(spr);
            return View("SearchViewDetails", product);
        }

        //[HttpPost]
        //public IActionResult CompareTwoParts(string product1)
        //{
        //    var pr = product1;

        //    return View("SearchCompareTwoParts", product1);
        //}

        [HttpPost]
        public IActionResult CompareTwoParts(string product1, string product2)
        {

            SellerProduct prod1 = JsonConvert.DeserializeObject<SellerProduct>(product1);
            SellerProduct prod2 = JsonConvert.DeserializeObject<SellerProduct>(product2);

            List<SellerProduct> products = new List<SellerProduct>();
            products.Add(prod1);
            products.Add(prod2);

            return View("SearchCompareTwoParts", products);
        }        

    }

}

