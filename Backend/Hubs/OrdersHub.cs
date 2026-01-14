using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

[Authorize]
public class OrdersHub : Hub
{
    public async Task SendNewOrder(string orderNumber)
    {
        await Clients.All.SendAsync("new-order", orderNumber);
    }
}
