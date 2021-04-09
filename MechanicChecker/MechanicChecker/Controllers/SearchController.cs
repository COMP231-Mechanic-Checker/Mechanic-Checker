using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Mvc;

namespace MechanicChecker.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            return View(context.GetAllProducts());
        }

        public ViewResult Filter(string filterType, double minPrice, double maxPrice, string searchquery, string sellerid)
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

            IEnumerable<SellerProduct> listOfQueriedProducts;
            IEnumerable<SellerProduct> listOfOriginalQueriedProducts;

            var allSellersProducts = context.GetAllSellerProducts();

            if (searchquery != null)
            {
                listOfOriginalQueriedProducts = allSellersProducts.Where(
                   product =>
                   product.localProduct.Title.Contains(searchquery, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(searchquery, StringComparison.OrdinalIgnoreCase)
                   );
            }
            else
            {
                listOfOriginalQueriedProducts = allSellersProducts;
            }

            switch (filterType)
            {
                case "atoz":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.OrderBy(p => p.localProduct.Title);
                    break;
                case "ztoa":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.OrderByDescending(p => p.localProduct.Title);
                    break;
                case "lowtohigh":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => !p.localProduct.Price.Equals("")).OrderByDescending(p =>
                    Convert.ToDouble(p.localProduct.Price));
                    break;
                case "hightolow":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => !p.localProduct.Price.Equals("")).OrderBy(p =>
                    Convert.ToDouble(p.localProduct.Price));
                    break;
                case "parts":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => p.localProduct.Category.Contains("Item"));
                    break;
                case "services":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => p.localProduct.Category.Contains("Service"));
                    break;
                case "price":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => !p.localProduct.Price.Equals("")
                    && Convert.ToDouble(p.localProduct.Price) > minPrice &&
                    Convert.ToDouble(p.localProduct.Price) < maxPrice);
                    break;
                case "seller":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => p.localProduct.sellerId.Equals(sellerid));
                    break;
                case "quote":
                    listOfQueriedProducts = listOfOriginalQueriedProducts.Where(p => p.localProduct.IsQuote.Equals(false));
                    break;
                default:
                    listOfQueriedProducts = listOfOriginalQueriedProducts;
                    break;
            }

            ViewBag.SearchQuery = searchquery;
            return View("SearchResultsList", listOfQueriedProducts);
        }

        public async Task<ViewResult> SearchLocalSellersProducts(string homeAddress, string vendorFilter, string query) // the value gotten from the url
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

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
                    ecommerceProducts = await EcommerceHelper.GetProductsFromEcommerceSite(vendorFilter, query);
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
                        var distance = GoogleMapsHelper.CalculateDistance(homeAddress, item.sellerAddress.Address);
                        //set returned value to the property
                        item.distance = distance.Result;

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

            ViewBag.PostalCode = homeAddress;
            ViewBag.SearchQuery = query;
            return View("SearchResultsList", listOfQueriedProducts);
        }

        public IActionResult SearchViewDetails()
        {
            return View("SearchViewDetails");
        }

    }

}
