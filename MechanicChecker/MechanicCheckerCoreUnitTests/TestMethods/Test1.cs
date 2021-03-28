using MechanicChecker.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;


namespace MechanicCheckerCoreUnitTests.TestMethods
{
    
    [TestClass]
    class Test1
    {        

        public void TestMethod1()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestSearchResultsView()
        {


            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            //result = controller.SearchLocalSellersProducts("tire");
            Assert.AreEqual("Index", result.ViewName);

        }
     
    }
}
