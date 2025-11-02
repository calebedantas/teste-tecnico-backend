using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;

        public PedidosController(IPedidoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var pedidos = await _service.GetAllAsync();

            if (!pedidos.Any())
                return Ok(new List<object>()); 

            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CriarPedidoDto cmd)
        {
            var novo = await _service.CreateAsync(cmd);
            return Ok(novo);
        }
    }
}
