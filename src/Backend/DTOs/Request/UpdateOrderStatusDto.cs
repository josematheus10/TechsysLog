using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Request;

public class UpdateOrderStatusDto
{
    [Required(ErrorMessage = "O status é obrigatório")]
    [RegularExpression("^(novo|entregue)$", ErrorMessage = "Status inválido. Use 'novo' ou 'entregue'")]
    public string Status { get; set; } = string.Empty;
}
