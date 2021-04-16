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
        private SellerAddressContext sellerAddressContext;

        // GET: LocalSellerController
        public ActionResult Index()
        {
            string userName = HttpContext.Session.GetString("username");
            if(userName != null)
            {
                
                sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
                sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
                Seller seller = sellerContext.GetSeller(userName);
                var allSellerProducts = sellerProductContext.GetAllSellerProducts();
                var sellerProducts = allSellerProducts.Where(p => p.seller.UserName.Equals(seller.UserName));

                return View("../LocalSeller/SellerLandingPage", sellerProducts);

            }else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchProducts(string username, string keyword)
        {
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
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
                        searchedSellerProducts = currentSellerProducts.Where(
                           product =>
                           (
                              product.seller.UserName.Equals(username)
                           ));
                    }
                    return View("SellerLandingPage", searchedSellerProducts);
                }
                catch
                {
                    return View("SellerLandingPage", currentSellerProducts);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }


        // GET: LocalSellerController/Create
        public ActionResult Create(/*string username*/)
        {

            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
            {
                sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
                Seller seller = sellerContext.GetSeller(userName);
                LocalProduct localProduct = new LocalProduct()
                {
                    sellerId = seller.SellerId.ToString()
                };
                //localProduct = 
                return View(localProduct);
            }
            else
            {
                return RedirectToAction("SignIn", "Account");

            }
        }

        // POST: LocalSellerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection formCollection)
        {
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
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
            else
            {
                return RedirectToAction("SignIn", "Account");

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
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
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
            else
            {
                return RedirectToAction("SignIn", "Account");
            }

        }

       
            //Delete item from seller account
        public IActionResult Delete(string imageUrl, int sellerID, int productId)
        {
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
            {
                localProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;            
            var result = localProductContext.deleteProduct(sellerID, productId);
     
            // To delete product image from aws
            string productUrlToDelete = imageUrl;
            string filenameToDelete = productUrlToDelete.Split('/').Last<string>();
            AmazonS3Uploader s3Upload = new AmazonS3Uploader();
            try
            {
                s3Upload.AWSdelete(filenameToDelete, "product");
            }
            catch (Exception e)
            { }

            return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }

        public IActionResult DeleteViewPage()
        {
            return View("SellerDeletePage");
        }

        

        public ActionResult DeletePage(int id, string sellerID)
        {
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
            {
                sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
                currentSellerProducts = (List<SellerProduct>)sellerProductContext.GetAllSellerProducts();
                IEnumerable<SellerProduct> searchedSellerProducts = new List<SellerProduct>();
                if (currentSellerProducts.Count() > 0)
                {
                    //searchedSellerProducts = currentSellerProducts ;
                    searchedSellerProducts = currentSellerProducts.Where(
                    product =>
                    product.localProduct.LocalProductId == id && product.localProduct.sellerId == sellerID
                    );
                    ViewBag.CurrentSeller = searchedSellerProducts.FirstOrDefault().seller.UserName;
                    return View("SellerDeletePage", searchedSellerProducts.FirstOrDefault().localProduct);
                }
                else
                {
                    return View("SellerLandingPage", searchedSellerProducts);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }
        [HttpPost]
        public ActionResult DeleteItem(int sellerID, int productId)
        {
            return View("SellerDeletePage");
        }

        // GET: LocalSellerController/Create
        public ActionResult Edit(int id, string sellerID)
        {
            string userName = HttpContext.Session.GetString("username");
            if (userName != null)
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
                    return View(searchedSellerProducts.FirstOrDefault().localProduct);
                }
                else
                {
                    return View("SellerLandingPage", searchedSellerProducts);
                }
            }
           
            else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IFormCollection formCollection)
        {
            localProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            LocalProduct editedProduct = EditProduct(formCollection);
            //TODO: If saving to database fails need to delete the uploaded s3 company logo image
            if (localProductContext.editProduct(editedProduct))
            {
                LocalProduct updatedProduct = localProductContext.getLocalProduct(editedProduct.LocalProductId);
                TempData["CreateProduct"] = "Your product information has been updated successfully!";
                return View("Details", updatedProduct);
            }
            else
            {
                //TODO: Need useful error messages to tell the user what is wrong
                ViewData["CreateErrorMsg"] = "Something went wrong! Please review your product information.";
                return View("Details");

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public LocalProduct EditProduct(IFormCollection formCollection)
        {
            localProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            string awsS3CompanyLogoUrl;

            if (formCollection.Files.FirstOrDefault() != null)
            {


                IFormFile companyImgStream = formCollection.Files.FirstOrDefault();
                S3FileUploader s3Upload = new S3FileUploader();
                // if something goes wrong uploading to s3 use placeholder company logo url
                try
                {
                    awsS3CompanyLogoUrl = s3Upload.value(companyImgStream, "product");
                }
                catch (Exception e)
                {
                    awsS3CompanyLogoUrl = "https://s3.amazonaws.com/mechanic.checker/product/default/unnamed.jpg";
                }
            }
            else
            {
                awsS3CompanyLogoUrl = localProductContext.getLocalProduct(Convert.ToInt32(formCollection["LocalProductId"].ToString())).ImageUrl;
            }

            //string sellerWebsiteUrl = "https://michaelasemota.netlify.app/";

            LocalProduct updatedProduct = new LocalProduct()
            {
                LocalProductId = Convert.ToInt32(formCollection["LocalProductId"].ToString()),
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

            return updatedProduct;
            // return View();

        }

        [HttpGet]
        public IActionResult EditAccount(string userName) //value gotten from the url
        {
            string username = HttpContext.Session.GetString("username");
            if (username != null)
            {
                sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
                int currSellerId = Convert.ToInt32(sellerContext.GetUserIdByUserName(username));

                sellerAddressContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerAddressContext)) as SellerAddressContext;
                List<SellerAddress> allAddresses = sellerAddressContext.GetAllAddresses();
                SellerAddress currSellerAddress = allAddresses.Where(s => s.SellerId.Equals(currSellerId)).FirstOrDefault();
                
                
                Seller seller = sellerContext.GetSeller(userName);
                
                ViewBag.CurrentSellerAddress = currSellerAddress;
                return View("EditAccount", seller);
            }
            else
            {
                return RedirectToAction("SignIn", "Account");
            }
        }

    }
}

