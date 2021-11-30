using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var carContext = new CarDealerContext();
            carContext.Database.EnsureDeleted();
            carContext.Database.EnsureCreated();

            ImportSuppliers(carContext, File.ReadAllText("../../../Datasets/suppliers.json"));
            ImportParts(carContext, File.ReadAllText("../../../Datasets/parts.json"));
            ImportCars(carContext, File.ReadAllText("../../../Datasets/cars.json"));

            var result = ImportCustomers(carContext, File.ReadAllText("../../../Datasets/customers.json"));

            Console.WriteLine(result);
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsInput = JsonConvert.DeserializeObject<List<CarsInputModel>>(inputJson);

            List<Car> listOfcars = new List<Car>();
            foreach (var carJson in carsInput)
            {
                Car car = new Car()
                {
                    Make = carJson.Make,
                    Model = carJson.Model,
                    TravelledDistance = carJson.TravelledDistance
                };
                foreach (var partId in carJson.PartsId.Distinct())
                {
                    car.PartCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }
                listOfcars.Add(car);
            }

            context.Cars.AddRange(listOfcars);
            context.SaveChanges();
            return $"Successfully imported {listOfcars.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var suppliersID = context.Suppliers.Select(x => x.Id).ToArray();

            var dtoParts = JsonConvert.DeserializeObject<IEnumerable<PartsInputModel>>(inputJson)
                .Where(x => suppliersID.Contains(x.SupplierId));

            var parts = mapper.Map<IEnumerable<Part>>(dtoParts);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSuppliers = JsonConvert.DeserializeObject<IEnumerable<SuppliersInputModel>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}