using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes {
	public class VehicleGraphType : ObjectGraphType<Vehicle> {
		public VehicleGraphType() {
			Name = "vehicle";
			Field(c => c.VehicleModel, nullable: false,
			type: typeof(ModelGraphType)).Description("The model of this vehicle");
			Field(c => c.Registration).Description("The registration plate of this vehicle");
			Field(c => c.Color).Description("What color is this vehicle?");
			Field(c => c.Year).Description("The year this vehicle was first registered");
		}
	}
}