using System;
using System.Threading.Tasks;
using EasyNetQ;
using Messages;

namespace Publisher {
	class Program {
		const string AMQP = "amqps://uzvpuvak:2q_TbfXVV9q6o2FY_DLfgZCbXirqINpH@rattlesnake.rmq.cloudamqp.com/uzvpuvak";
		static async Task Main(string[] args) {
			var bus = RabbitHutch.CreateBus(AMQP);
			var c = 0;
			while (true) {
				Console.WriteLine("Enter a message:");
				var message = Console.ReadLine();

				if (c % 2 == 0) {
					var em = new ExampleMessage {
						MachineName = Environment.MachineName,
						MessageBody = message,
						MessageSent = DateTimeOffset.UtcNow,
						MessageNumber = c
					};
					await bus.PubSub.PublishAsync(em);
				} else {
					await bus.PubSub.PublishAsync<string>($"Message {c}: {message}");
				}
				Console.WriteLine($"Published message {c}: {message}");
				c++;
				Console.ReadKey(true);


			}
		}
	}
}
