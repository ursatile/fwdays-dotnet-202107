using Microsoft.AspNetCore.Mvc;

namespace Demo.Website.Controllers.Api {
	[Route("api/[controller]")]
	[ApiController]

	public class DataController : ControllerBase {

		[HttpGet]
		public IActionResult Get() {
			return Ok("Hello World");
		}
	}
}
