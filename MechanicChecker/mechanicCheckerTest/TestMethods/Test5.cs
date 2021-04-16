using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using MechanicChecker.Controllers;
using Assert = NUnit.Framework.Assert;

namespace mechanicCheckerTest.TestMethods
{
    class Test5
    {
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
