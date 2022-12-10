using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IT_TaxiStation
{
    public static class Helpers
    {
        public static IHttpContextAccessor _accessor;
        public static IConfiguration _configuration;

        #region Constants
        internal const string c_tablename = "tablename";
        #endregion

        public static void SetInitElements(IHttpContextAccessor accesor, IConfiguration configuration)
        {
            _configuration = configuration;
            _accessor = accesor;
        }

        public static string getUserId()
        {
            string userName = "";
            return userName;
        }

        public static Guid getAppId()
        {
            var ret = Guid.Parse(Environment.GetEnvironmentVariable("AppId"));
            return ret;
        }

        public static double GetDistance(Point p1, Point p2) 
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }
        public static string GetUserGroupsKey()
        {
            try
            {
                string UserGroupsKey = Helpers.getUserId() + "_" + _configuration.GetValue<string>("appName");
                return UserGroupsKey;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static string getConnectionString()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = "LAPTOP-E4MIDL72";
            connectionStringBuilder.InitialCatalog = "TaxiStation";
            connectionStringBuilder.IntegratedSecurity = true;
            return connectionStringBuilder.ConnectionString;
        }
    }
}