using Backend.DTOs.Request;
using Backend.DTOs.Response;
using Backend.Models;

namespace Backend.Mappers;

public static class OrderMapper
{
    public static Order ToModel(this CreateOrderDto dto, string userId, string? userName)
    {
        return new Order
        {
            OrderNumber = dto.OrderNumber,
            Description = dto.Description,
            Value = dto.Value,
            DeliveryAddress = dto.DeliveryAddress.ToModel(),
            UserId = userId,
            UserName = userName
        };
    }

    public static OrderResponseDto ToDto(this Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Description = order.Description,
            Value = order.Value,
            DeliveryAddress = order.DeliveryAddress.ToDto(),
            Status = order.Status,
            UserId = order.UserId,
            UserName = order.UserName,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public static IEnumerable<OrderResponseDto> ToDto(this IEnumerable<Order> orders)
    {
        return orders.Select(order => order.ToDto());
    }
}
