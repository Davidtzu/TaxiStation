using IT_TaxiStation;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace TaxiStation.DAL
{
    public class clsDal
    {


        public static DataSet GetInfraData()
        {
            DataSet res = new DataSet("InfraData");
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("InfraData", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    sa.Fill(res);
                    if(res == null || res.Tables.Count < 1)
                    {
                        throw new Exception("returned invalid result set from db");
                    }
                    res.Tables[0].TableName = Helpers.c_tablename;
                    res.Tables[1].TableName = Helpers.c_tablename;
                }
            }
            return res;
        }

        public static DataSet GetDriveHistory()
        {
            DataSet res = new DataSet("GetDriveHistory");
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("GetDriveHistory", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
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
        public static DataSet GetAvailableTaxis()
        {
            DataSet res = new DataSet("GetAvailableTaxis");
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("GetAvailableTaxis", conn))
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


        public static bool SaveUserApproveRequest_SaveUserRequest(int requestId)
        {
            var parameterOutName = 10;
            using (var conn = new SqlConnection(Helpers.getConnectionString()))
            {
                conn.Open();
                using (var command = new SqlCommand("StoredProcedureName", conn))
                {
                    SqlDataAdapter sa = new SqlDataAdapter();

                    sa.SelectCommand = command;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("requestId", requestId);

                    //if i want parameter out
                    command.Parameters.AddWithValue("parameterOutName", parameterOutName);
                    command.Parameters["parameterOutName"].Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    parameterOutName = Convert.ToInt32(command.Parameters["parameterOutName"].Value);
                }
                conn.Close();
            }
            return true;
        }
    }
}
