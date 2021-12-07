using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Theatre.Data.Models;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlaysDto
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        [XmlElement("Title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        public string Duration { get; set; }

        [Range(0.00, 10.00)]
        [XmlElement("Rating")]
        public float Rating { get; set; }

        [EnumDataType(typeof(Genre))]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [Required]
        [MaxLength(700)]
        [XmlElement("Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        [XmlElement("Screenwriter")]
        public string Screenwriter { get; set; }
    }
}
//•	Id – integer, Primary Key
//•	Title – text with length [4, 50] (required)
//•	Duration – TimeSpan in format
//{ hours: minutes: seconds}, with a minimum length of 1 hour. (required)
//•	Rating – float in the range[0.00….10.00] (required)
//•	Genre – enumeration of type Genre, with possible values (Drama, Comedy, Romance, Musical) (required)
//•	Description – text with length up to 700 characters (required)
//•	Screenwriter – text with length [4, 30] (required)
//•	Casts - a collection of type Cast
//•	Tickets - a collection of type Ticket

//<Play>
//  <Title>The Hsdfoming</Title>
//  <Duration>03:40:00</Duration>
//  <Rating>8.2</Rating>
//  <Genre>Action</Genre>
//  <Description>A guyat Pinter turns into a debatable conundrum as oth ordinary and menacing.Muchof this has to do with the fabled "Pinter Pause," which simply mirrors the way we ofterespond to each other in conversation, tossing in remainders of thoughts on one subjectwellafter having moved on to another.</Description>
//  <Screenwriter>Roger Nciotti</Screenwriter>
//</Play>
