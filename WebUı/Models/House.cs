using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUı.Models
{
    public class House
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat alanı zorunludur")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Adres alanı zorunludur")]
        public string Address { get; set; }

        // Resim yolu
        public string? ImagePath { get; set; }

        // Oda sayısı
        public int RoomCount { get; set; }

        // Metrekare
        public int SquareMeters { get; set; }

        // Hızlı Satış
        public bool IsQuickSale { get; set; }

        // Aktif/Pasif durumu
        public bool IsActive { get; set; } = true;

        // Oluşturulma tarihi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // İl ilişkisi
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }

        // İlçe ilişkisi
        public int DistrictId { get; set; }
        [ForeignKey("DistrictId")]
        public District District { get; set; }

        // Oluşturan kullanıcı
        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public AppUser CreatedBy { get; set; }
    }
} 