using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechanicChecker.Models;
using Microsoft.AspNetCore.Mvc;

namespace MechanicChecker.Controllers
{
    public class SearchController : Controller
    {
    
        public IActionResult Index()
        {
            LocalProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.LocalProductContext)) as LocalProductContext;
            return View(context.GetAllProducts());
        }

        public ViewResult SearchLocalSellersProducts(string homeAddress, string query) // the value gotten from the url
        {
            SellerProductContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerProductContext)) as SellerProductContext;

            IEnumerable<SellerProduct> listOfQueriedProducts;
            List<SellerProduct> queryFilteredResults = new List<SellerProduct>();
            List<SellerProduct> resultsWithAddress = new List<SellerProduct>();

            var allSellersProducts = context.GetAllSellerProducts();

            if (!string.IsNullOrEmpty(homeAddress) || !string.IsNullOrEmpty(query))
            {
                if (query != null)
                {
                    queryFilteredResults = allSellersProducts.Where(
                                       product =>
                                       product.localProduct.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || product.localProduct.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                                       ).ToList();
                }
                
                //check if home address is there
                if(homeAddress != null)
                {  
                    //if query is present, then take query filtered result, else take all products
                    List<SellerProduct> items = new List<SellerProduct>();
                    items = query != null ? queryFilteredResults : allSellersProducts.ToList();
                    //sorting first by distance then by address
                    items = items.OrderBy(x => x.distance).ThenBy(i => i.sellerAddress.Address).ToList();
                    //loop all the products
                    foreach (var item in items)
                    {
                        //for each product, call google api to calculate distance from the use entered location
                        var distance = GoogleMapsHelper.CalculateDistance(homeAddress, item.sellerAddress.Address);
                        //set returned value to the property
                        item.distance = distance;

                        resultsWithAddress.Add(item);
                    }                    
                }

                listOfQueriedProducts = homeAddress != null ? resultsWithAddress : queryFilteredResults;
            }
            else
            {
                listOfQueriedProducts = allSellersProducts;
            }
            return View("SearchResultsList", listOfQueriedProducts);
        }

    }
}
