using Microsoft.EntityFrameworkCore;
using WebUı.Models;

namespace WebUı.Repositories
{
    public class CityRepository : GenericRepository<City>
    {
        private readonly AppDbContext _context;

        public CityRepository(AppDbContext context) : base(context, context.Cities)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetCityListWithDetails()
        {
            return await _context.Cities
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    DistrictCount = c.Districts.Count
                }).ToListAsync();
        }

        public async Task<IEnumerable<object>> GetCitiesForDropdown()
        {
            return await _context.Cities
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();
        }
    }
} 