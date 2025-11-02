using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProdutoService> _logger;

        public ProdutoService(AppDbContext context, IMapper mapper, ILogger<ProdutoService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
        {
            _logger.LogInformation("Iniciando busca de todos os produtos");

            try
            {
                var produtos = await _context.Produtos
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Foram encontrados {count} produtos.", produtos.Count);

                var resultado = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
                _logger.LogInformation("Busca de produtos concluída com sucesso.");

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos.");
                throw;
            }
        }

        public async Task<ProdutoDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando produto com ID {id}", id);

            try
            {
                var produto = await _context.Produtos.FindAsync(id);

                if (produto == null)
                {
                    _logger.LogWarning("Produto com ID {id} não encontrado.", id);
                    return null;
                }

                _logger.LogInformation("Produto {id} encontrado: {nome}", produto.Id, produto.NomeProduto);

                return _mapper.Map<ProdutoDto>(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto com ID {id}", id);
                throw;
            }
        }

        public async Task<ProdutoDto> CreateAsync(ProdutoDto dto)
        {
            _logger.LogInformation("Criando novo produto: {nome}", dto.NomeProduto);

            try
            {
                var produto = _mapper.Map<Produto>(dto);

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto criado com sucesso: ID={id}, Nome={nome}, Valor={valor}",
                    produto.Id, produto.NomeProduto, produto.Valor);

                return _mapper.Map<ProdutoDto>(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto: {nome}", dto.NomeProduto);
                throw;
            }
        }
    }
}
