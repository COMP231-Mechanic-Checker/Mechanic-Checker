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
        [HttpGet]
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
            //TODO: Need form validation

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

                _ = EmailSender.SendActivationEmail(activationLink, toName, email);
                TempData["SignIn"] = "Congrats your account has been created! Check your email to verify your email account";

                return RedirectToAction("SignIn");
            }
            else
            {
                //TODO: Need useful error messages to tell the user what is wrong
                ViewData["PostSignUp"] = "Something went wrong! Please create your account again";
                return View("SignUp");

            }
        }

        public IActionResult ActivateEmail(string id) //value gotten from the url
        {
            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            if (context.activateSellerAccount(id))
            {
                TempData["SignIn"] = "Congrats! Your account email has been verified";

                //Return Confirmation Message
                return RedirectToAction("SignIn");
            }
            else
            {
                TempData["SignIn"] = "Something went wrong! Please check your activation email again";

                //Return Error Message
                return RedirectToAction("SignIn");
            }

        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View("ResetPassword");
        }

        [HttpPost]
        public IActionResult ResetPassword(IFormCollection formCollection)
        {
            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;
            int codeLength = 30;
            string resetPasswordCode = Utility.RandomString(codeLength);

            string hostUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            string websiteUrl = hostUrl + "/Account/UpdatePassword/";

            string resetPasswordLink = websiteUrl + resetPasswordCode;

            string sellerEmail = formCollection["Email"].ToString().Trim();
            
            string sellerName;
            
            if (context.verifyUserByEmail(sellerEmail, out sellerName))
            {

                context.updateResetPasswordCode(resetPasswordCode, sellerEmail);
                // send email and return confirmation page
                _ = EmailSender.SendResetPasswordEmail(resetPasswordLink, sellerName, sellerEmail);
                TempData["SignIn"] = "A reset password link has been sent to the email address you entered";

                return RedirectToAction("SignIn");
            }
            else
            {
                TempData["ResetPassword"] = "This email address is not associated with Mechanic Checker";

                return View("ResetPassword");
            }

        }

        [HttpGet]
        public IActionResult UpdatePassword(string id) //value gotten from the url
        {
            ViewBag.id = id;
            return View("UpdatePassword");
        }

        [HttpPost]
        public IActionResult UpdatePassword(IFormCollection formCollection)
        {

            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            string resetId = formCollection["resetCode"];

            string password = formCollection["password"];
            string encryptedPassword = Utility.encryptPassword(password);

            if (context.updatePassword(encryptedPassword, resetId))
            {
                TempData["SignIn"] = "Congrats! Your password has been updated";
                return RedirectToAction("SignIn");
            }
            else
            {
                TempData["SignIn"] = "Something went wrong! Please check your password reset email again";

                //Return Error Message
                return RedirectToAction("SignIn");
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
   


