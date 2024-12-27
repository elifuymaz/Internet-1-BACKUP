using System.ComponentModel.DataAnnotations;

namespace WebUı.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // İlçe ilişkisi
        public ICollection<District> Districts { get; set; }
    }
} 