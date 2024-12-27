using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebUı.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebUı.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class HouseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;

        public HouseController(
            AppDbContext context,
            INotyfService notyfService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _notyfService = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.Cities = _context.Cities.ToList(); // İlleri ViewBag'e ekle
            return View(_context.Houses.Include(h => h.City).Include(h => h.District).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetHouseList()
        {
            var houses = await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .Select(h => new
                {
                    h.Id,
                    h.Title,
                    h.Price,
                    CityName = h.City.Name,
                    DistrictName = h.District.Name,
                    h.IsQuickSale,
                    h.IsActive,
                    CreatedBy = h.CreatedBy.UserName,
                    ImageUrl = string.IsNullOrEmpty(h.ImagePath) ? "/images/no-image.jpg" : h.ImagePath
                })
                .ToListAsync();

            return Json(new { data = houses });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cities = new SelectList(await _context.Cities.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] House house, IFormFile? image)
        {
            try 
            {
                // Mevcut kullanıcının ID'sini al
                var userId = _userManager.GetUserId(User);
               
                if (string.IsNullOrEmpty(userId))
                {
                    _notyfService.Warning("Sistemsel hata ile karşılaşıldı");
                    return RedirectToAction("Index", "House");
                }

                // Kullanıcının gerçekten var olduğunu kontrol et
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _notyfService.Warning("Sistemsel hata ile karşılaşıldı");
                    return RedirectToAction("Index", "House");
                }

                if (image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "house-images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    house.ImagePath = "/house-images/" + uniqueFileName;
                }

                house.CreatedById = userId;
                house.CreatedAt = DateTime.Now; // Oluşturulma tarihini de ekleyelim
                
                await _context.Houses.AddAsync(house);
                await _context.SaveChangesAsync();
                _notyfService.Success("Ev Başarıyla Eklendi");
                return RedirectToAction("Index", "House");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ev eklenirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHouse(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
                return Json(new { success = false, message = "Ev bulunamadı." });

            return Json(new { success = true, data = house });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var house = await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
                return NotFound();

            ViewBag.Cities = new SelectList(await _context.Cities.ToListAsync(), "Id", "Name", house.CityId);
            ViewBag.Districts = new SelectList(await _context.Districts
                .Where(d => d.CityId == house.CityId)
                .ToListAsync(), "Id", "Name", house.DistrictId);

            return View(house);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] House house, IFormFile? image)
        {
           
                var existingHouse = await _context.Houses.FindAsync(house.Id);
                if (existingHouse == null)
                    return Json(new { success = false, message = "Ev bulunamadı." });
            var userId = _userManager.GetUserId(User);


            if (image != null)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(existingHouse.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                            existingHouse.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // Yeni resim için klasör yolu tanımla
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "house-images");
                    
                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Yeni resmi kaydet
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    existingHouse.ImagePath = "/house-images/" + uniqueFileName;
                }

                existingHouse.Title = house.Title;
                existingHouse.Description = house.Description;
                existingHouse.Price = house.Price;
                existingHouse.Address = house.Address;
                existingHouse.RoomCount = house.RoomCount;
                existingHouse.SquareMeters = house.SquareMeters;
                existingHouse.IsQuickSale = house.IsQuickSale;
                existingHouse.IsActive = house.IsActive;
                existingHouse.CityId = house.CityId;
                existingHouse.DistrictId = house.DistrictId;
                existingHouse.CreatedById = userId;

                await _context.SaveChangesAsync();
                _notyfService.Success("Ev Başarıyla Güncellendi");
                return RedirectToAction("Index", "House");

        }

    
        public async Task<IActionResult> Delete(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                // Resmi sil
                if (!string.IsNullOrEmpty(house.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                        house.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Houses.Remove(house);
                await _context.SaveChangesAsync();
                _notyfService.Success("Ev Başarıyla Silindi");
                return RedirectToAction("Index", "House");
            }
            _notyfService.Success("Sistemsel Bir Hata Yaklandı");
            return RedirectToAction("Index", "House");
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            return Json(districts);
        }

        public IActionResult LoadMore(int page)
        {
            var houses = _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Skip(page * 5)
                .Take(5)
                .ToList();

            return PartialView("_HouseListPartial", houses);
        }

        public IActionResult FilterByCity(int id)
        {
            var houses = _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Where(h => h.CityId == id)
                .ToList();

            return PartialView("_HouseListPartial", houses);
        }
    }
} 