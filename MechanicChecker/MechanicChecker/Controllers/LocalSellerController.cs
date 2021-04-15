using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MechanicChecker.Controllers
{
    public class LocalSellerController : Controller
    {
        //global variables for the list of all the products belonging to the seller, and context
        private List<SellerProduct> currentSellerProducts = new List<SellerProduct>();
        private SellerProductContext context;

        // GET: LocalSellerController
        public ActionResult Index(string userName)
        {

            SellerProductContext sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            SellerContext sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            Seller seller = sellerContext.GetSeller(userName);
            var allSellerProducts = sellerProductContext.GetAllSellerProducts();
            var sellerProducts = allSellerProducts.Where(p => p.seller.UserName.Equals(seller.UserName));
            return View("../LocalSeller/SellerLandingPage", sellerProducts);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchProducts(string username, string keyword)
        {
            try
            {

                context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
                currentSellerProducts = (List<SellerProduct>)context.GetAllSellerProducts();
                IEnumerable<SellerProduct> searchedSellerProducts = new List<SellerProduct>();
                if (keyword != null)
                {
                    searchedSellerProducts = currentSellerProducts.Where(
                       product =>
                       (product.localProduct.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            && product.seller.UserName.Equals(username)
                       );
                }
                else
                {
                    searchedSellerProducts = currentSellerProducts;                
                }
                return View("SellerLandingPage", searchedSellerProducts);
            }
            catch
            {
                return View("SellerLandingPage", currentSellerProducts);
            }
        }



        // GET: LocalSellerController/Create
        public ActionResult Create(int id, string sellerID)
        {
            context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            currentSellerProducts = (List<SellerProduct>)context.GetAllSellerProducts();
            ViewBag.CurrentSeller = currentSellerProducts.FirstOrDefault().seller.UserName;
            return View("Create");
        }

        // POST: LocalSellerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return View("Create");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
