namespace MusicHub.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class SongPerformer
    {
        public int SongId { get; set; }

        public Song Song { get; set; }

        public int PerformerId { get; set; }

        public Performer Performer { get; set; }

    }
}

//•	SongId – Integer, Primary Key
//•	Song – the performer’s Song (required)
//•	PerformerId – integer, Primary Key
//•	Performer – the song’s Performer (required)
