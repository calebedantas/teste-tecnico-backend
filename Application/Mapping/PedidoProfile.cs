using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<Pedido, PedidoDto>()
                .ForMember(d => d.ValorTotal, o => o.Ignore());

            CreateMap<ItemPedido, ItemPedidoDto>()
                .ForMember(d => d.IdProduto, o => o.MapFrom(s => s.ProdutoId))
                .ForMember(d => d.NomeProduto, o => o.MapFrom(s => s.Produto!.NomeProduto))
                .ForMember(d => d.ValorUnitario, o => o.MapFrom(s => s.Produto!.Valor));
        }
    }
}
