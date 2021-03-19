using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MechanicChecker.Models
{
    public class LocalProduct
    {
        private LocalProductContext context;
        [Key]
        public int LocalProductId { get; set; }
        //[Required(ErrorMessage = "Please enter your name")]
        public string Title { get; set; }
        //[Required(ErrorMessage = "Please choose the amount of servings")]
        public string Price { get; set; }
        //[Required(ErrorMessage = "Please enter your dish name")]
        public string Description { get; set; }
       // [Required(ErrorMessage = "Please enter the description of your dish")]
        public string ImageUrl { get; set; }
        //[Required(ErrorMessage = "Please enter the Instructions of your dish")]
        public bool IsVisible { get; set; }
        public string Category { get; set; }
        public string ProductUrl { get; set; }
        public bool IsQuote { get; set; }
        public string sellerId { get; set; }
    }
}