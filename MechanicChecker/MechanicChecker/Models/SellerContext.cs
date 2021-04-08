using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MechanicChecker.Models
{
    public class SellerContext
    {
        public string ConnectionString { get; set; }
        public SellerContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public bool saveSeller(Seller seller)
        {
            bool isPassed = true;
            try
            {
                // Note: The Application date can be null because it is being generated in the DB
                string stringCmd =
                    "INSERT INTO Seller(Username, FirstName, LastName, Email, PasswordHash, AccountType, IsApproved, CompanyName, BusinessPhone, CompanyLogoUrl, WebsiteUrl, Application, ApplicationDate, ActivationCode) " +
                    "VALUES ('"
                    + seller.UserName + "', '"
                    + seller.FirstName + "', '"
                    + seller.LastName + "', '"
                    + seller.Email + "', '"
                    + seller.PasswordHash + "', '"
                    + seller.AccountType + "', "
                    + Convert.ToInt32(seller.IsApproved) + ", '"
                    + seller.CompanyName + "', '"
                    + seller.BusinessPhone + "', '"
                    + seller.CompanyLogoUrl + "', '"
                    + seller.WebsiteUrl + "', '"
                    + seller.Application + "', '"
                    /*
                     * Even though application date is automatically generated via the database the database operates with a different timezone.
                     * Better to manually set the date dynamically on the website.
                     */
                    + seller.ApplicationDate.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                    + seller.ActivationCode + "')"; 

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

        public bool activateSellerAccount(string activationId)
        {
            bool isPassed = false;

            string command = "SELECT Username, IsApproved from Seller where ActivationCode = '" + activationId + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            // read response
            using (var reader = myCommand.ExecuteReader())
            {
                // will only run if the query returns a record
                isPassed = reader.Read() && (Convert.ToBoolean(reader["IsApproved"]) == false);
            }

            if (isPassed.Equals(true))
            {
                string stringCmd = "UPDATE Seller SET IsApproved = 1, ApprovalDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE ActivationCode = '" + activationId + "';";

                MySqlCommand secondCommand = new MySqlCommand(stringCmd);
                secondCommand.Connection = myConnection;
                secondCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB 

            }
            myConnection.Close();

            return isPassed;

        }

    }
}
