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

        public ExternalAPIs GetApiByService(string apiName)
        {
            //List<ExternalAPIs> list = new List<ExternalAPIs>();
            ExternalAPIs externalAPI = null;

            using (MySqlConnection connec = GetConnection())
            {
                connec.Open();
                MySqlCommand comd = new MySqlCommand("select * from APIKey where Service = '" + apiName + "'", connec);

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
                            Quota = Convert.ToInt32(reader["Quota"]),
                            ActiveDate = Convert.ToDateTime(reader["ActiveDate"].ToString()),
                            ExpireDate = Convert.ToDateTime(reader["ExpireDate"].ToString()),
                            APIHost = reader["APIHost"].ToString()
                        };
                    }
                }
            }

            return externalAPI;
        }

        
        public bool activateAPI(string apiService)
        {
            bool isPassed = false;
            string command = "select * from APIKey where Service = '" + apiService + "';";

            //send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            //read response
            using (var reader = myCommand.ExecuteReader())
            {
                //Will only run if the query returns a record
                if(reader.Read())
                {
                    Debug.WriteLine(reader["Service"].ToString());
                    isPassed = true;
                }
            }

            if (isPassed.Equals(true))
            {
                string stringCmd = "UPDATE APIKey SET Quota = 1 WHERE Service = '" + apiService + "';";

                //ExecuteNonQuery Function is what allows us to update, insert and delete from the DB              
                MySqlCommand secondCommand = new MySqlCommand(stringCmd);
                secondCommand.Connection = myConnection;                
                secondCommand.ExecuteNonQuery();
            }
            myConnection.Close();

            return isPassed;
        }
    }
}

