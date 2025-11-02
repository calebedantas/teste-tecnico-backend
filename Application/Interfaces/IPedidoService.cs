using Application.DTOs;

namespace Application.Interface
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDto>> GetAllAsync();
        Task<PedidoDto?> GetByIdAsync(int id);
        Task<PedidoDto> CreateAsync(CriarPedidoDto cmd);
    }
}
