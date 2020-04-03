using System;
using System.ComponentModel.DataAnnotations;

namespace FightCorona.DataCollector.Data.Models
{
    public class DistrictStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int Confirmed { get; set; }

        [Required]
        public int Active { get; set; }

        [Required]
        public int Recovered { get; set; }

        [Required]
        public int Deaths { get; set; }
    }
}
