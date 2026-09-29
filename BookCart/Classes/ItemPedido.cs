namespace BookCart.Classes;

public class ItemPedido
{
    public int LivroId { get; set; }
    public string Titulo { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }

    public decimal Subtotal => PrecoUnitario * Quantidade;

    public ItemPedido() { }

    public ItemPedido(Livro livro, int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("A quantidade deve ser maior que zero.");
        this.LivroId = livro.Id;
        this.Titulo = livro.Titulo;
        this.PrecoUnitario = livro.Preco; // guarda o preço da época da compra
        this.Quantidade = quantidade;
    }
}
