using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Laundry.Models
{
    public class CuciSepatu
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public string NamaSepatu { get; set; }

        [Required]
        public string JenisSepatu { get; set; }

        [Required]
        public string JenisLayanan { get; set; } // Deep Clean, Fast Clean, dll

        [Required]
        public string TipeLayanan { get; set; } // Regular, Express

        [Required]
        public DateTime TanggalCuci { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalHarga { get; set; }

        public string Status { get; set; } = "PROSES";
    }
}
