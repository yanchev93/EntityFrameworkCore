namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Town
    {
        public int TownId { get; set; }

        [Required]
        public string Name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}

//•	Town – TownId, Name, CountryId