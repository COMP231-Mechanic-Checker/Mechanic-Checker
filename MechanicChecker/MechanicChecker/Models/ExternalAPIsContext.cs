﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
                MySqlCommand comd = new MySqlCommand("select * from APIKey", connec);

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
    }
}
