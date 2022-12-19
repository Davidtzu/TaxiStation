using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaxiStation.Business_Logic;
using TaxiStation.DAL;
using TaxiStation.dtos;
using TaxiStation.Enums;

namespace TaxiStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private BusinessLogic _businessLogic;
        public BusinessLogic bs
        {
            get
            {
                if (_businessLogic == null)
                {
                    _businessLogic = new BusinessLogic();
                }
                return _businessLogic;
            }
        }

        [HttpPost("MessagesFromUser")]
        public async Task<IActionResult> MessagesFromUser(pusherData dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest();
                }

                return Ok(await bs.UserSubscribeAsPrudocer(dto.action, dto.id, cancellationToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromUser: " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("GetDriveHistoryByUser")]
        public async Task<IActionResult> GetDriveHistoryByUser(SearchHistoryByID user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest();
                }
                string pusherAppKey = Environment.GetEnvironmentVariable("pusherAppKey");
                var driveHistory = clsDal.GetDriveHistory(user.id, (int)userType.user);
                return Ok(new { driveHistory, pusherAppKey });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetDriveHistoryByUser " + ex.Message);
                return StatusCode(500);
            }
        }
        [HttpPost("GetDriveHistoryByTaxi")]
        public async Task<IActionResult> GetDriveHistoryByTaxi(SearchHistoryByID taxi)
        {
            try
            {
                if (taxi == null)
                {
                    return BadRequest();
                }
                string pusherAppKey = Environment.GetEnvironmentVariable("pusherAppKey");
                var driveHistory = clsDal.GetDriveHistory(taxi.id, (int)userType.taxi);
                return Ok(new { driveHistory, pusherAppKey });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetDriveHistoryByTaxi " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("TaxiSubscribeAsConsumer")]
        public async Task<ActionResult> TaxiSubscribeAsConsumer(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await bs.TaxiSubscribeAsConsumer(cancellationToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in TaxiSubscribeAsConsumer: " + ex.Message);
                return StatusCode(500);
            }
        }


        [HttpPost("GetDriveHistoryByStation")]
        public async Task<IActionResult> GetDriveHistoryByStation(SearchHistoryByID user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest();
                }
                var driveHistory = clsDal.GetDriveHistory(user.id, (int)userType.station);
                return Ok(new { driveHistory });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetDriveHistoryByStation " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("MessagesFromTaxi")]
        public async Task<IActionResult> MessagesFromTaxi(taxiPusherData dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest();
                }
                return Ok(await bs.PusherMessagesFromTaxi(dto, cancellationToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromTaxi: " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("InsertDrive")]
        public IActionResult InsertDrive(idData data)
        {
            if (string.IsNullOrEmpty(data.taxiID) || string.IsNullOrEmpty(data.userID))
            {
                return BadRequest();
            }
            try
            {
                bool isAdded = clsDal.InsertDrive(data.userID, data.taxiID);
                clsDal.FinishDrive(int.Parse(data.taxiID), int.Parse(data.userID));
                return Ok(isAdded);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in InsertDrive: " + ex.Message);
                return StatusCode(500);
            }

        }

    }
}