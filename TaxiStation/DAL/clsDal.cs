using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TaxiStation.dtos;
using TaxiStation.Helper;

namespace TaxiStation.DAL
{
    public class clsDal
    {
        public static void FinishDrive(int taxiID, int userID)
        {
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("FinishDrive", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("TaxiID", taxiID);
                    command.Parameters.AddWithValue("UserID", userID);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static void AddTaxiToPool(int taxiID, int userID)
        {
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("addTaxiToPool", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("TaxiID", taxiID);
                    command.Parameters.AddWithValue("UserID", userID);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static List<AvailableTaxis> GetLisiningTaxis(int userID)
        {
            DataSet res = new DataSet("GetLisiningTaxis"); //lesanen lefi userID + taxiID
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("GetLisiningTaxis", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("UserID", userID);
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    sa.Fill(res);
                    if (res == null || res.Tables.Count < 1)
                    {
                        throw new Exception("returned invalid result set from db");
                    }
                }
            }
            return res.Tables[0].AsEnumerable().Select(dr => new AvailableTaxis(dr)).ToList();
        }

        public static List<DriveHistory> GetDriveHistory(string userID, int userType)
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
                    command.Parameters.AddWithValue("UserID", userID);
                    command.Parameters.AddWithValue("UserType", userType);
                    sa.Fill(res);
                    if (res == null || res.Tables.Count < 1)
                    {
                        throw new Exception("returned invalid result set from db");
                    }
                    res.Tables[0].TableName = Helpers.c_DriveHistoryTblName;
                }
            }
            return res.Tables[Helpers.c_DriveHistoryTblName]
                    .AsEnumerable().Select(dr => new DriveHistory(dr)).ToList();
        }

        public static bool InsertDrive(string userID, string taxiID)
        {
            var isAdded = false;
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("InsertDrive", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("UserID", userID);
                    command.Parameters.AddWithValue("TaxiID", taxiID);
                    //if i want parameter out
                    command.Parameters.AddWithValue("IsAdded", isAdded);
                    command.Parameters["IsAdded"].Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    isAdded = Convert.ToBoolean(command.Parameters["IsAdded"].Value);
                }
                conn.Close();
            }
            return isAdded;
        }
    }
}