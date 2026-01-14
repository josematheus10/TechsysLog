using Backend.Data;
using Backend.Models;
using Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderStore _orderStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderStore orderStore,
        UserManager<ApplicationUser> userManager,
        ILogger<OrdersController> logger)
    {
        _orderStore = orderStore;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    /// <param name="createOrderDto">Dados do pedido a ser criado</param>
    /// <returns>O pedido criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Obter o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            // Obter o usuário para pegar o nome
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Usuário não encontrado");
            }

            // Verificar se já existe um pedido com o mesmo número
            var existingOrder = await _orderStore.GetByOrderNumberAsync(createOrderDto.OrderNumber);
            if (existingOrder != null)
            {
                return Conflict(new { message = "Já existe um pedido com este número" });
            }

            // Criar o pedido
            var order = new Order
            {
                OrderNumber = createOrderDto.OrderNumber,
                Description = createOrderDto.Description,
                Value = createOrderDto.Value,
                DeliveryAddress = new DeliveryAddress
                {
                    Cep = createOrderDto.DeliveryAddress.Cep,
                    Street = createOrderDto.DeliveryAddress.Street,
                    Number = createOrderDto.DeliveryAddress.Number,
                    Neighborhood = createOrderDto.DeliveryAddress.Neighborhood,
                    City = createOrderDto.DeliveryAddress.City,
                    State = createOrderDto.DeliveryAddress.State.ToUpper()
                },
                UserId = userId,
                UserName = user.UserName
            };

            var createdOrder = await _orderStore.CreateAsync(order);

            _logger.LogInformation("Pedido {OrderNumber} criado com sucesso pelo usuário {UserId}", 
                createdOrder.OrderNumber, userId);

            var response = new OrderResponseDto
            {
                Id = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                Description = createdOrder.Description,
                Value = createdOrder.Value,
                DeliveryAddress = new DeliveryAddressDto
                {
                    Cep = createdOrder.DeliveryAddress.Cep,
                    Street = createdOrder.DeliveryAddress.Street,
                    Number = createdOrder.DeliveryAddress.Number,
                    Neighborhood = createdOrder.DeliveryAddress.Neighborhood,
                    City = createdOrder.DeliveryAddress.City,
                    State = createdOrder.DeliveryAddress.State
                },
                Status = createdOrder.Status,
                UserId = createdOrder.UserId,
                UserName = createdOrder.UserName,
                CreatedAt = createdOrder.CreatedAt,
                UpdatedAt = createdOrder.UpdatedAt
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, new { message = "Erro interno ao criar pedido" });
        }
    }

    /// <summary>
    /// Obtém um pedido por ID
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>O pedido encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(string id)
    {
        try
        {
            var order = await _orderStore.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Description = order.Description,
                Value = order.Value,
                DeliveryAddress = new DeliveryAddressDto
                {
                    Cep = order.DeliveryAddress.Cep,
                    Street = order.DeliveryAddress.Street,
                    Number = order.DeliveryAddress.Number,
                    Neighborhood = order.DeliveryAddress.Neighborhood,
                    City = order.DeliveryAddress.City,
                    State = order.DeliveryAddress.State
                },
                Status = order.Status,
                UserId = order.UserId,
                UserName = order.UserName,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido {OrderId}", id);
            return StatusCode(500, new { message = "Erro interno ao buscar pedido" });
        }
    }

    /// <summary>
    /// Obtém todos os pedidos do usuário autenticado
    /// </summary>
    /// <returns>Lista de pedidos do usuário</returns>
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(List<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            var orders = await _orderStore.GetByUserIdAsync(userId);
            
            var response = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Description = order.Description,
                Value = order.Value,
                DeliveryAddress = new DeliveryAddressDto
                {
                    Cep = order.DeliveryAddress.Cep,
                    Street = order.DeliveryAddress.Street,
                    Number = order.DeliveryAddress.Number,
                    Neighborhood = order.DeliveryAddress.Neighborhood,
                    City = order.DeliveryAddress.City,
                    State = order.DeliveryAddress.State
                },
                Status = order.Status,
                UserId = order.UserId,
                UserName = order.UserName,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedidos do usuário");
            return StatusCode(500, new { message = "Erro interno ao buscar pedidos" });
        }
    }

    /// <summary>
    /// Obtém todos os pedidos (requer autenticação)
    /// </summary>
    /// <returns>Lista de todos os pedidos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAllOrders()
    {
        try
        {
            var orders = await _orderStore.GetAllAsync();
            
            var response = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Description = order.Description,
                Value = order.Value,
                DeliveryAddress = new DeliveryAddressDto
                {
                    Cep = order.DeliveryAddress.Cep,
                    Street = order.DeliveryAddress.Street,
                    Number = order.DeliveryAddress.Number,
                    Neighborhood = order.DeliveryAddress.Neighborhood,
                    City = order.DeliveryAddress.City,
                    State = order.DeliveryAddress.State
                },
                Status = order.Status,
                UserId = order.UserId,
                UserName = order.UserName,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os pedidos");
            return StatusCode(500, new { message = "Erro interno ao buscar pedidos" });
        }
    }
}
