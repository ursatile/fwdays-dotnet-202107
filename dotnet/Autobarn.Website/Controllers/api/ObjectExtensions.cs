// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Autobarn.Data.Entities;

namespace Autobarn.Website.Controllers.api {
	public static class ObjectExtensions {
		public static dynamic ToDynamic(this object value) {
            IDictionary<string, object> expando = new ExpandoObject();
            var properties = TypeDescriptor.GetProperties(value.GetType());
            foreach(PropertyDescriptor prop in properties) {
                if (Ignore(prop)) continue;
                expando.Add(prop.Name, prop.GetValue(value));
            }
            return (ExpandoObject)expando;
        }
        private static bool Ignore(PropertyDescriptor prop) {
            return prop.Attributes.OfType<Newtonsoft.Json.JsonIgnoreAttribute>().Any();
        }

        public static dynamic ToResource(this Vehicle vehicle) {
            var resource = vehicle.ToDynamic();
            ((IDictionary<string,object>) resource).Remove("VehicleModel");
            resource._links = new {
                self = new {
                    href = $"/api/vehicles/{vehicle.Registration}"
                },
                vehicleModel = new {
                    href = $"/api/models/{vehicle.ModelCode}"
                }        
            };
            return resource;
        }
	}
}
