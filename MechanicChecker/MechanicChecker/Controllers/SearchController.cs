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
                listOfQueriedProducts = allSellersProducts;

            }
            return View("SearchResultsList", listOfQueriedProducts);
        }

    }
}
