using Microsoft.AspNetCore.SignalR;

namespace WebUı.Hubs
{
    public class HouseHub : Hub
    {
        public async Task SendNewHouseNotification(string message, int houseId)
        {
            await Clients.All.SendAsync("ReceiveNewHouseNotification", message, houseId);
        }
    }
} 