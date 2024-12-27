using WebUı.Models;

namespace WebUı.ViewModels
{
    public class HouseListViewModel
    {
        public IEnumerable<House> Houses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalHouses { get; set; }
    }
}
