using MechanicChecker.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;

            return View(context.GetAllProducts());
        }

        public IActionResult Results()
        {
            return View("SearchResultsList");
        }

        public IActionResult filterRecords(object e)
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;

            return View(context.GetAllProducts());
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ViewResult SearchCategories(String query) // the value gotten from the url
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;

            IEnumerable<LocalProduct> listOfQueriedProducts;
            var allProducts = context.GetAllProducts();


            if (query != null)
            {
                listOfQueriedProducts = allProducts.Where(
                   product =>
                   product.Category.Contains(query) 
                   );
            }
            else
            {
                listOfQueriedProducts = allProducts;

            }

            return View(listOfQueriedProducts);
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
