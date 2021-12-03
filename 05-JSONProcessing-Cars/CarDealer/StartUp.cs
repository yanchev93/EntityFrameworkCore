using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var carContext = new CarDealerContext();
            //carContext.Database.EnsureDeleted();
            //carContext.Database.EnsureCreated();

            //ImportSuppliers(carContext, File.ReadAllText("../../../Datasets/suppliers.json"));
            //ImportParts(carContext, File.ReadAllText("../../../Datasets/parts.json"));
            //ImportCars(carContext, File.ReadAllText("../../../Datasets/cars.json"));
            //ImportCustomers(carContext, File.ReadAllText("../../../Datasets/customers.json"));

            //var result = ImportSales(carContext, File.ReadAllText("../../../Datasets/sales.json"));

            var output = GetSalesWithAppliedDiscount(carContext);

            Console.WriteLine(output);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            
            var cars = context.Sales
                .Take(10)
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Car.Make,
                        Model = c.Car.Model,
                        TravelledDistance = c.Car.TravelledDistance
                    },
                    customerName = c.Customer.Name,
                    Discount = c.Discount.ToString("F2"),
                    price = c.Car.Sales.Sum(x => x.Car.PartCars.Sum(y => y.Part.Price)).ToString("F2"),
                    priceWithDiscount =
                    (c.Car.Sales.Sum(x => x.Car.PartCars.Sum(y => y.Part.Price)) - c.Car.Sales.Sum(x => x.Car.PartCars.Sum(y => y.Part.Price)) * c.Discount / 100).ToString("F2")
                })
                .ToArray();

            var jsonCar = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return jsonCar;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Include(x => x.Sales)
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales
                    .Sum(x => x.Car.PartCars
                               .Sum(p => p.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToArray();

            var jsonCustomers = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return jsonCustomers;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance,
                    },
                    parts = c.PartCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    })
                })
                .ToArray();

            var jsonCarWithParts = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);

            return jsonCarWithParts;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var jsonSuppliers = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return jsonSuppliers;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsToyota = context.Cars
                .Where(x => x.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var jsonCars = JsonConvert.SerializeObject(carsToyota, Formatting.Indented);

            return jsonCars;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            var jsonCustomers = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return jsonCustomers;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSales = JsonConvert.DeserializeObject<IEnumerable<SalesInputModel>>(inputJson);

            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
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