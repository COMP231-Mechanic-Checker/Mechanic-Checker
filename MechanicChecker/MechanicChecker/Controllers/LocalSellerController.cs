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
        public ActionResult SearchProducts(string keyword)
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
                       product.localProduct.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)
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


        // GET: LocalSellerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LocalSellerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LocalSellerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LocalSellerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LocalSellerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LocalSellerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LocalSellerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
