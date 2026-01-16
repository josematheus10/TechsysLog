using Backend.DTOs.Request;
using Backend.Models;

namespace Backend.Mappers;

public static class DeliveryAddressMapper
{
    public static DeliveryAddress ToModel(this DeliveryAddressDto dto)
    {
        return new DeliveryAddress
        {
            Cep = dto.Cep,
            Street = dto.Street,
            Number = dto.Number,
            Neighborhood = dto.Neighborhood,
            City = dto.City,
            State = dto.State.ToUpper()
        };
    }

    public static DeliveryAddressDto ToDto(this DeliveryAddress model)
    {
        return new DeliveryAddressDto
        {
            Cep = model.Cep,
            Street = model.Street,
            Number = model.Number,
            Neighborhood = model.Neighborhood,
            City = model.City,
            State = model.State
        };
    }
}
