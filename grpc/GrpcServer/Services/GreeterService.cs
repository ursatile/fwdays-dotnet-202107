using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer {
	public class GreeterService : Greeter.GreeterBase {
		private readonly ILogger<GreeterService> _logger;
		public GreeterService(ILogger<GreeterService> logger) {
			_logger = logger;
		}

		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
			string greeting;
			switch (request.Language) {
				case "ua-UA":
					greeting = $"Привіт, {request.Name}";
					break;
				default:
					greeting = $"Hello, {request.Name}";
					break;

			}
			return Task.FromResult(new HelloReply {
				Message = greeting
			});
		}
	}
}
