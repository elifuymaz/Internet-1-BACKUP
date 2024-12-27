using Microsoft.EntityFrameworkCore;
using WebUı.Models;

namespace WebUı.Repositories
{
    public class DistrictRepository : GenericRepository<District>
    {
        private readonly AppDbContext _context;

        public DistrictRepository(AppDbContext context) : base(context, context.Districts)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetDistrictListWithDetails()
        {
            return await _context.Districts
                .Include(d => d.City)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    CityName = d.City.Name,
                    d.CityId
                }).ToListAsync();
        }
    }
} 