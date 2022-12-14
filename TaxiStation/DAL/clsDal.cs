using IT_TaxiStation;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace TaxiStation.DAL
{
    public class clsDal
    {
        public static void FinishDrive(int TaxiID, int UserID)
        {
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("FinishDrive", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("TaxiID", TaxiID);
                    command.Parameters.AddWithValue("UserID", UserID);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static void AddTaxiToPool(int taxiID)
        {
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("addTaxiToPool", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("taxiID", taxiID);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static void ClearPoolOfTaxis()
        {
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("clearPoolOfTaxis", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static DataSet GetLisiningTaxis()
        {
            DataSet res = new DataSet("GetLisiningTaxis");
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("GetLisiningTaxis", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    sa.Fill(res);
                    if (res == null || res.Tables.Count < 1)
                    {
                        throw new Exception("returned invalid result set from db");
                    }
                }
            }
            return res;
        }

        public static DataSet GetDriveHistory(string userID, int userType)
        {
            DataSet res = new DataSet("GetDriveHistory");
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("GetDriveHistory", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("userID", userID);
                    command.Parameters.AddWithValue("userType", userType);
                    sa.Fill(res);
                    if (res == null || res.Tables.Count < 1)
                    {
                        throw new Exception("returned invalid result set from db");
                    }
                    res.Tables[0].TableName = Helpers.c_tablename;
                    //res.Tables[1].TableName = Helpers.c_tablename;
                }
            }
            return res;
        }

        public static bool InsertDrive(string userID, string taxiID)
        {
            var isAdded = false;
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("InsertDrive", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();

                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("userID", userID);
                    command.Parameters.AddWithValue("taxiID", taxiID);
                    //if i want parameter out
                    command.Parameters.AddWithValue("isAdded", isAdded);
                    command.Parameters["isAdded"].Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    isAdded = Convert.ToBoolean(command.Parameters["isAdded"].Value);
                }
                conn.Close();
            }
            return isAdded;
        }
    }
}