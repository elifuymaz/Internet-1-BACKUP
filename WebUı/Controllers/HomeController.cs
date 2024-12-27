using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebUı.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebUı.Hubs;
using WebUı.ViewModels;

namespace WebUı.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<HouseHub> _hubContext;

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext context,
            INotyfService notyfService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<AppUser> userManager,
            IHubContext<HouseHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 6;
            var housesQuery = _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .Where(h => h.IsActive)
                .OrderByDescending(h => h.CreatedAt);

            var totalHouses = await housesQuery.CountAsync();
            var houses = await housesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new HouseListViewModel
            {
                Houses = houses,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalHouses / (double)pageSize),
                TotalHouses = totalHouses
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cities = new SelectList(await _context.Cities.ToListAsync(), "Id", "Name");
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] House house, IFormFile? image)
        {
            try 
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _notyfService.Warning("Sistemsel hata ile karşılaşıldı");
                    return RedirectToAction("Index");
                }

                // Resim yükleme işlemleri
                if (image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "house-images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

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

                // Normal üye için varsayılan değerler
                house.IsQuickSale = false;
                house.IsActive = false;
                house.CreatedById = userId;
                house.CreatedAt = DateTime.Now;
                
                await _context.Houses.AddAsync(house);
                await _context.SaveChangesAsync();

                // Test bildirimi gönder
                await _hubContext.Clients.All.SendAsync("ReceiveNewHouseNotification", 
                    $"Yeni bir ev ilanı eklendi: {house.Title}", house.Id);

                _notyfService.Success("Ev başarıyla eklendi. Yönetici onayından sonra yayınlanacaktır.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Ev eklenirken bir hata oluştu");
                return RedirectToAction("Index");
            }
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int id)
        {
            var house = await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .FirstOrDefaultAsync(h => h.Id == id && h.IsActive);

            if (house == null)
            {
                _notyfService.Error("İlan bulunamadı.");
                return RedirectToAction("Index");
            }

            return View(house);
        }
    }
}
