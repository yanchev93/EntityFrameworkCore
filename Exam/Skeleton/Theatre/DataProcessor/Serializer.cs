namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theaters = context.Theatres
                .ToList()
                .Where(x => numbersOfHalls <= x.NumberOfHalls && x.Tickets.Count() >= 20)
                .Select(x => new
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = x.Tickets
                                   .Where(t => 1 <= t.RowNumber && 5 >= t.RowNumber)
                                   .Sum(p => p.Price),
                    Tickets = x.Tickets.Select(t => new
                    {
                        Price = t.Price,
                        RowNumber = t.RowNumber
                    })
                    .OrderByDescending(p => p.Price)
                    .ToList()
                })
                .OrderByDescending(x => x.Halls)
                .ThenBy(x => x.Name)
                .ToList();

            var jsonTheater = JsonConvert.SerializeObject(theaters, Formatting.Indented);

            return jsonTheater;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            var plays = context.Plays
                .Where(x => rating >= x.Rating)
                .Select(x => new ExportPlayDto
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c"),
                    Rating = x.Rating != 0 ? x.Rating.ToString() : "Premier",
                    Genre = x.Genre.ToString(),
                    Actors = x.Casts
                        .Where(a => a.IsMainCharacter)
                        .Select(c => new ActorInputDto
                        {
                            FullName = c.FullName,
                            MainCharacter = $"Plays main character in '{x.Title}'."
                        })
                        .OrderByDescending(a => a.FullName)
                        .ToArray()

                })
                .OrderBy(x => x.Title)
                .ToList();

            var xml = XmlConverter.Serialize(plays, "Plays");

            return xml;
        }
    }
}
