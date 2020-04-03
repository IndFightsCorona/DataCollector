using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FightCorona.DataCollector.Data.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Suggestions{ get; set; }
 

        [Required]
        public DateTime CreatedDate { get; set; }

       

    }
}
