using System;
using System.IO;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Autobarn.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Autobarn.AuditLog {
	class Program {
		private static readonly IConfigurationRoot config = ReadConfiguration();

		const string SUBSCRIBER_ID = "Autobarn.Notifier";

		static HubConnection hub;

		static async Task Main(string[] args) {
			JsonConvert.DefaultSettings = JsonSettings;

			const string SIGNALR_HUB_URL = "https://workshop.ursatile.com:5001/hub";

			var amqp = config.GetConnectionString("AutobarnAmqpConnectionString");
			var bus = RabbitHutch.CreateBus(amqp);
			bus.PubSub.Subscribe<VehiclePriceCalculatedMessage>(SUBSCRIBER_ID, Handler);

			hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
			await hub.StartAsync();

			Console.WriteLine("Listening for VehiclePriceCalculatedMessage messages");
			Console.ReadKey(false);
		}

		private static async Task Handler(VehiclePriceCalculatedMessage message) {
			try {
				var json = JsonConvert.SerializeObject(message);
			Console.WriteLine($"Sending JSON to hub: {json}");
			await hub.SendAsync("NotifyWebUsers", "Autobarn.Notifier", json);
			Console.WriteLine("Sent!");			
			} catch(Exception ex) { 
				Console.WriteLine(ex);
				throw;
			}
		}

		private static JsonSerializerSettings JsonSettings() =>
			new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				}
			};


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
