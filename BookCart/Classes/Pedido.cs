namespace BookCart.Classes;

public enum StatusPedido { Aberto, Pago, Cancelado }

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public List<ItemPedido> Itens { get; set; } = new();
    public StatusPedido Status { get; set; } = StatusPedido.Aberto; // RN04
    public string FormaPagamento { get; set; }
    public decimal PercentualDesconto { get; set; }
    public decimal ValorFinal { get; set; }

    // RN02: total calculado automaticamente
    public decimal Subtotal => Itens.Sum(i => i.Subtotal);

    public void AdicionarItem(Livro livro, int quantidade)
    {
        var existente = Itens.FirstOrDefault(i => i.LivroId == livro.Id);
        if (existente != null)
        {
            if (quantidade <= 0) throw new ArgumentException("A quantidade deve ser maior que zero.");
            existente.Quantidade += quantidade;
        }
        else Itens.Add(new ItemPedido(livro, quantidade));
    }

    public override string ToString()
    {
        var linhas = string.Join("\n", Itens.Select(i => $"      {i.Quantidade}x {i.Titulo} ({Validador.Moeda(i.PrecoUnitario)})"));
        return $"Pedido #{Id} | {Data:dd/MM/yyyy HH:mm} | Cliente #{ClienteId} | {Status}\n{linhas}\n" +
               $"      Subtotal: {Validador.Moeda(Subtotal)} | Desc. cliente: {PercentualDesconto:P0} | " +
               $"Pagamento: {FormaPagamento} | TOTAL: {Validador.Moeda(ValorFinal)}";
    }
}
