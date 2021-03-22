using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Models
{
    public class SellerAddressContext
    {
        public string ConnectionString { get; set; }

        public SellerAddressContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<SellerAddress> GetAllAddresses()
        {
            List<SellerAddress> addressList = new List<SellerAddress>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Address", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        addressList.Add(new SellerAddress()
                        {
                            AddressId = Convert.ToInt32(reader["AddressId"]),
                            SellerId = Convert.ToInt32(reader["SellerId"]),
                            Address = reader["Address"].ToString(),
                            PostalCode = reader["PostalCode"].ToString(),
                            Province = reader["Province"].ToString(),
                            City = reader["City"].ToString()
                        });

                    }
                }
            }
            return addressList;
        }
    }
}
