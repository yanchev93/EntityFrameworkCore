namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Team
    {
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string LogoUrl { get; set; }

        [Required]
        [Column(TypeName = "char(3)")]
        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; }

        public Color PrimaryKitColor { get; set; }

        public int SecondaryKitColorId { get; set; }

        public Color SecondaryKitColor { get; set; }

        public int TownId { get; set; }

        public Town Town { get; set; }

        public ICollection<Game> HomeGames { get; set; }

        public ICollection<Game> AwayGames { get; set; }
        // ???????
        public ICollection<Player> Players { get; set; }

        //collection players?

    }
}

//•	Team – TeamId, Name, LogoUrl, Initials (JUV, LIV, ARS…), Budget, PrimaryKitColorId, SecondaryKitColorId, TownId
