using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Request;

public class CreateOrderDto
{
    [Required(ErrorMessage = "O número do pedido é obrigatório")]
    [StringLength(50, ErrorMessage = "O número do pedido deve ter no máximo 50 caracteres")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Value { get; set; }

    [Required(ErrorMessage = "O endereço de entrega é obrigatório")]
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
}

public class DeliveryAddressDto
{
    [Required(ErrorMessage = "O CEP é obrigatório")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido. Use o formato: 12345-678 ou 12345678")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "A rua é obrigatória")]
    [StringLength(200, ErrorMessage = "A rua deve ter no máximo 200 caracteres")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número é obrigatório")]
    [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
    public string Number { get; set; } = string.Empty;

    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
    public string Neighborhood { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter exatamente 2 caracteres")]
    public string State { get; set; } = string.Empty;
}
