using System;
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

        public bool connectToSQL()
        {
            bool res = true;
            try
            {
                connectionToSql.Open();
            }
            catch (SqlException)
            {
                res = false;
                throw new Exception("error while connecting to DB");
            }

            return res;
        }

        public bool closeConnectionAndReading()
        {
            bool res = true;
            try
            {
                connectionToSql.Close();
            }
            catch
            {
                res = false;
                throw new Exception("connection to sql is closed\n");
            }
            try
            {
                dataReader.Close();
            }
            catch
            {
                res = false;
                //throw new Exception("datareader is closed\n");
            }

            return res;
        }

        public SqlDataReader sendSqlQuery(string sqlQuery)
        {
            command = new SqlCommand(sqlQuery, connectionToSql);
            if(dataReader != null)
            {
                if (!dataReader.IsClosed)
                {
                    closeConnectionAndReading();
                    connectToSQL();
                }
            }
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