namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.DataProcessor;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlPlaysDto = XmlConverter.Deserializer<ImportPlaysDto>(xmlString, "Plays");

            var validPlay = new List<Play>();

            foreach (var playsDto in xmlPlaysDto)
            {
                if (!IsValid(playsDto) || TimeSpan.Parse(playsDto.Duration).Hours < 1)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var play = new Play
                {
                    Title = playsDto.Title,
                    Duration = TimeSpan.Parse(playsDto.Duration),
                    Rating = playsDto.Rating,
                    Genre = Enum.Parse<Genre>(playsDto.Genre),
                    Description = playsDto.Description,
                    Screenwriter = playsDto.Screenwriter
                };

                validPlay.Add(play);

                sb.AppendLine($"Successfully imported {play.Title} with genre {play.Genre} and a rating of {play.Rating}!");
            }

            context.Plays.AddRange(validPlay);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlCastsDto = XmlConverter.Deserializer<ImportCastDto>(xmlString, "Casts");

            var validCasts = new List<Cast>();

            foreach (var castDto in xmlCastsDto)
            {
                if (!IsValid(castDto))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var cast = new Cast
                {
                    FullName = castDto.FullName,
                    IsMainCharacter = castDto.IsMainCharacter,
                    PhoneNumber = castDto.PhoneNumber,
                    PlayId = castDto.PlayId
                };

                validCasts.Add(cast);

                var isMain = cast.IsMainCharacter ? "main" : "lesser";
                sb.AppendLine($"Successfully imported actor {cast.FullName} as a {isMain} character!");
            }

            context.Casts.AddRange(validCasts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var jsonProjections = JsonConvert.DeserializeObject<IEnumerable<ImportProjetionDto>>(jsonString);

            var validProjections = new List<Theatre>();
            var allPlays = context.Plays.Select(x => x.Id).ToList();

            foreach (var currentProjection in jsonProjections)
            {
                if (!IsValid(currentProjection) ||
                    !currentProjection.Tickets.All(IsValid))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                foreach (var ticket in currentProjection.Tickets)
                {
                    if (!allPlays.Contains(ticket.PlayId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }
                }

                var projection = new Theatre
                {
                    Name = currentProjection.Name,
                    NumberOfHalls = currentProjection.NumberOfHalls,
                    Director = currentProjection.Director,
                    Tickets = currentProjection.Tickets.Select(t => new Ticket
                    {
                        Price = t.Price,
                        RowNumber = t.RowNumber,
                        PlayId = t.PlayId
                    })
                    .ToList()
                };

                validProjections.Add(projection);

                sb.AppendLine($"Successfully imported theatre {projection.Name} with #{projection.Tickets.Count} tickets!");
            }

            context.Theatres.AddRange(validProjections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
