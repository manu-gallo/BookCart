using BookCart.Interfaces;

namespace BookCart.Services;

public class PagamentoPix : IPagamento
{
    public string Nome => "Pix";
    // RN06: 5% de desconto no Pix
    public decimal CalcularValorFinal(decimal valor) => Math.Round(valor * 0.95m, 2);
    public void RealizarPagamento(decimal valor) =>
        Console.WriteLine($"  ✔ Pix de {Validador.Moeda(valor)} confirmado (5% de desconto aplicado).");
}

public class PagamentoCartao : IPagamento
{
    private readonly int parcelas;
    public PagamentoCartao(int parcelas)
    {
        if (parcelas < 1 || parcelas > 6) throw new ArgumentException("Parcelas devem ser de 1 a 6.");
        this.parcelas = parcelas;
    }
    public string Nome => $"Cartão {parcelas}x";
    public decimal CalcularValorFinal(decimal valor) => Math.Round(valor, 2);
    public void RealizarPagamento(decimal valor) =>
        Console.WriteLine($"  ✔ Cartão aprovado: {parcelas}x de {Validador.Moeda(valor / parcelas)}.");
}

public class PagamentoBoleto : IPagamento
{
    public string Nome => "Boleto";
    public decimal CalcularValorFinal(decimal valor) => Math.Round(valor, 2);
    public void RealizarPagamento(decimal valor) =>
        Console.WriteLine($"  ✔ Boleto de {Validador.Moeda(valor)} gerado (vence em 3 dias).");
}
