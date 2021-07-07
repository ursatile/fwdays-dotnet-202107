using System;

namespace Autobarn.Messages {
	public class VehicleAddedMessage {
		public string Registration { get; set; }
		public string ManufacturerName { get; set; }
		public string ModelName { get; set; }
		public string ModelCode { get; set;}
		public string ManufacturerCode { get; set; }
		public string Color { get; set; }
		public int Year { get; set; }
		public DateTimeOffset ListedAt { get; set; }
	}
}
