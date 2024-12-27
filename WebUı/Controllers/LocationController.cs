using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebUı.Models;
using WebUı.Repositories;

namespace WebUı.Controllers
{
    public class LocationController : Controller
    {
        private readonly CityRepository _cityRepository;
        private readonly DistrictRepository _districtRepository;
        private readonly INotyfService _notyfService;

        public LocationController(AppDbContext context, INotyfService notyfService)
        {
            _cityRepository = new CityRepository(context);
            _districtRepository = new DistrictRepository(context);
            _notyfService = notyfService;
        }

        public IActionResult Cities() => View();

        [HttpGet]
        public async Task<IActionResult> GetCityList()
        {
            var cities = await _cityRepository.GetCityListWithDetails();
            return Json(new { data = cities });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] City city)
        {
            try
            {
                await _cityRepository.AddAsync(city);
                return Json(new { success = true, message = "İl Eklendi." });
            }
            catch
            {
                return Json(new { success = false, message = "İl eklenirken bir hata oluştu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCity(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                return Json(new { success = false, message = "İl bulunamadı." });

            return Json(new { success = true, data = city });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCity([FromBody] City city)
        {
            try
            {
                await _cityRepository.UpdateAsync(city);
                return Json(new { success = true, message = "İl başarıyla güncellendi." });
            }
            catch
            {
                return Json(new { success = false, message = "İl güncellenirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                await _cityRepository.DeleteAsync(id);
                return Json(new { success = true, message = "İl başarıyla silindi." });
            }
            catch
            {
                return Json(new { success = false, message = "İl bulunamadı." });
            }
        }

        public IActionResult Districts() => View();

        [HttpGet]
        public async Task<IActionResult> GetDistrictList()
        {
            var districts = await _districtRepository.GetDistrictListWithDetails();
            return Json(new { data = districts });
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrict(int id)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null)
                return Json(new { success = false, message = "İlçe bulunamadı." });

            return Json(new { success = true, data = district });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] District district)
        {
            try
            {
                await _districtRepository.AddAsync(district);
                return Json(new { success = true, message = "İlçe başarıyla eklendi." });
            }
            catch
            {
                return Json(new { success = false, message = "İlçe eklenirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDistrict([FromBody] District district)
        {
            try
            {
                await _districtRepository.UpdateAsync(district);
                return Json(new { success = true, message = "İlçe başarıyla güncellendi." });
            }
            catch
            {
                return Json(new { success = false, message = "İlçe güncellenirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            try
            {
                await _districtRepository.DeleteAsync(id);
                return Json(new { success = true, message = "İlçe başarıyla silindi." });
            }
            catch
            {
                return Json(new { success = false, message = "İlçe bulunamadı." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesForDropdown()
        {
            var cities = await _cityRepository.GetCitiesForDropdown();
            return Json(cities);
        }
    }
} 