using MechanicChecker.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace mechanicCheckerTest.TestMethods
{
    class Test2
    {
        //Test to see whether or not the AccountController returns the right view
        [Test]
        public void Test1()
        {
            var controller = new AccountController();
            var result = controller.SignIn() as ViewResult;
            Assert.AreEqual("SignIn", result.ViewName);
        }
    }
}
