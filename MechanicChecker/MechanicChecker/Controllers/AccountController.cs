using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MechanicChecker.Models;


namespace MechanicChecker.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return View("SignIn");
        }
    

    public IActionResult SignUp()
    {
        return View("SignUp");
    }

    }
}
   


