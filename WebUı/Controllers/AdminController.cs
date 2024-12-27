using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebUı.Models;


namespace WebUı.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyfService;

        public AdminController(AppDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHouseDetails(int id)
        {
            var house = await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
                return NotFound();

            return View("HouseDetail", house);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveHouse(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
                return NotFound();

            house.IsActive = true;
            await _context.SaveChangesAsync();
            _notyfService.Success("İlan başarıyla onaylandı!");
            
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> PendingHouses()
        {
            var pendingHouses = await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .Where(h => !h.IsActive)
                .ToListAsync();

            return View(pendingHouses);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
