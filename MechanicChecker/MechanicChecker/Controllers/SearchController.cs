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


        public ViewResult SearchLocalSellersProducts(string query) // the value gotten from the url
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

            IEnumerable<SellerProduct> listOfQueriedProducts;

            var allSellersProducts = context.GetAllSellerProducts();

            if (query != null)
            {

                listOfQueriedProducts = allSellersProducts.Where(
                   product =>
                   product.localProduct.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                   );
            }
            else
            {
               return View("~/Views/Home/Index.cshtml");
            }
            ViewBag.SearchQuery = query;
            return View("SearchResultsList", listOfQueriedProducts);
        }

        public ViewResult Filter(string filterType, double minPrice, double maxPrice, string searchquery, string sellerid) 
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

            IEnumerable<SellerProduct> listOfQueriedProducts;
            IEnumerable<SellerProduct> listOfOriginalQueriedProducts;

            var allSellersProducts = context.GetAllSellerProducts();

            listOfOriginalQueriedProducts = allSellersProducts.Where(
                product =>
                product.localProduct.Title.Contains(searchquery, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(searchquery, StringComparison.OrdinalIgnoreCase)
                );

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
    }
}
