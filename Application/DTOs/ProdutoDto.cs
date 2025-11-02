namespace Application.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
