using Confluent.Kafka;
using Kafka.Public.Loggers;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace TaxiStation.Classes
{

    public class kafkaConsumerHostedService : IHostedService
    {
        private ILogger<kafkaConsumerHostedService> _logger;
        private ClusterClient _cluster;
        public kafkaConsumerHostedService(ILogger<kafkaConsumerHostedService> logger)
        {
            _logger = logger;
            _cluster = new ClusterClient(new Configuration
            {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record =>
            {
                _logger.LogInformation($"Received: {Encoding.UTF8.GetString(record.Value as byte[])}");
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }


    public class KafkaProducerHostedService : IHostedService
    {
        private readonly ILogger<KafkaProducerHostedService> _logger;
        private IProducer<Null, string> _producer;

        public KafkaProducerHostedService(ILogger<KafkaProducerHostedService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 1; i++)
            {
                var value = $"hello world {i}";
                _logger.LogInformation(value);
                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = value
                }, cancellationToken);
            }
            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }




    public class DriveHistory
    {
        public string userFullName { get; set; }
        public string userEmail { get; set; }
        public string taxiNumber { get; set; }
        public string taxiDriverFullName { get; set; }
        public double cost { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
        //להוסיף איפה הורד ואיפה נאסף
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
        }
    }

    public class AvailableTaxis
    {
        public string taxiID { get; set; }
        public Point location { get; set; }

    }
    public class TaxiID
    {
        public string taxiID { get; set; }
    }
    public class SearchHistoryByID
    {
        public string ID { get; set; }
    }

    public class Taxi : User
    {
        public int taxiID { get; set; }
        public int taxiNumber { get; set; }
    }
    public class User
    {
        public int userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? email { get; set; }
        public int statusID { get; set; }
    }


}
