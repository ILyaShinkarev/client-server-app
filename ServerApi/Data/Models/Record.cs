using System.ComponentModel.DataAnnotations;

namespace Server.Api.Data.Models
{
    public class Record
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        [MaxLength(10)]
        public string Message { get; set; } = string.Empty;

    }
}
