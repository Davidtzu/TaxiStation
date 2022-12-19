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

namespace TaxiStation.Helper
{
    public static class Helpers
    {
        public static IHttpContextAccessor _accessor;
        public static IConfiguration _configuration;
        private static readonly Random random = new Random();

        #region Constants
        internal const string c_DriveHistoryTblName = "DriveHistory";
        #endregion
        public static void SetInitElements(IHttpContextAccessor accesor, IConfiguration configuration)
        {
            _configuration = configuration;
            _accessor = accesor;
        }
        public static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }
        public static int GetRandomNumber()
        {
            return random.Next(1, 50);
        }
        internal static string getConnectionString()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = Environment.GetEnvironmentVariable("connectionStringDataSource");
            connectionStringBuilder.InitialCatalog = Environment.GetEnvironmentVariable("connectionStringInitialCatalog");
            connectionStringBuilder.IntegratedSecurity = true;
            return connectionStringBuilder.ConnectionString;
        }
    }
}