using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDto>> GetAllAsync();
        Task<ProdutoDto?> GetByIdAsync(int id);
        Task<ProdutoDto> CreateAsync(ProdutoDto dto);
    }
}
