using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MechanicChecker.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace mechanicCheckerTest.TestMethods
{
    //These three methods will test whether the Index, About and Contact action methods are working properly.
   
    class Test3
    {
        [Test]
        public void Test1()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.Results() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.About() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.Contact() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
