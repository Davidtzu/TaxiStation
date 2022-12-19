using Confluent.Kafka;
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
using TaxiStation.DAL;
using TaxiStation.Enums;
using TaxiStation.dtos;
using TaxiStation.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;

namespace TaxiStation.Business_Logic
{
    public class BusinessLogic
    {
        private static IProducer<Null, string> _producer;
        private static ClusterClient _cluster;
        private static Pusher _pusher;
        private static int _groupsID = 0;
        public async Task<bool> PusherMessagesFromTaxi(taxiPusherData dto, CancellationToken cancellationToken)
        {
            try
            {
                bool result = false;
                if (_pusher == null)
                {
                    var options = new PusherOptions
                    {
                        Cluster = "us2",
                        Encrypted = true
                    };
                    _pusher = new Pusher(
                      Environment.GetEnvironmentVariable("pusherAppID"),
                      Environment.GetEnvironmentVariable("pusherAppKey"),
                      Environment.GetEnvironmentVariable("pusherAppSecret"),
                      options);
                }
                if (dto.action == (int)pusherAction.getDrive)
                {
                    int action;
                    clsDal.AddTaxiToPool(int.Parse(dto.taxiID), int.Parse(dto.userID));  // add to the list of taxis that accept the request.
                    Thread.Sleep(6000);                                  // wait for all taxis to register.
                    string taxiID = GetCloserTaxiAvailable(dto.userID);                  // after some time, the closess taxi is choosen 
                    if (string.IsNullOrEmpty(taxiID))
                    {
                        action = (int)pusherAction.noneTaxiAvailable;                    //in case none taxi has accept the request.
                    }
                    else
                    {
                        action = (int)pusherAction.foundTaxi;
                    }
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await _pusher.TriggerAsync(                                      // msg to the taxi who was choosen
                          "chat",
                          "message",
                          new
                          {
                              taxiID = taxiID,
                              userID = dto.userID,
                              action = action
                          });
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in PusherMessagesFromTaxi: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> TaxiSubscribeAsConsumer(CancellationToken cancellationToken)
        {
            try
            {
                if (_cluster == null)
                {
                    _cluster = new ClusterClient(new Configuration
                    {
                        Seeds = Environment.GetEnvironmentVariable("server")
                        
                    }, new ConsoleLogger());
                }
                _cluster.ConsumeFromLatest("drives");
                _cluster.MessageReceived += async record =>
                {
                    Console.WriteLine($"Received Value From Producer: {Encoding.UTF8.GetString(record.Value as byte[])}");
                    string[] value = Encoding.UTF8.GetString(record.Value as byte[]).Split(',');
                    if (int.Parse(value[0]) == (int)pusherAction.searchTaxi)
                    {
                        pusherData dto = new pusherData()
                        {
                            id = value[1],  //userID
                            action = (int)pusherAction.searchTaxi
                        };
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await MessagesFromConsumers(dto);
                        }
                    }
                };
                if (!cancellationToken.IsCancellationRequested)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TaxiSubscribeAsConsumer " + ex.Message);
                return false;
            }
        }
        public async Task<bool> MessagesFromConsumers(pusherData dto)
        {
            try
            {
                if (_pusher == null)
                {
                    var options = new PusherOptions
                    {
                        Cluster = "us2",
                        Encrypted = true
                    };
                    _pusher = new Pusher(
                      Environment.GetEnvironmentVariable("pusherAppID"),
                      Environment.GetEnvironmentVariable("pusherAppKey"),
                      Environment.GetEnvironmentVariable("pusherAppSecret"),
                      options);
                }
                await _pusher.TriggerAsync(
                  "chat",
                  "message",
                  new
                  {
                      id = dto.id,
                      action = dto.action
                  });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MessagesFromConsumers: " + ex.Message);
                return false;
            }
        }
        public string GetCloserTaxiAvailable(string userID)
        {
            try
            {
                var taxis = clsDal.GetLisiningTaxis(int.Parse(userID));
                if (taxis == null)
                {
                    return string.Empty;
                }
                Point DriverLocation = new Point
                {
                    X = Helpers.GetRandomNumber(),
                    Y = Helpers.GetRandomNumber()
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
        public async Task<bool> UserSubscribeAsPrudocer(int action, string userID, CancellationToken cancellationToken)
        {
            try
            {
                if (_producer == null)
                {
                    var config = new ProducerConfig()
                    {
                        BootstrapServers = Environment.GetEnvironmentVariable("server")
                    };
                    _producer = new ProducerBuilder<Null, string>(config).Build();
                }
                await _producer.ProduceAsync("drives", new Message<Null, string>()
                {
                    Value = action.ToString() + "," + userID
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