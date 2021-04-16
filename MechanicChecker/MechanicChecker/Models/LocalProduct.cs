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
        [Required(ErrorMessage = "Please enter Title of your product")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter price of your product")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Please enter a valid decimal Number with 2 decimal places")]
        public string Price { get; set; }
        [Required(ErrorMessage = "Please enter description your product")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please attach an image of your product")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif|.PNG|.JPG|.GIF)$", ErrorMessage = "Only Image files allowed.")]
        public string ImageUrl { get; set; }
        public bool IsVisible { get; set; }
        public string Category { get; set; }
        //[DataType(DataType.Url)]
        [Required(ErrorMessage = "Please enter product url")]
        [Url]
        public string ProductUrl { get; set; }
        public bool IsQuote { get; set; }
        public string sellerId { get; set; }
    }
}