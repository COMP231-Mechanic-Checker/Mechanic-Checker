using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechanicChecker.AWS;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MechanicChecker.Controllers
{
    public class LocalSellerController : Controller
    {
        //global variables for the list of all the products belonging to the seller, and context
        private List<SellerProduct> currentSellerProducts = new List<SellerProduct>();
        private SellerProductContext sellerProductContext;
        private SellerContext sellerContext;
        private LocalProductContext localProductContext;

        // GET: LocalSellerController
        public ActionResult Index()
        {
            string userName = HttpContext.Session.GetString("username");
            sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
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

                sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
                currentSellerProducts = (List<SellerProduct>)sellerProductContext.GetAllSellerProducts();
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
        public ActionResult Create(string username)
        {
            sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
            Seller seller = sellerContext.GetSeller(username);
            LocalProduct localProduct = new LocalProduct()
            {
                sellerId = seller.SellerId.ToString()
            };
            //localProduct = 
            return View(localProduct);
        }

        // POST: LocalSellerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection formCollection)
        {
            localProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            LocalProduct newProduct = CreateProduct(formCollection);
            //TODO: If saving to database fails need to delete the uploaded s3 company logo image
            if (localProductContext.saveProduct(newProduct))
            {
                TempData["CreateProduct"] = "Your product has been added!";
                return RedirectToAction("Index", "LocalSeller");
            }
            else
            {
                //TODO: Need useful error messages to tell the user what is wrong
                ViewData["CreateErrorMsg"] = "Something went wrong! Please review your product information.";
                return View("Create");

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public LocalProduct CreateProduct(IFormCollection formCollection)
        {
            //IFormFile imageUrlStream = formCollection.Files.FirstOrDefault();
            IFormFile companyImgStream = formCollection.Files.FirstOrDefault();
            S3FileUploader s3Upload = new S3FileUploader();
            string awsS3CompanyLogoUrl;

            // if something goes wrong uploading to s3 use placeholder company logo url
            try
            {
                //awsS3CompanyLogoUrl = s3Upload.value(imageUrlStream, "product");
                awsS3CompanyLogoUrl = s3Upload.value(companyImgStream, "product");
            }
            catch (Exception e)
            {
                awsS3CompanyLogoUrl = "https://s3.amazonaws.com/mechanic.checker/product/default/unnamed.jpg";
            }

            //string sellerWebsiteUrl = "https://michaelasemota.netlify.app/";

            LocalProduct newProduct = new LocalProduct()
            {
                Category = formCollection["Category"].ToString().Trim(),
                Title = formCollection["Title"].ToString().Trim(),
                Price = formCollection["Price"].ToString().Trim(),
                Description = formCollection["Description"].ToString().Trim(),
                ImageUrl = awsS3CompanyLogoUrl,
                ProductUrl = formCollection["ProductUrl"].ToString().Trim(),
                IsQuote = Convert.ToBoolean(formCollection["IsQuote"].ToString().Split(',')[0]),
                IsVisible = true,
                sellerId = formCollection["SellerId"].ToString()
            };

            return newProduct;
            // return View();

        }
        // GET: LocalSellerController/Details/5
        public ActionResult Details(int id, string sellerID)
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            currentSellerProducts = (List<SellerProduct>)context.GetAllSellerProducts();
            IEnumerable<SellerProduct> searchedSellerProducts = new List<SellerProduct>();
            if (currentSellerProducts.Count() > 0)
            {

                //searchedSellerProducts = currentSellerProducts ;
                searchedSellerProducts = currentSellerProducts.Where(
                   product =>
                   product.localProduct.LocalProductId == id && product.localProduct.sellerId == sellerID
                   );
                ViewBag.CurrentSeller = searchedSellerProducts.FirstOrDefault().seller.UserName;
                return View("Details", searchedSellerProducts.FirstOrDefault().localProduct);
            }
            else
            {
                return View("SellerLandingPage", searchedSellerProducts);
            }

        }


    }
}
