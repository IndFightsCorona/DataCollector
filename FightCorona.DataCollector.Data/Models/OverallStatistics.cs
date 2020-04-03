using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FightCorona.DataCollector.Data.Models
{
    public class OverallStatistics
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Passengers_screened_at_airport { get; set; }

        [Required]

        public int Active_COVID_2019_cases { get; set; }

        [Required]

        public int Cured_discharged_cases { get; set; }

        [Required]

        public int Death_cases { get; set; }

        [Required]
        public int Migrated_COVID19_Patient { get; set; }

        [Required]
        public DateTime LastUpdatedTime { get; set; }

        [Required]
        public int TotalCasesOverDays { get; set; }

        [Required]
        public int DeathsOverDays { get; set; }

        [Required]
        public int NewCasesOverDays { get; set; }

        
        [Required]
        public int Version { get; set; }


    }
}
