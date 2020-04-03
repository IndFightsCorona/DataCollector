using System;
using System.ComponentModel.DataAnnotations;

namespace FightCorona.DataCollector.Data.Models
{
    public class CountryStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int Confirmed { get; set; }
        
        [Required]
        public int Deaths { get; set; }

        [Required]
        public int Recovered { get; set; }

        [Required]
        public int Active { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
