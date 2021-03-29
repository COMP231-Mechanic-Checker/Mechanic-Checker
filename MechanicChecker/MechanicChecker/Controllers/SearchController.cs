using System;
using System.Collections.Generic;
using System.Linq;
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

        public ViewResult SearchLocalSellersProducts(string homeAddress, string query) // the value gotten from the url
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

            IEnumerable<SellerProduct> listOfQueriedProducts;
            List<SellerProduct> queryFilteredResults = new List<SellerProduct>();
            List<SellerProduct> resultsWithAddress = new List<SellerProduct>();

            var allSellersProducts = context.GetAllSellerProducts();

            if (!string.IsNullOrEmpty(homeAddress) || !string.IsNullOrEmpty(query))
            {
                if (query != null)
                {
                    queryFilteredResults = allSellersProducts.Where(
                                       product =>
                                       product.localProduct.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                                       ).ToList();
                }

                //check if home address is there
                if (homeAddress != null)
                {
                    //if query is present, then take query filtered result, else take all products
                    List<SellerProduct> items = new List<SellerProduct>();
                    items = query != null ? queryFilteredResults : allSellersProducts.ToList();
                    //sorting first by distance then by address
                    items = items.OrderBy(x => x.distance).ThenBy(i => i.sellerAddress.Address).ToList();
                    //loop all the products
                    foreach (var item in items)
                    {
                        //for each product, call google api to calculate distance from the use entered location
                        var distance = GoogleMapsHelper.CalculateDistance(homeAddress, item.sellerAddress.Address);
                        //set returned value to the property
                        item.distance = distance;

                        resultsWithAddress.Add(item);
                    }
                }

                listOfQueriedProducts = homeAddress != null ? resultsWithAddress : queryFilteredResults;
            }
            else
            {
                listOfQueriedProducts = allSellersProducts;
            }
            return View("SearchResultsList", listOfQueriedProducts);
        }
    }
}