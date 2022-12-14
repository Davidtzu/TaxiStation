using System;
using System.Data;
using System.Drawing;

namespace TaxiStation.Classes
{
    public class DriveHistory
    {
        public string userFullName { get; set; }
        public string userEmail { get; set; }
        public string taxiNumber { get; set; }
        public string taxiDriverFullName { get; set; }
        public double cost { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
        public string pickUp { get; set; }
        public string takenDown { get; set; }
        public DriveHistory() { }
        public DriveHistory(DataRow dr)
        {
            userFullName = dr.Field<string>("UserFullName");
            userEmail = dr.Field<string>("UserEmail");
            taxiNumber = dr.Field<string>("TaxiNumber");
            taxiDriverFullName = dr.Field<string>("TaxiDriverFullName");
            startDate = dr.Field<DateTime>("StartDate");
            finishDate = dr.Field<DateTime>("FinishDate");
            cost = dr.Field<double>("Cost");
            pickUp = dr.Field<string>("PickUp");
            takenDown = dr.Field<string>("TakenDown");
        }
    }

    public class AvailableTaxis
    {
        public string taxiID { get; set; }
        public Point location { get; set; }
        public AvailableTaxis()
        {

        }
        public AvailableTaxis(DataRow dr)
        {
            taxiID = dr.Field<int>("TaxiID").ToString();
            location = new Point
            {
                X = GetRandomNumber(),
                Y = GetRandomNumber()
            };
        }
        public int GetRandomNumber()
        {
            Random rnd = new Random();
            return rnd.Next(1, 50);
        }
    }
    public class TaxiID
    {
        public string taxiID { get; set; }
    }
    public class SearchHistoryByID
    {
        public string ID { get; set; }
    }
}