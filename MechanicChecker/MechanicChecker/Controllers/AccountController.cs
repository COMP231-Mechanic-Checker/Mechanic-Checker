using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Http;
using MechanicChecker.Helper;
using MechanicChecker.AWS;

namespace MechanicChecker.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return View("SignIn");
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            return View("SignUp");
        }

        [HttpPost]
        public IActionResult SignUp(IFormCollection formCollection)
        {

            string hostUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            string websiteUrl = hostUrl + "/Account/ActivateEmail/";
            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            Seller newSeller = RegisterSeller(formCollection);

            string activationLink = websiteUrl + newSeller.ActivationCode;

            string toName = newSeller.FirstName;
            string email = newSeller.Email;

            //TODO: If saving to database fails need to delete the uploaded s3 company logo image
            if (context.saveSeller(newSeller))
            {

                //TODO: Send email and return confirmation page

                var sdfsad = EmailSender.SendActivationEmail(activationLink, toName, email);
                ViewData["SignIn"] = "Congrats your account has been created! Check your email to verify your email account";

                return View("SignIn");
            }
            else
            {
                ViewData["PostSignUp"] = "Unfortunately your account has not been created. Please try again";

                //TODO: Do not send email and return a message/page that something went wrong with signup
                return View("SignUp");

            }
        }

        public IActionResult ActivateEmail(string id) //value gotten from the url
        {
            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            if (context.activateSellerAccount(id))
            {
                ViewData["SignIn"] = "Congrats! Your account email has been verified";

                //Return Confirmation Message
                return View("SignIn");
            }
            else
            {
                ViewData["SignIn"] = "Please Try Again your account email has not been verified";

                //Return Error Message
                return View("SignIn");
            }

        }

        public Seller RegisterSeller(IFormCollection formCollection)
        {

            IFormFile companyImgStream = formCollection.Files.First();
            S3FileUploader s3Upload = new S3FileUploader();
            string awsS3CompanyLogoUrl;

            // if something goes wrong uploading to s3 use placeholder company logo url
            try
            {
                awsS3CompanyLogoUrl = s3Upload.value(companyImgStream);
            }
            catch(Exception e)
            {
                awsS3CompanyLogoUrl = "https://s3.amazonaws.com/mechanic.checker/seller/default/unnamed.jpg";
            }

            //TODO: Replace when form validation for signup page is fixed
            // placeholder to prevent application crash
            string sellerWebsiteUrl = "https://michaelasemota.netlify.app/";

            Seller newSeller = new Seller()
            {
                UserName = formCollection["UserName"].ToString().Trim(),
                AccountType = formCollection["AccountType"].ToString().Trim(),
                FirstName = formCollection["FirstName"].ToString().Trim(),
                LastName = formCollection["LastName"].ToString().Trim(),
                Email = formCollection["Email"].ToString().Trim(),
                PasswordHash = Utility.encryptPassword(formCollection["Password"]),
                Application = formCollection["Application"].ToString().Trim(),
                BusinessPhone = formCollection["BusinessPhone"].ToString().Trim(),
                ApplicationDate = DateTime.Now,
                CompanyLogoUrl = awsS3CompanyLogoUrl,
                CompanyName = formCollection["CompanyName"].ToString().Trim(),
                IsApproved = false,
                WebsiteUrl = sellerWebsiteUrl.ToString().Trim(),
                ActivationCode = Guid.NewGuid().ToString()
            };
            return newSeller;
        }

    }
}
   


