using MechanicChecker.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace mechanicCheckerTest.TestMethods
{
    class Test4
    {
        //Unit test to see if the SearchCompareTwo returns a View
        [Test]
        public void Employees()
        {
            // Arrange
            SearchController controller = new SearchController();

            // Act
            ViewResult result = controller.SearchCompareTwoParts() as ViewResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
        }
    }
}
