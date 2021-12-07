using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportProjetionDto
    {
        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Name { get; set; }

        [Range(1, 10)]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Director { get; set; }

        public IEnumerable<TicketImportDto> Tickets { get; set; }
    }

    public class TicketImportDto
    {
        [Required]
        [Range(1.00, 100.00)]
        public decimal Price { get; set; }

        [Range(1, 10)]
        public sbyte RowNumber { get; set; }

        public int PlayId { get; set; }
    }
}

//•	Id – integer, Primary Key
//•	Name – text with length [4, 30] (required)
//•	NumberOfHalls – sbyte between[1…10] (required)
//•	Director – text with length [4, 30] (required)
//•	Tickets - a collection of type Ticket

//•	Id – integer, Primary Key
//•	Price – decimal in the range [1.00….100.00] (required)
//•	RowNumber – sbyte in range[1….10](required)
//•	PlayId – integer, foreign key(required)
//•	TheatreId – integer, foreign key(required)

//{
//    "Name": "",
//    "NumberOfHalls": 7,
//    "Director": "Ulwin Mabosty",
//    "Tickets": [
//      {
//        "Price": 7.63,
//        "RowNumber": 5,
//        "PlayId": 4
//      },
//      {
//        "Price": 47.96,
//        "RowNumber": 9,
//        "PlayId": 3
//      }
//    ]
//  }

