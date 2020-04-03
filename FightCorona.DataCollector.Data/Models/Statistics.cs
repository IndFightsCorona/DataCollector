using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FightCorona.DataCollector.Data.Models
{
    public class Statistics
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SNo { get; set; }

        [Required]
        public string Name_of_State_UT { get; set; }

        [Required]
        [DefaultValue("DEF")]
        public string State_UT_Code { get; set; }

        [Required]
        public int Total_Confirmed_cases_Indian_National { get; set; }

        [Required]
        public int Total_Confirmed_cases_Foreign_National { get; set; }

        [Required]
        public int Cured_Discharged_Migrated { get; set; }

        [Required]
        public int Death { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int Version { get; set; }
    }
}
