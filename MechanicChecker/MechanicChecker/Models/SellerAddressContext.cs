using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool SaveSellerAddress(SellerAddress sellerAddress)
        {
            bool isPassed = true;
            try
            {
                // Note: The Application date can be null because it is being generated in the DB
                string stringCmd =
                    "INSERT INTO Address(SellerId, Address, City, Province, PostalCode) " +
                    "VALUES ("
                    + sellerAddress.SellerId + ", '"
                    + sellerAddress.Address + "', '"
                    + sellerAddress.City + "', '"
                    + sellerAddress.Province + "', '"
                    + sellerAddress.PostalCode + "')";

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

    }
}
