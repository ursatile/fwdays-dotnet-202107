using System;
using System.Threading.Tasks;
using EasyNetQ;
using Messages;

namespace Subscriber {
	class Program {
		const string AMQP = "amqps://uzvpuvak:2q_TbfXVV9q6o2FY_DLfgZCbXirqINpH@rattlesnake.rmq.cloudamqp.com/uzvpuvak";

		static async Task Main(string[] args) {
            var SUBSCRIBER = $"dylan@{Environment.MachineName}";
            var bus = RabbitHutch.CreateBus(AMQP);
            await bus.PubSub.SubscribeAsync<string>(SUBSCRIBER, s => Console.WriteLine(s));
            await bus.PubSub.SubscribeAsync<ExampleMessage>(SUBSCRIBER, HandleExampleMessage);
            Console.WriteLine("Subscribed to <string> messages.");
            Console.ReadKey(true);		
		}

        static void HandleExampleMessage(ExampleMessage message) {            
            Console.WriteLine(new String('-', 72));
            Console.WriteLine(message.MessageBody);
            Console.WriteLine($"Created at {message.MessageSent} on {message.MachineName}");
            Console.WriteLine($"This was message #{message.MessageNumber}");
        }
	}
}
