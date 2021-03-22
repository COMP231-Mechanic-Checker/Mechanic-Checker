using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MechanicChecker.Models
{
    public class SellerProductContext
    {
        public string ConnectionString { get; set; }

        public SellerProductContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<SellerProduct> GetAllSellerProducts()
        {
            LocalProduct localProduct = new LocalProduct();
            Seller seller = new Seller();
            List<SellerProduct> list = new List<SellerProduct>();


            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Seller Join Product on Seller.SellerId = Product.SellerId where Product.IsVisible = true", conn);

                using (var reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        localProduct= new LocalProduct()
                        {
                            LocalProductId = Convert.ToInt32(reader["ProductId"]),
                            Category = reader["Category"].ToString(),
                            Title = reader["Title"].ToString(),
                            Price = reader["Price"].ToString(),
                            Description = reader["Description"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString(),
                            sellerId = reader["SellerId"].ToString(),
                            ProductUrl = reader["ProductUrl"].ToString(),
                            IsVisible = Convert.ToBoolean(reader["IsVisible"]),
                            IsQuote = Convert.ToBoolean(reader["IsQuote"])
                        };
                        seller = new Seller()
                        {
                            SellerId = Convert.ToInt32(reader["SellerId"]),
                            UserName = reader["Username"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            AccountType = reader["AccountType"].ToString(),
                            IsApproved = Convert.ToBoolean(reader["IsApproved"]),
                            CompanyName = reader["CompanyName"].ToString(),
                            BusinessPhone = reader["BusinessPhone"].ToString(),
                            CompanyLogoUrl = reader["CompanyLogoUrl"].ToString(),
                            WebsiteUrl = reader["WebsiteUrl"].ToString(),
                            Application = reader["Application"].ToString(),
                            ApplicationDate = Convert.ToDateTime(reader["ApplicationDate"]),
                            ApprovalDate = Convert.ToDateTime(reader["ApprovalDate"])
                        };
                        list.Add(new SellerProduct(localProduct, seller));

                    }

             
                }
            }
            return list;
        }
    }
}
