using Backend.DTOs.Request;
using Backend.DTOs.Response;
using Backend.Hubs;
using Backend.Mappers;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrdersController> _logger;
    private readonly IHubContext<OrdersHub> _hubContext;

    public OrdersController(
        IOrderRepository orderRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<OrdersController> logger,
        IHubContext<OrdersHub> hubContext)
    {
        _orderRepository = orderRepository;
        _userManager = userManager;
        _logger = logger;
        _hubContext = hubContext;
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
            var existingOrder = await _orderRepository.GetByOrderNumberAsync(createOrderDto.OrderNumber);
            if (existingOrder != null)
            {
                return Conflict(new { message = "Já existe um pedido com este número" });
            }

            // Criar o pedido usando o mapper
            var order = createOrderDto.ToModel(userId, user.UserName);

            var createdOrder = await _orderRepository.CreateAsync(order);

            _logger.LogInformation("Pedido {OrderNumber} criado com sucesso pelo usuário {UserId}", 
                createdOrder.OrderNumber, userId);

            // Converter para DTO usando o mapper
            var response = createdOrder.ToDto();

            // Emitir evento para atualizar a lista de pedidos em tempo real
            await _hubContext.Clients.All.SendAsync("new-order", response);

            await _hubContext.Clients.All.SendAsync("new-order-notify", response.OrderNumber);

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
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            var response = order.ToDto();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido {OrderId}", id);
            return StatusCode(500, new { message = "Erro interno ao buscar pedido" });
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
            var orders = await _orderRepository.GetAllAsync();
            
            var response = orders.ToDto().ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os pedidos");
            return StatusCode(500, new { message = "Erro interno ao buscar pedidos" });
        }
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="updateStatusDto">Novo status do pedido</param>
    /// <returns>O pedido atualizado</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(
        string id, 
        [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Verificar se o pedido existe
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            // Atualizar o status
            var updated = await _orderRepository.UpdateStatusAsync(id, updateStatusDto.Status);
            if (!updated)
            {
                return StatusCode(500, new { message = "Erro ao atualizar status do pedido" });
            }

            // Buscar o pedido atualizado
            var updatedOrder = await _orderRepository.GetByIdAsync(id);
            if (updatedOrder == null)
            {
                return NotFound(new { message = "Pedido não encontrado após atualização" });
            }

            _logger.LogInformation(
                "Status do pedido {OrderId} atualizado para {Status}", 
                id, 
                updateStatusDto.Status);

            var response = updatedOrder.ToDto();

            // Emitir evento SignalR com o pedido completo
            await _hubContext.Clients.All.SendAsync("order-status-changed", response);

            // Emitir evento específico para atualização do dashboard
            if (updateStatusDto.Status == "entregue")
            {
                _logger.LogInformation("Emitindo evento delivered-order-notify para pedido {OrderNumber}", updatedOrder.OrderNumber);
                await _hubContext.Clients.All.SendAsync("delivered-order-notify", updatedOrder.OrderNumber);
            }


            // Emitir evento específico para atualização do dashboard
            if (updateStatusDto.Status == "novo")
            {
                _logger.LogInformation("Emitindo evento new-order-notify para pedido {OrderNumber}", response.OrderNumber);
                await _hubContext.Clients.All.SendAsync("new-order-notify", response.OrderNumber);
            }


            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
            return StatusCode(500, new { message = "Erro interno ao atualizar status do pedido" });
        }
    }
}
