using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Models
{
    public class ExternalAPIsContext
    {
        public string ConnectionString { get; set; }

        public ExternalAPIsContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<ExternalAPIs> GetAllAPIs()
        {
            List<ExternalAPIs> list = new List<ExternalAPIs>();

            using (MySqlConnection connec = GetConnection())
            {
                connec.Open();
                MySqlCommand comd = new MySqlCommand("select * from APIKey ", connec);

                using (var reader = comd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ExternalAPIs()
                        {
                            APIKeyId = Convert.ToInt32(reader["APIKeyId"]),
                            Service = reader["Service"].ToString(),
                            APIKey = reader["APIKey"].ToString(),
                            KeyOwner = reader["KeyOwner"].ToString(),
                            Quota = Convert.ToInt32(reader["Quota"]),
                            ActiveDate = Convert.ToDateTime(reader["ActiveDate"].ToString()),
                            ExpireDate = Convert.ToDateTime(reader["ExpireDate"].ToString()),
                            APIHost = reader["APIHost"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public ExternalAPIs GetApiByService(string apiName, string keyOwner)
        {
            //List<ExternalAPIs> list = new List<ExternalAPIs>();
            ExternalAPIs externalAPI = null;

            using (MySqlConnection connec = GetConnection())
            {
                connec.Open();
                MySqlCommand comd = new MySqlCommand("select * from APIKey where Service = '" + apiName +
                "' AND KeyOwner = '" + keyOwner + "';", connec);

                using (var reader = comd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        externalAPI = new ExternalAPIs()
                        {
                            APIKeyId = Convert.ToInt32(reader["APIKeyId"]),
                            Service = reader["Service"].ToString(),
                            APIKey = reader["APIKey"].ToString(),
                            KeyOwner = reader["KeyOwner"].ToString(),
                            IsEnabled = Convert.ToBoolean(reader["IsEnabled"]),
                            Quota = Convert.ToInt32(reader["Quota"]),
                            ActiveDate = Convert.ToDateTime(reader["ActiveDate"].ToString()),
                            APIHost = reader["APIHost"].ToString()
                        };

                        try
                        {
                            externalAPI.ExpireDate = Convert.ToDateTime(reader["ExpireDate"].ToString());
                        }
                        catch (Exception e)
                        {
                            externalAPI.ExpireDate = null;
                        }

                    }
                }
            }

            return externalAPI;
        }


        public void activateAPI(string apiService, string keyOwner)
        {
            bool isPassed = false;
            string command = "select APIKey, Quota from APIKey where Service = '" + apiService +
                "' AND KeyOwner = '" + keyOwner + "';";

            //send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            string returnedAPIKey = null;
            string returnedQuota = null;
            using (var reader = myCommand.ExecuteReader())
            {

                if (reader.Read())
                {
                    Debug.WriteLine(reader["APIKey"].ToString());
                    returnedAPIKey = reader["APIKey"].ToString();
                    returnedQuota = reader["Quota"].ToString();

                    isPassed = true;
                }
            }

            if (isPassed && Convert.ToInt32(returnedQuota) > 0)
            {
                string stringCmd = "UPDATE APIKey SET Quota = Quota - 1 WHERE Service = '" + apiService +
                "' AND KeyOwner = '" + keyOwner + "';";

                MySqlCommand secondCommand = new MySqlCommand(stringCmd);
                secondCommand.Connection = myConnection;
                secondCommand.ExecuteNonQuery();

            }
            myConnection.Close();
        }
    }
}

