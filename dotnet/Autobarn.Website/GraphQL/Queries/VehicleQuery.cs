using System;
using System.Collections.Generic;
using System.Linq;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.Queries {
	public class VehicleQuery : ObjectGraphType {
		private readonly IAutobarnDatabase db;

		public VehicleQuery(IAutobarnDatabase db) {
			this.db = db;

			Field<ListGraphType<VehicleGraphType>>("Vehicles",
				"Return all the vehicles in the system",
				resolve: GetAllVehicles);

			Field<VehicleGraphType>("Vehicle", "Return a specific vehicle",
				new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {
					Name = "registration", Description = "The registration plate of the car"
				}),
			resolve: FindSpecificVehicle);

			Field<ListGraphType<VehicleGraphType>>("VehiclesByAge", "Return vehicles by age",
			new QueryArguments(
				new QueryArgument<NonNullGraphType<IntGraphType>> {
					Name = "age", Description = "How many years old are the cars"
				},
				new QueryArgument<NonNullGraphType<StringGraphType>> {
					Name = "which", Description = "'older' or 'newer'"
				}
			), resolve: FindVehiclesByAge);
		}

		private IEnumerable<Vehicle> FindVehiclesByAge(IResolveFieldContext<object> context) {
			var age = context.GetArgument<int>("age");
			var year = DateTime.Now.Year - age;

			var which = context.GetArgument<string>("which");
			switch (which) {
				case "older":
					return db.ListVehicles().Where(v => v.Year <= year);
				default:
					return db.ListVehicles().Where(v => v.Year >= year);
			}
		}

		private Vehicle FindSpecificVehicle(IResolveFieldContext<object> context) {
			var registration = context.GetArgument<string>("registration");
			return db.FindVehicle(registration);
		}

		private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context) {
			return db.ListVehicles();
		}
	}
}