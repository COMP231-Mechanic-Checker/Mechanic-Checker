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

        public ViewResult Search(String query) // the value gotten from the url
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;

            IEnumerable<LocalProduct> listOfQueriedProducts;
            var allProducts = context.GetAllProducts();
          

            if (query != null)
            {
                listOfQueriedProducts = allProducts.Where(
                   product =>
                   product.Title.Contains(query) || product.Description.Contains(query)
                   );
            }else
            { 
                listOfQueriedProducts = allProducts;
               
            }

            return View("SearchResultsList", listOfQueriedProducts);
        }
    }
}
