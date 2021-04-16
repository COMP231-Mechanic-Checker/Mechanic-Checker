using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace MechanicChecker.Models
{
    public class LocalProductContext
    {
        public string ConnectionString { get; set; }

        public LocalProductContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<LocalProduct> GetAllProducts()
        {
            List<LocalProduct> list = new List<LocalProduct>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Product", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LocalProduct()
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
                        });
 
                    }
                }
            }
            return list;
        }

        public bool saveProduct(LocalProduct localProduct)
        {
            bool isPassed = true;
            try
            {
                string stringCmd = "INSERT INTO Product( SellerId, Category, Title, Price, Description, ImageUrl, ProductUrl, IsQuote, IsVisible)" +
                    "VALUES (" + Convert.ToInt32(localProduct.sellerId) + ", '" +
                   localProduct.Category + "', '" +
                   localProduct.Title + "', " +
                    Convert.ToDecimal(localProduct.Price) + ", '" +
                    localProduct.Description + "', '" +
                    localProduct.ImageUrl + "', '" +
                   localProduct.ProductUrl + "', " +
                    Convert.ToInt32(localProduct.IsQuote) + ", " +
                    Convert.ToInt32(localProduct.IsVisible) + ");";

                Debug.WriteLine(stringCmd);
                MySqlConnection myConnection = GetConnection();
                MySqlCommand myCommand = new MySqlCommand(stringCmd);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB
                myCommand.Connection.Close();

            }
            catch (Exception e)
            {
                isPassed = false;
                return isPassed;
            }
            return isPassed;
        }
       
        //Delete item from seller list
        public bool deleteProduct(int selllerId, int productId)
        {
            try
            {
                string stringCmd = $"DELETE FROM Product WHERE SellerId={selllerId} AND ProductId={productId}";

                Debug.WriteLine(stringCmd);
                MySqlConnection myConnection = GetConnection();
                MySqlCommand myCommand = new MySqlCommand(stringCmd);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB
                myCommand.Connection.Close();

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool editProduct(LocalProduct localProduct)
        {
            bool isPassed = true;
            try
            {
                string stringCmd = "UPDATE Product set " +
                    "Category = '" + localProduct.Category + "' , " +
                    "Title = '" + localProduct.Title + "' , " +
                    "Price= '" + Convert.ToDecimal(localProduct.Price) + "' , " +
                    "Description = '" + localProduct.Description + "' , " +
                    "ImageUrl = '" + localProduct.ImageUrl + "' , " +
                    "IsQuote = '" + Convert.ToInt32(localProduct.IsQuote) + "' , " +
                    "ProductUrl = '" + localProduct.ProductUrl + "' where ProductId = " + localProduct.LocalProductId + ";";

                Debug.WriteLine(stringCmd);
                MySqlConnection myConnection = GetConnection();
                MySqlCommand myCommand = new MySqlCommand(stringCmd);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB
                myCommand.Connection.Close();

            }
            catch (Exception e)
            {
                isPassed = false;
                return isPassed;
            }
            return isPassed;
        }

        public LocalProduct getLocalProduct(int productId)
        {
            LocalProduct LocalProduct = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Product " +
                    "where ProductId = " + productId + ";", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LocalProduct = new LocalProduct()
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

                    }
                }
            }

            return LocalProduct;
        }

    }
}
