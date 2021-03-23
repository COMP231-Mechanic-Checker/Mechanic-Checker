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
            product.Add(new LocalProduct() {LocalProductId=1,Title="This is a tire",Price="45",Description="A tire runs vertically",ImageUrl="url",IsVisible=false,Category="Car Parts",ProductUrl="url",IsQuote=false,sellerId="4" });//Add table details

        }

        [Test]
        public void Test1()
        {
            //Act
            productLocal.Setup(a => a.GetAllQueryAble()).Returns(product.AsQueryable());

            //Arrange
            var bl = new LocalProductBL(productLocal.Object); // change to the class similar to BL
            var List = bl.GetAllActiveProduts();


            Assert.IsTrue(List.All(s=>s.IsVisible==false));
            Assert.IsTrue(List.Count == 2);
        }
    }
}
