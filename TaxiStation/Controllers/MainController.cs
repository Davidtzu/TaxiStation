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

namespace TaxiStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        [HttpPost("Messages")]
        public async Task<ActionResult> Messages(MessageDTO dto)
        {
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

            await pusher.TriggerAsync(
              "chat",
              "message",
              new 
              {
                  userName = dto.userName,
                  message = dto.message
              });

            return Ok(new string[] { });
        }


        [HttpGet("SendMail")]
        public IActionResult SendMail()
        {
            return Ok("asd");
        }

        [HttpPost("SearchingTaxi")]
        public IActionResult SearchingTaxi(Point location)
        {
            double minimumDistance = 999;
            int taxiID = 0;
            Point taxiLocation = new Point();
            var taxis = clsDal.GetAvailableTaxis().Tables[0].AsEnumerable().Select(dr => new AvailableTaxis(dr)).ToList();
            foreach (AvailableTaxis taxi in taxis)
            {
                var tmp = taxi.location.Substring(6, taxi.location.Length - 2).Split(' ');
                taxiLocation.X = int.Parse(tmp[0]);
                taxiLocation.Y = int.Parse(tmp[1]);
                if(Helpers.GetDistance(location, taxiLocation) < minimumDistance)
                {
                    minimumDistance = Helpers.GetDistance(location, taxiLocation);
                    taxiID = taxi.taxiID;
                }
            }

            return Ok(taxiID);
        }


        [HttpGet("GetDriveHistory")]
        public IActionResult GetDriveHistory()
        {
            try
            {
                var ds = clsDal.GetDriveHistory();

                var DriveHistory = ds.Tables[Helpers.c_tablename]
                    .AsEnumerable().Select(dr => new DriveHistory(dr)).ToList();
                string test = "asd";
                //var Data1 = ds.Tables[Helpers.c_tablename]
                //    .AsEnumerable().Select(dr => new ClassName(dr)).ToList();

                return Ok(new { DriveHistory, test });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetInfraData " + ex.Message);
                throw;
            }
        }




    }
}
