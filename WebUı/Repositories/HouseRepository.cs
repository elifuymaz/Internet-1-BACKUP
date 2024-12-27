using Microsoft.EntityFrameworkCore;
using WebUı.Models;

namespace WebUı.Repositories
{
    public class HouseRepository : GenericRepository<House>
    {
        private readonly AppDbContext _context;

        public HouseRepository(AppDbContext context) : base(context, context.Houses)
        {
            _context = context;
        }

        public async Task<IEnumerable<House>> GetAllHousesWithDetailsAsync()
        {
            return await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .Where(h => h.IsActive)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<House> GetHouseByIdWithDetailsAsync(int id)
        {
            return await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .FirstOrDefaultAsync(h => h.Id == id && h.IsActive);
        }

        public async Task<IEnumerable<House>> GetPaginatedHousesAsync(int page, int pageSize)
        {
            return await _context.Houses
                .Include(h => h.City)
                .Include(h => h.District)
                .Include(h => h.CreatedBy)
                .Where(h => h.IsActive)
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalHousesCountAsync()
        {
            return await _context.Houses.Where(h => h.IsActive).CountAsync();
        }
    }
} 