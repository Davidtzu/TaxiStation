using Confluent.Kafka;
using IT_TaxiStation;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiStation.Classes;
using TaxiStation.DAL;
using TaxiStation.dtos;


namespace TaxiStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private static IProducer<Null, string> _producer;
        private static ClusterClient _cluster;
        private static string _searchingUserID { get; set; }
        private static string _taxiLisining { get; set; }

        [HttpPost("MessagesFromUser")]
        public async Task<ActionResult> MessagesFromUser(MessageDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(_searchingUserID))
                {
                    _searchingUserID = dto.userID;
                    await UserSubscribeAsPrudocer((int)PusherAction.SearchTaxi);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromUser: " + ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("GetDriveHistory")]
        public async Task<IActionResult> GetDriveHistory(SearchHistoryByID user)
        {
            try
            {
                userType type;
                if (user.ID.Length == 1) //users
                {
                    type = userType.users;
                }
                else if (user.ID.Length == 2) //taxis
                {
                    type = userType.taxis;
                    TaxiSubscribeAsConsumer();
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
                Console.WriteLine("Error in GetDriveHistory " + ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("MessagesFromTaxi")]
        public async Task<ActionResult> MessagesFromTaxi(MessageDTO dto)
        {
            try
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
                if (dto.action == (int)PusherAction.GetDrive)
                {
                    if (_taxiLisining == null)
                    {
                        _taxiLisining = dto.taxiID;
                    }
                    clsDal.AddTaxiToPool(int.Parse(dto.taxiID));
                    Thread.Sleep(6000);
                    string taxiID = GetCloserTaxiAvailable();
                    int action = (int)PusherAction.foundTaxi;
                    if (taxiID == PusherAction.noneTaxiAvailable.ToString())
                    {
                        action = 4;
                    }
                    await pusher.TriggerAsync(
                      "chat",
                      "message",
                      new
                      {
                          userID = taxiID,
                          action = action
                      });
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromTaxi: " + ex.Message);
                return BadRequest();
            }

        }

        [HttpPost("InsertDrive")]
        public IActionResult InsertDrive(TaxiID taxi)
        {
            if (string.IsNullOrEmpty(_searchingUserID) || string.IsNullOrEmpty(taxi.taxiID))
            {
                return BadRequest();
            }
            try
            {
                bool isAdded = clsDal.InsertDrive(_searchingUserID, taxi.taxiID);
                Thread.Sleep(3000);
                clsDal.FinishDrive(int.Parse(taxi.taxiID), int.Parse(_searchingUserID));
                Thread.Sleep(2000);
                clsDal.ClearPoolOfTaxis();
                return Ok(isAdded);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in InsertDrive: " + ex.Message);
                return BadRequest();
            }

        }

        public void TaxiSubscribeAsConsumer()
        {
            try
            {
                MessageDTO dto = null;
                if (_cluster == null)
                {
                    _cluster = new ClusterClient(new Configuration
                    {
                        Seeds = "localhost:9092"
                    }, new ConsoleLogger());
                }
                _cluster.ConsumeFromLatest("demo");
                _cluster.MessageReceived += async record =>
                {
                    Console.WriteLine($"Received Value From Producer: {Encoding.UTF8.GetString(record.Value as byte[])}");
                    string value = Encoding.UTF8.GetString(record.Value as byte[]);
                    if (int.Parse(value) == (int)PusherAction.SearchTaxi)
                    {
                        dto = new MessageDTO()
                        {
                            userID = "",
                            taxiID = "",
                            action = (int)PusherAction.SearchTaxi
                        };
                        await MessagesFromConsumers(dto);
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TaxiSubscribeAsConsumer " + ex.Message);
            }
        }

        public async Task<ActionResult> MessagesFromConsumers(MessageDTO dto)
        {
            try
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
                      userID = dto.taxiID,
                      action = dto.action
                  });
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromConsumers: " + ex.Message);
                return BadRequest();
            }
        }
        public string GetCloserTaxiAvailable()
        {
            try
            {
                var taxis = clsDal.GetLisiningTaxis().Tables[0].AsEnumerable().Select(dr => new AvailableTaxis(dr)).ToList();
                if (taxis == null)
                {
                    return PusherAction.noneTaxiAvailable.ToString();
                }
                Random rnd = new Random();
                Point DriverLocation = new Point
                {
                    X = rnd.Next(1, 50),
                    Y = rnd.Next(1, 50)
                };
                double minimumDistance = double.MaxValue;
                string taxiID = string.Empty;

                foreach (var taxi in taxis)
                {
                    double getDistance = Helpers.GetDistance(DriverLocation, taxi.location);
                    if (getDistance < minimumDistance)
                    {
                        minimumDistance = getDistance;
                        taxiID = taxi.taxiID;
                    }
                }
                return taxiID;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetCloserTaxiAvailable " + ex.Message);
                return string.Empty;
            }

        }
        public async Task<bool> UserSubscribeAsPrudocer(int action)
        {
            try
            {
                CancellationToken cancellationToken = new CancellationToken();
                var config = new ProducerConfig()
                {
                    BootstrapServers = "localhost:9092"
                };
                _producer = new ProducerBuilder<Null, string>(config).Build();
                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = action.ToString()
                }, cancellationToken);
                _producer.Flush(TimeSpan.FromSeconds(10));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UserSubscribeAsPrudocer " + ex.Message);
                return false;
            }
        }
    }
}