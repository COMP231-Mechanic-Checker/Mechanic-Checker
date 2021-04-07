using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MechanicChecker.Models;

namespace MechanicChecker.Models
{
    public class Seller
    {
        //private SellerContext context;
        [Key]
        public int SellerId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string AccountType { get; set; }
        public Boolean IsApproved { get; set; }
        public string CompanyName { get; set; }
        public string BusinessPhone { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Application { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime ApprovalDate { get; set; }
    
    }
}
