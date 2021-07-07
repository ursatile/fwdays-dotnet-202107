using System;
using System.IO;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Autobarn.Messages;

namespace Autobarn.AuditLog {
	class Program {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        const string SUBSCRIBER_ID = "Autobarn.AuditLog";

		static void Main(string[] args) {
            var amqp = config.GetConnectionString("AutobarnAmqpConnectionString");
            var bus = RabbitHutch.CreateBus(amqp);
            bus.PubSub.Subscribe<VehicleAddedMessage>(SUBSCRIBER_ID, Handler);
            
			Console.WriteLine("Listening for VehicleAddedMessage messages");
			Console.ReadKey(false);
		}

        private static void Handler(VehicleAddedMessage message) {
            Console.WriteLine(message.Registration);
        }

        private static IConfigurationRoot ReadConfiguration() {
			var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
			return new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();
		}
	}
}
