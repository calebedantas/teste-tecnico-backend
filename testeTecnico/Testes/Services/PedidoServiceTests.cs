using Application.Mapping;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Services
{
    public class PedidoServiceTests
    {
        private readonly AppDbContext _context;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "PedidosTestDb")
                .Options;

            _context = new AppDbContext(options);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PedidoProfile>();
            });
            var mapper = config.CreateMapper();

            _service = new PedidoService(_context, mapper);
        }

        [Fact(DisplayName = "Deve retornar um pedido pelo ID")]
        public async Task GetByIdAsync_DeveRetornarPedido_ComItensECalculoCorreto()
        {
            var produto = new Produto { NomeProduto = "Caneta", Valor = 2.50m };
            await _context.Produtos.AddAsync(produto);

            var pedido = new Pedido
            {
                NomeCliente = "João",
                EmailCliente = "joao@teste.com",
                Pago = true,
                DataCriacao = DateTime.UtcNow
            };
            pedido.ItensPedido.Add(new ItemPedido
            {
                Produto = produto,
                ProdutoId = produto.Id,
                Quantidade = 4
            });

            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(pedido.Id);

            result.Should().NotBeNull();
            result!.NomeCliente.Should().Be("João");
            result.ItensPedido.Should().HaveCount(1);
            result.ValorTotal.Should().Be(10.00m); 
        }
    }
}
