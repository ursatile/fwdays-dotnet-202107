using System;
using System.Threading.Tasks;
using EasyNetQ;

namespace Publisher {
	class Program {
        const string AMQP = "amqps://uzvpuvak:2q_TbfXVV9q6o2FY_DLfgZCbXirqINpH@rattlesnake.rmq.cloudamqp.com/uzvpuvak";
		static async Task Main(string[] args) {
			var bus = RabbitHutch.CreateBus(AMQP);
            var c = 0;
            while(true) {
                await bus.PubSub.PublishAsync<string>($"This is a message {c}");
                Console.WriteLine($"Published message {c}");
                c++;
                Console.ReadKey(true);
            }            
		}
	}
}
