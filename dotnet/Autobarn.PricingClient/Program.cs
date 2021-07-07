using System;
using System.IO;
using Autobarn.PricingServer;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Autobarn.Messages;
using EasyNetQ;


namespace Autobarn.PricingClient {
	class Program {
		private static readonly IConfigurationRoot config = ReadConfiguration();

		const string SUBSCRIBER_ID = "Autobarn.PricingClient_DYLAN";

		static Pricer.PricerClient grpcClient;
		static IBus bus;

		static void Main(string[] args) {
			// Connect to gRPC
			using var channel = GrpcChannel.ForAddress(config["AutobarnPricingServerAddress"]);
			grpcClient = new Pricer.PricerClient(channel);

			// connect to EasyNetQ
			var amqp = config.GetConnectionString("AutobarnAmqpConnectionString");
			bus = RabbitHutch.CreateBus(amqp);
			bus.PubSub.Subscribe<VehicleAddedMessage>(SUBSCRIBER_ID, CalculateVehiclePrice);

			Console.WriteLine("Ready.");
			Console.ReadKey();
		}

		private static void CalculateVehiclePrice(VehicleAddedMessage message) {
			try {
				Console.WriteLine($"Calculating price...");
				var request = new PriceRequest {
					ModelCode = message.ModelCode,// ?? "UNKNOWN_MODEL" ,
					ManufacturerCode = message.ManufacturerCode, // ?? "UNKNOWN_MANUFACTURER",
					Year = message.Year,
					Color = message.Color
				};
				var reply = grpcClient.GetPrice(request);
				Console.WriteLine($"Calculated price! {reply.Price} {reply.CurrencyCode}");
				Console.WriteLine("Sending VehiclePriceCalculatedMessage...");
				var outgoingMessage = message.ToVehiclePriceCalculatedMessage(reply.Price, reply.CurrencyCode);				
				bus.PubSub.Publish(outgoingMessage);
				Console.WriteLine("Done!");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
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

