using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(AppDbContext context, IMapper mapper, ILogger<PedidoService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PedidoDto>> GetAllAsync()
        {
            _logger.LogInformation("Iniciando busca de todos os pedidos");

            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Foram encontrados {count} pedidos.", pedidos.Count);

                var resultado = pedidos.Select(p => new PedidoDto
                {
                    Id = p.Id,
                    NomeCliente = p.NomeCliente,
                    EmailCliente = p.EmailCliente,
                    Pago = p.Pago,
                    ValorTotal = p.ItensPedido.Sum(i => i.Produto.Valor * i.Quantidade),
                    ItensPedido = p.ItensPedido.Select(i => new ItemPedidoDto
                    {
                        Id = i.Id,
                        IdProduto = i.ProdutoId,
                        NomeProduto = i.Produto.NomeProduto,
                        ValorUnitario = i.Produto.Valor,
                        Quantidade = i.Quantidade
                    }).ToList()
                });

                _logger.LogInformation("Busca concluída com sucesso.");
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos.");
                throw;
            }
        }

        public async Task<PedidoDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando pedido com ID {id}", id);

            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    _logger.LogWarning("Pedido com ID {id} não encontrado.", id);
                    return null;
                }

                _logger.LogInformation("Pedido com ID {id} encontrado para o cliente {cliente}", id, pedido.NomeCliente);

                return new PedidoDto
                {
                    Id = pedido.Id,
                    NomeCliente = pedido.NomeCliente,
                    EmailCliente = pedido.EmailCliente,
                    Pago = pedido.Pago,
                    ValorTotal = pedido.ItensPedido.Sum(i => i.Produto.Valor * i.Quantidade),
                    ItensPedido = pedido.ItensPedido.Select(i => new ItemPedidoDto
                    {
                        Id = i.Id,
                        IdProduto = i.ProdutoId,
                        NomeProduto = i.Produto.NomeProduto,
                        ValorUnitario = i.Produto.Valor,
                        Quantidade = i.Quantidade
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar o pedido com ID {id}", id);
                throw;
            }
        }

        public async Task<PedidoDto> CreateAsync(CriarPedidoDto cmd)
        {
            _logger.LogInformation("Criando novo pedido para {cliente} ({email})", cmd.NomeCliente, cmd.EmailCliente);

            try
            {
                var pedido = new Pedido
                {
                    NomeCliente = cmd.NomeCliente,
                    EmailCliente = cmd.EmailCliente,
                    Pago = cmd.Pago,
                    DataCriacao = DateTime.Now
                };

                foreach (var item in cmd.ItensPedido)
                {
                    _logger.LogDebug("Processando item: ProdutoID={produtoId}, Quantidade={quantidade}",
                        item.IdProduto, item.Quantidade);

                    var produto = await _context.Produtos.FindAsync(item.IdProduto);
                    if (produto == null)
                    {
                        _logger.LogWarning("Produto com ID {produtoId} não encontrado. Abortando criação de pedido.",
                            item.IdProduto);
                        throw new Exception($"Produto com ID {item.IdProduto} não encontrado.");
                    }

                    pedido.ItensPedido.Add(new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        Quantidade = item.Quantidade
                    });
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pedido {id} criado com sucesso para {cliente}.",
                    pedido.Id, pedido.NomeCliente);

                var pedidoCompleto = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .FirstAsync(p => p.Id == pedido.Id);

                return new PedidoDto
                {
                    Id = pedidoCompleto.Id,
                    NomeCliente = pedidoCompleto.NomeCliente,
                    EmailCliente = pedidoCompleto.EmailCliente,
                    Pago = pedidoCompleto.Pago,
                    ValorTotal = pedidoCompleto.ItensPedido.Sum(i => i.Produto.Valor * i.Quantidade),
                    ItensPedido = pedidoCompleto.ItensPedido.Select(i => new ItemPedidoDto
                    {
                        Id = i.Id,
                        IdProduto = i.ProdutoId,
                        NomeProduto = i.Produto.NomeProduto,
                        ValorUnitario = i.Produto.Valor,
                        Quantidade = i.Quantidade
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido para {cliente} ({email})",
                    cmd.NomeCliente, cmd.EmailCliente);
                throw;
            }
        }
    }
}
