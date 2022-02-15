using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CoreProject.Helpers
{
    public class DBBase
    {
        /// <summary>
        /// 嘗試連接資料庫，並回傳資料
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string dbCommand, List<SqlParameter> parameters)
        {
            string connectionString = GetConnectionString();


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(dbCommand, connection);
                command.Parameters.AddRange(parameters.ToArray());

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();

                    return dt;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 取得資料筆數
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetScale(string dbCommand, List<SqlParameter> parameters)
        {
            string connectionString = GetConnectionString();


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(dbCommand, connection);

                List<SqlParameter> parameters2 = new List<SqlParameter>();
                foreach (var item in parameters)
                {
                    parameters2.Add(new SqlParameter(item.ParameterName, item.Value));
                }

                command.Parameters.AddRange(parameters2.ToArray());

                try
                {
                    connection.Open();
                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 執行SQL指令，如途中有錯誤則拋回錯誤並取消執行
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string dbCommand, List<SqlParameter> parameters)
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(dbCommand, connection);

                List<SqlParameter> parameters2 = new List<SqlParameter>();
                foreach (var item in parameters)
                {
                    parameters2.Add(new SqlParameter(item.ParameterName, item.Value));
                }

                command.Parameters.AddRange(parameters2.ToArray());

                connection.Open();
                SqlTransaction sqlTransaction = connection.BeginTransaction();
                command.Transaction = sqlTransaction;

                try
                {
                    int totalChange = command.ExecuteNonQuery();
                    sqlTransaction.Commit();

                    return totalChange;
                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();

                    throw;
                }
            }
        }
        /// <summary>
        /// 取得位在Web.config的資料庫連線字串
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            var manage = System.Configuration.ConfigurationManager.ConnectionStrings["systemDataBase"];

            if (manage == null)
                return string.Empty;
            else
                return manage.ConnectionString;
        }
    }
}
