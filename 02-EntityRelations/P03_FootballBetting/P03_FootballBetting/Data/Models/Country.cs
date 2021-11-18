using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Country
    {
        public int CountryId { get; set; }

        [Required]
        public string Name { get; set; }

        //collection ? towns
    }
}

//•	Country – CountryId, Name