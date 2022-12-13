using TaxiStation;
using TaxiStation.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TaxiStation.Classes;
using IT_TaxiStation;
using System.Drawing;
using Microsoft.AspNetCore.Http.Connections;
using PusherServer;
using TaxiStation.dtos;
using System.Threading;

namespace TaxiStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private static string SearchingUserID { get; set; }
        private static List<string> lisiningTaxiList { get; set; }


        [HttpPost("Messages")]
        public async Task<ActionResult> Messages(MessageDTO dto)
        {
            if(!string.IsNullOrEmpty(dto.userID))
            {
                SearchingUserID = dto.userID;
            }
            var options = new PusherOptions
            {
                Cluster = "us2",
                Encrypted = true
            };
            var pusher = new Pusher(
              "1522722",
              "52a43643bf829d8624d0", 
              "1ac2b30992b6c8e36564",
              options);
            
            if (dto.action == 2) //GetDrive
            {
                if (lisiningTaxiList == null)
                {
                    lisiningTaxiList = new List<string>();
                }
                lisiningTaxiList.Add(dto.taxiID);
                Thread.Sleep(1000);
                string taxiID = GetCloserTaxiAvailable();
                int action = 3; //foundTaxi
                await pusher.TriggerAsync(
                  "chat",
                  "message",
                  new
                  {
                      userID = taxiID,
                      action = action
                  });
            }
            else
            {
                await pusher.TriggerAsync(
                  "chat",
                  "message",
                  new
                  {
                      userID = dto.taxiID,
                      action = dto.action
                  });
            }
            return Ok(value: 1);
        }

        [HttpPost("InsertDrive")]
        public IActionResult InsertDrive(TaxiID taxi)
        {
            if (string.IsNullOrEmpty(SearchingUserID) || string.IsNullOrEmpty(taxi.taxiID))
            {
                return BadRequest();
            }
            try
            {
                bool isAdded = clsDal.InsertDrive(SearchingUserID, taxi.taxiID);
                Thread.Sleep(3000);
                clsDal.FinishDrive(int.Parse(taxi.taxiID));
                Thread.Sleep(2000);
                lisiningTaxiList = null;
                return Ok(isAdded);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return BadRequest();
            }

        }

        public string GetCloserTaxiAvailable()
        {
            Random rnd = new Random();
            Point DriverLocation = new Point
            {
                X = rnd.Next(1, 50),
                Y = rnd.Next(1, 50)
            };
            double minimumDistance = double.MaxValue;
            string taxiID = string.Empty;

            var taxis = lisiningTaxiList;//clsDal.GetAvailableTaxis(lisiningTaxiList).Tables[0].AsEnumerable().Select(dr => new AvailableTaxis(dr)).ToList();
            foreach (string taxi in taxis)
            {
                Point TaxiLocation = new Point
                {
                    X = rnd.Next(1, 50),
                    Y = rnd.Next(1, 50)
                };
                double getDistance = Helpers.GetDistance(DriverLocation, TaxiLocation);
                if (getDistance < minimumDistance)
                {
                    minimumDistance = getDistance;
                    taxiID = taxi;
                }
            }
            return taxiID;
        }

        [HttpPost("GetDriveHistory")]
        public IActionResult GetDriveHistory(SearchHistoryByID user)
        {
            try
            {
                userType type;
                if (user.ID.Length == 1) //users
                {
                    type = userType.users;
                }
                else if(user.ID.Length == 2) //taxis
                {
                    type = userType.taxis;
                }
                else //taxiStation
                {
                    type = userType.taxiStation;
                }
                var ds = clsDal.GetDriveHistory(user.ID, (int)type);

                var DriveHistory = ds.Tables[Helpers.c_tablename]
                    .AsEnumerable().Select(dr => new DriveHistory(dr)).ToList();

                return Ok(new { DriveHistory });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetInfraData " + ex.Message);
                throw;
            }
        }
    }
}
