using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Models
{
    public class FilterOptions
    {
        public string Category { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string Quote { get; set; }
        public string Seller { get; set; }

    }
}
