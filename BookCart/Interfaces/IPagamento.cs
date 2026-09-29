namespace BookCart.Interfaces;

// INTERFACE: contrato para qualquer forma de pagamento
public interface IPagamento
{
    string Nome { get; }
    decimal CalcularValorFinal(decimal valor);
    void RealizarPagamento(decimal valor);
}
