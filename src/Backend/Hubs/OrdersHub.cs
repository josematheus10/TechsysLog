using Backend.Models.DTOs;
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

    public async Task SendOrderStatusChanged(OrderResponseDto order)
    {
        await Clients.All.SendAsync("order-status-changed", order);
    }
}
