using MechanicChecker.Models;
using MechanicChecker.UnitTests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mechanicCheckerTest.TestMethods
{
    public class localProductTests
    {
        private Mock<IEntityRepository<LocalProduct>> productLocal;
        private List<LocalProduct> product;
        [SetUp]
        public void Setup()
        {
            productLocal = new Mock<IEntityRepository<LocalProduct>>();
            product = new List<LocalProduct>();
            product.Add(new LocalProduct() { LocalProductId=1,Title="Tires",Price="79",ImageUrl="url",IsVisible=false,ProductUrl="url",IsQuote=true,sellerId="5",Category="Car parts",Description="This is a tire"});//Add table details

        }

        [Test]
        public void Test1()
        {
            //Act
            productLocal.Setup(a => a.GetAllQueryAble()).Returns(product.AsQueryable());

            //Arrange
            var bl = new LocalProductBL(productLocal.Object); // change to the class similar to BL
            var List = bl.GetAllActiveProduts();


           
            Assert.IsTrue(List.Count == 2);
            Assert.IsTrue(List.All(S => S.IsVisible == false));
        }
    }
}
