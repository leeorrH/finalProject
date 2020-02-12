﻿using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace webTry2.Controllers
{
    public abstract class DBController : Controller
    {
        private string connectionString;
        protected SqlConnection connectionToSql;
        public SqlCommand command;
        public static SqlDataReader dataReader;
        public String sqlQuery;

        public DBController()
        {
            //initilaize parameters for controller
            connectionString = "Data Source=DESKTOP-7NV1617\\SQLEXPRESS;" +
                                      "Initial Catalog=project_DB;" +
                                         "Integrated Security=True";

            connectionToSql = new SqlConnection(connectionString);
        }

        public void connectToSQL()
        {
            try
            {
                connectionToSql.Open();
            }
            catch (SqlException)
            {
                Console.WriteLine("error while connecting to DB");
            }
        }

        public void closeConnectionAndReading()
        {
            connectionToSql.Close();
            dataReader.Close();
        }

        public SqlDataReader sendSqlQuery(string sqlQuery)
        {
            command = new SqlCommand(sqlQuery, connectionToSql);
            dataReader = command.ExecuteReader();
            return dataReader;
        }
        // sql query for Insert Delete or Update
        public int sqlIUDoperation (string sqlIUDquery)
        {
            command = new SqlCommand(sqlIUDquery, connectionToSql);
            int numberOfRowsEffected = (int)command.ExecuteNonQuery();
            return numberOfRowsEffected;
        }
    }
}