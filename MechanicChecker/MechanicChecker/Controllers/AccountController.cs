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

        [HttpPost]
        public IActionResult SignIn(string userId, string password)
        {
            //SellerProductContext sellerProductContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;
            SellerContext sellerContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            Seller validSeller = sellerContext.GetSeller(userId);

            if (validSeller != null)
            {
                if (validSeller.IsApproved)
                {

                    Utility u = new Utility();
                    bool isValidSeller = u.verifyPassword(password, validSeller.PasswordHash);

                    if (isValidSeller)
                    {
                        HttpContext.Session.SetString("username", validSeller.UserName);
                        HttpContext.Session.SetString("firstname", validSeller.FirstName);
                        HttpContext.Session.SetString("lastname", validSeller.LastName);
                        ViewBag.UserName = validSeller.UserName;
                        return RedirectToAction("Index", "LocalSeller");
                    }
                    else
                    {
                        ViewBag.Error = "Username or password is invalid.";
                        return View("../Account/SignIn");
                    }
                }
                else
                {
                    ViewBag.Error = "Please Verify your account through your email";
                    return View("../Account/SignIn");
                }
            }
            else
            {
               
                ViewBag.Error = "Username or password is invalid.";
                return View("../Account/SignIn");
               
            }

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
            //ExternalAPIsContext contextAPIs = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.ExternalAPIsContext)) as ExternalAPIsContext;

            Seller newSeller = RegisterSeller(formCollection);

            string activationLink = websiteUrl + newSeller.ActivationCode;
            ExternalAPIsContext contextAPIs = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.ExternalAPIsContext)) as ExternalAPIsContext;

            string toName = newSeller.FirstName;
            string email = newSeller.Email;
            string username = newSeller.UserName;
            string phone = newSeller.BusinessPhone;
            string companyName = newSeller.CompanyName;



            if (context.GetSeller(email) != null)
            {

                ViewData["PostSignUp"] = "This Email Address has already been taken please use another";
                return View("SignUp");
            }
            else
            {

                if (context.GetSeller(username) != null)
                {
                    ViewData["PostSignUp"] = "This Username has already been taken please user another one";
                    return View("SignUp");
                }
                else
                {
                    if (context.GetSellerByCompanyName(companyName) != null)
                    {
                        ViewData["PostSignUp"] = "This Company Name has already been taken please user another one";
                        return View("SignUp");
                    }
                    else
                    {
                        if (context.GetSellerByPhoneNumber(phone) != null)
                        {
                            ViewData["PostSignUp"] = "This Phone Number has already been taken please user another one";
                            return View("SignUp");
                        }
                        else
                        {

                            //TODO: If saving to database fails need to delete the uploaded s3 company logo image
                            if (context.saveSeller(newSeller))
                            {
                                if (RegisterAddress(formCollection))
                                {
                                    _ = EmailSender.SendActivationEmail(contextAPIs, activationLink, toName, email);
                                    TempData["SignIn"] = "Congrats your account has been created! Check your email to verify your email account";

                                    return RedirectToAction("SignIn");
                                }
                                else
                                {

                                    // To delete company logo image from aws
                                    string productUrlToDelete = newSeller.CompanyLogoUrl;
                                    string filenameToDelete = productUrlToDelete.Split('/').Last<string>();
                                    AmazonS3Uploader s3Upload = new AmazonS3Uploader();
                                    try
                                    {
                                        s3Upload.AWSdelete(filenameToDelete, "seller");
                                    }
                                    catch (Exception e)
                                    { }
                                    context.DeleteSellerByCompanyName(companyName);

                                    //TODO: Need useful error messages to tell the user what is wrong
                                    ViewData["PostSignUp"] = "Invalid Postal Code!";
                                    return View("SignUp");
                                }
                            }
                            else
                            {
                                //TODO: Need useful error messages to tell the user what is wrong
                                ViewData["PostSignUp"] = "Something went wrong! Please review your account information and try again.";
                                return View("SignUp");

                            }
                        }
                    }
                }
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
            ExternalAPIsContext contextAPIs = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.ExternalAPIsContext)) as ExternalAPIsContext;
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
                _ = EmailSender.SendResetPasswordEmail(contextAPIs, resetPasswordLink, sellerName, sellerEmail);

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
                awsS3CompanyLogoUrl = s3Upload.value(companyImgStream, "seller");
            }
            catch (Exception e)
            {
                awsS3CompanyLogoUrl = "https://mechanicchecker.s3.amazonaws.com/seller/default/unnamed.jpg";
            }

            //TODO: Replace when form validation for signup page is fixed
            // placeholder to prevent application crash
            string urlWebsite = "http://mechanicchecker.us-east-1.elasticbeanstalk.com/";
            if (!string.IsNullOrEmpty(formCollection["WebsiteUrl"].ToString()))
            {
                urlWebsite = formCollection["WebsiteUrl"].ToString().Trim();
            }
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
                WebsiteUrl = urlWebsite,
                ActivationCode = Guid.NewGuid().ToString()
            };
            return newSeller;
        }
        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("firstname");
            HttpContext.Session.Remove("lastname");
            return RedirectToAction("Index", "Home");

        }

        public bool RegisterAddress(IFormCollection formCollection)
        {
            SellerContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerContext)) as SellerContext;

            string sellerId = context.GetUserIdByUserName(formCollection["UserName"].ToString().Trim());
            SellerAddress newSellerAddress = new SellerAddress()
            {
                Address = formCollection["Address"].ToString().Trim(),
                City = formCollection["City"].ToString().Trim(),
                PostalCode = formCollection["PostalCode"].ToString().Trim(),
                Province = formCollection["Province"].ToString().Trim(),
                SellerId = Convert.ToInt32(sellerId),

            };

            SellerAddressContext addressContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerAddressContext)) as SellerAddressContext;
            return addressContext.SaveSellerAddress(newSellerAddress);

        }


    }
}



