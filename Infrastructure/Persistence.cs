using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
        public DbSet<Produto> Produtos => Set<Produto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>(e =>
            {
                e.ToTable("tb_pedidos");
                e.HasKey(p => p.Id);
                e.Property(p => p.NomeCliente).HasMaxLength(60).IsRequired();
                e.Property(p => p.EmailCliente).HasMaxLength(60).IsRequired();
                e.Property(p => p.DataCriacao).HasColumnType("datetime");
                e.Property(p => p.Pago).HasColumnType("bit");
            });

            modelBuilder.Entity<Produto>(e =>
            {
                e.ToTable("tb_produto");
                e.HasKey(p => p.Id);
                e.Property(p => p.NomeProduto).HasMaxLength(20).IsRequired();
                e.Property(p => p.Valor).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<ItemPedido>(e =>
            {
                e.ToTable("tb_itenspedido");
                e.HasKey(i => i.Id);
                e.Property(i => i.Quantidade).IsRequired();

                e.HasOne<Pedido>()
                    .WithMany(p => p.ItensPedido)
                    .HasForeignKey(i => i.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(i => i.Produto)
                    .WithMany()
                    .HasForeignKey(i => i.ProdutoId);
            });
        }
    }
}
