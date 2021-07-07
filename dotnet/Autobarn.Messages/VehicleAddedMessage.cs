using System;

namespace Autobarn.Messages {
	public class VehicleAddedMessage {
		public string Registration { get; set; }
		public string ManufacturerName { get; set; }
		public string ModelName { get; set; }
		public string ModelCode { get; set; }
		public string ManufacturerCode { get; set; }
		public string Color { get; set; }
		public int Year { get; set; }
		public DateTimeOffset ListedAt { get; set; }
	}

	public class VehiclePriceCalculatedMessage : VehicleAddedMessage {
		public int Price { get; set; }
		public string CurrencyCode { get; set; }
	}

	public static class MessageExtensions {
		public static VehiclePriceCalculatedMessage ToVehiclePriceCalculatedMessage(this VehicleAddedMessage incomingMessage, int price,
			string currencyCode) {
			return new VehiclePriceCalculatedMessage {
				ManufacturerCode = incomingMessage.ManufacturerCode,
				ModelCode = incomingMessage.ModelCode,
				Color = incomingMessage.Color,
				ModelName = incomingMessage.ModelName,
				Registration = incomingMessage.Registration,
				Year = incomingMessage.Year,
				CurrencyCode = currencyCode,
				Price = price
			};
		}
	}
}
