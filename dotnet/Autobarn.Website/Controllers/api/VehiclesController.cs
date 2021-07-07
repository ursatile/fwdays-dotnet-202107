using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Autobarn.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;
		private readonly IBus bus;

		public VehiclesController(IAutobarnDatabase db, IBus bus) {
			this.db = db;
			this.bus = bus;
		}

		private dynamic Paginate(string url, int initial) {
			dynamic links = new ExpandoObject();
			links.self = new { href = $"{url}?initial={(char)initial}" };
			links.final = new { href = $"{url}?initial=z" };
			links.first = new { href = $"{url}?initial=a" };
			if (initial > 'a') links.previous = new { href = $"{url}?initial={(char)(initial-1)}" };
			if (initial < 'z') links.next = new { href = $"{url}?initial={(char)(initial+1)}" };
			return links;
		}

		// GET: api/vehicles
		[HttpGet]
		[Produces("application/hal+json")] 
		public IActionResult Get(char initial = 'a') {
			var items = db.ListVehicles().Where(v => v.Registration.StartsWith(initial.ToString(), System.StringComparison.InvariantCultureIgnoreCase))
				.Select(vehicle => vehicle.ToResource());
			var total = db.CountVehicles();
			var _links = Paginate("/api/vehicles", initial);
			var result = new {
				_links,
				items
			};
			return Ok(result);
		}


		// GET api/vehicles/ABC123
		[HttpGet("{id}")]
		public IActionResult Get(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound();
			return Ok(vehicle);
		}

		// POST api/vehicles
		[HttpPost]
		public IActionResult Post([FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				VehicleModel = vehicleModel
			};
			db.CreateVehicle(vehicle);
			NotifyAboutNewVehicle(vehicle);
			return Ok(dto);
		}

		private void NotifyAboutNewVehicle(Vehicle vehicle) {
			var message = new VehicleAddedMessage {
				Registration = vehicle.Registration,
				Color = vehicle.Color,
				Year = vehicle.Year,
				ManufacturerName = vehicle?.VehicleModel?.Manufacturer?.Name,
				ModelName = vehicle?.VehicleModel?.Name,
				ListedAt = System.DateTimeOffset.UtcNow
			};
			bus.PubSub.Publish(message);			
		}

		// PUT api/vehicles/ABC123
		[HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				ModelCode = vehicleModel.Code
			};
			db.UpdateVehicle(vehicle);
			return Ok(dto);
		}

		// DELETE api/vehicles/ABC123
		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound();
			db.DeleteVehicle(vehicle);
			return NoContent();
		}
	}
}
