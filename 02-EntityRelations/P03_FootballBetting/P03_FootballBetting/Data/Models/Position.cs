using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Position
    {
        public int PositionId { get; set; }

        [Required]
        public string Name { get; set; }
    
        // collection players?
    }
}
