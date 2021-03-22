using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Models
{
    public class SellerProduct
    {
        public LocalProduct localProduct { get; set; }
        public Seller seller { get; set; }

        public SellerProduct(LocalProduct lP, Seller s)
        {
            localProduct = lP;
            seller = s;
        }

    }
}
