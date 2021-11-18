namespace P03_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Color
    {
        public int ColorId { get; set; }

        [Required]
        public string Name { get; set; }

        // collection teams?
    }
}

//•	Color – ColorId, Name
