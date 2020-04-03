using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FightCorona.DataCollector.Data.Models
{
    public class LastUpdate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

    }
}
