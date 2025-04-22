using Microsoft.AspNetCore.Mvc;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Services;

namespace TechGear.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService, IEmailService emailService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IEmailService _emailService = emailService;

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
        {
            var orderEmailDto = await _orderService.CreateOrderAsync(order);

            if (orderEmailDto == null)
            {
                return StatusCode(500, "Tạo đơn hàng thất bại.");
            }

            // Gửi email xác nhận (nếu cần)
            await _emailService.SendOrderConfirmationEmailAsync(orderEmailDto);

            return CreatedAtAction(nameof(GetOrderById), new { orderId = orderEmailDto.OrderId }, orderEmailDto);
        }


        [HttpPut("update/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDto order)
        {
            if (order.Id != orderId)
            {
                return BadRequest();
            }

            var result = await _orderService.UpdateOrderAsync(order);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
