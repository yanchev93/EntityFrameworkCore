namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.DTO.Input;
    using CarDealer.DTO.Output;
    using CarDealer.Models;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var xmlSuppliers = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var xmlParts = File.ReadAllText("../../../Datasets/parts.xml");
            //var xmlCars = File.ReadAllText("../../../Datasets/cars.xml");
            //var xmlCustomers = File.ReadAllText("../../../Datasets/customers.xml");
            //var xmlSales = File.ReadAllText("../../../Datasets/sales.xml");

            //ImportSuppliers(context, xmlSuppliers);
            //ImportParts(context, xmlParts);
            //ImportCars(context, xmlCars);
            //ImportCustomers(context, xmlCustomers);
            //var result = ImportSales(context, xmlSales);

            var output = GetLocalSuppliers(context);
            Console.WriteLine(output);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            const string root = "suppliers";

            var supplierDto = context.Suppliers
                .Include(x => x.Parts)
                .Where(x => x.IsImporter == false)
                .Select(x => new SuppliersOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            var xmlSuppliers = XmlConverter.Serialize(supplierDto, root);

            return xmlSuppliers;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            const string root = "cars";

            var bmwCars = context.Cars
                .Where(x => x.Make.ToLower() == "bmw")
                .Select(x => new BmwOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            var xmlBwmCars = XmlConverter.Serialize(bmwCars, root);

            return xmlBwmCars;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute("cars"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces(); // remove the namespace in the xml "www.w3.org" etc.
            ns.Add("", ""); // remove the namespace in the xml "www.w3.org" etc.

            xmlSerializer.Serialize(textWriter, cars, ns); // serializing the xml

            var result = textWriter.ToString();

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Sales";
            var carsId = context.Cars.Select(x => x.Id).ToList();

            var salesDto = XmlConverter.Deserializer<SaleInputModel>(inputXml, rootElement);
            var sales = salesDto
                .Where(x => carsId.Contains(x.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                }).ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Customers";

            var customersDto = XmlConverter.Deserializer<CustomerInputModel>(inputXml, rootElement);
            var customers = customersDto.Select(x => new Customer
            {
                Name = x.Name,
                BirthDate = x.BirthDate,
                IsYoungDriver = x.IsYoungDriver
            }).ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Cars";

            var cars = new List<Car>();
            var carsDto = XmlConverter.Deserializer<CarInputModel>(inputXml, rootElement);

            var allParts = context.Parts.Select(x => x.Id).ToArray();

            foreach (var currentCar in carsDto)
            {
                var distinctedParts = currentCar.CarPartsInputModel.Select(x => x.Id).Distinct();
                var parts = distinctedParts.Intersect(allParts);

                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TraveledDistance,
                };

                foreach (var part in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = part
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }


            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}"; ;
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(PartsInputModel[]), new XmlRootAttribute("Parts"));
            var textReader = new StringReader(inputXml);
            var partsDto = xmlSerializer.Deserialize(textReader) as PartsInputModel[];

            var suppliers = context.Suppliers.Select(x => x.Id).ToArray();

            var parts = partsDto
                .Where(x => suppliers.Contains(x.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));
            var textReader = new StringReader(inputXml);
            var suppliersDto = xmlSerializer.Deserialize(textReader) as SupplierInputModel[];

            var suppliers = suppliersDto.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter
            })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

    }
}