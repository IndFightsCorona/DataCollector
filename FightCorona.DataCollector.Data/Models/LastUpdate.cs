using System.ComponentModel.DataAnnotations;

namespace FightCorona.DataCollector.Data.Models
{
    public class ReaderStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReaderName { get; set; }

        [Required]
        public string Version { get; set; }
    }
}
