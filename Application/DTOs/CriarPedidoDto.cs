namespace Application.DTOs
{
    public class CriarPedidoDto
    {
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public bool Pago { get; set; }
        public List<ItemPedidoDto> ItensPedido { get; set; } = new();
    }
}
