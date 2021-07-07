using System;
using GrpcServer;
using Grpc.Net.Client;

namespace GrpcClient {
	class Program {
		static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
			using var channel = GrpcChannel.ForAddress("https://localhost:5001");
			var greeter = new Greeter.GreeterClient(channel);
            var c = 0;
			while (true) {
				var request = new HelloRequest {
                     Name = $"fwdays {c++}",
                     Language = "ua-UA"
                };
				var reply = greeter.SayHello(request);
				Console.WriteLine(reply.Message);
			}
		}
	}
}
