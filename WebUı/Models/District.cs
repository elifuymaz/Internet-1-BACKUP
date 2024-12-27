using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUı.Models
{
    public class District
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // İl ilişkisi
        public int CityId { get; set; }
        
        [ForeignKey("CityId")]
        public City City { get; set; }
    }
} 