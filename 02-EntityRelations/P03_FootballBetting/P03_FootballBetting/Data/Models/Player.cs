namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Player
    {
        public int PlayerId { get; set; }

        [Required]
        public string Name { get; set; }

        public int SquadNumber { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }


        public bool IsInjured { get; set; }

        // public ICollection<Game> Games { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
    }
}

//– PlayerId, Name, SquadNumber, TeamId, PositionId, IsInjured
