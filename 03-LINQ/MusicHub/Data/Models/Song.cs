namespace MusicHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Song
    {
        public Song()
        {
            this.SongPerformers = new HashSet<SongPerformer>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public int? AlbumId { get; set; }

        public Album Album { get; set; }

        public int WriterId { get; set; }

        public Writer Writer { get; set; }

        public decimal Price { get; set; }

        public ICollection<SongPerformer> SongPerformers { get; set; }

    }
}


//•	Id – integer, Primary Key
//•	Name – text with max length 20 (required)
//•	Duration – timeSpan(required)
//•	CreatedOn – date(required)
//•	Genre ¬– genre enumeration with possible values: "Blues, Rap, PopMusic, Rock, Jazz"(required)
//•	AlbumId – integer, foreign key
//•	Album – the song’s album
//•	WriterId – integer, Foreign key (required)
//•	Writer – the song’s writer
//•	Price – decimal (required)
//•	SongPerformers – a collection of type SongPerformer
