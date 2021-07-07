using System;
using System.Threading.Tasks;
using EasyNetQ;

namespace Subscriber {
	class Program {
		const string AMQP = "amqps://uzvpuvak:2q_TbfXVV9q6o2FY_DLfgZCbXirqINpH@rattlesnake.rmq.cloudamqp.com/uzvpuvak";

		static async Task Main(string[] args) {
            var bus = RabbitHutch.CreateBus(AMQP);
            await bus.PubSub.SubscribeAsync<string>("fwdays_subscriber", s => Console.WriteLine(s));
            Console.WriteLine("Subscribed to <string> messages.");
            Console.ReadKey(true);		
		}
	}
}
