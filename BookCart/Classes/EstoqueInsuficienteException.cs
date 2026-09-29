namespace BookCart.Classes;

// EXCEÇÃO PERSONALIZADA
public class EstoqueInsuficienteException : Exception
{
    public EstoqueInsuficienteException(string titulo, int disponivel, int solicitado)
        : base($"Estoque insuficiente para \"{titulo}\": disponível {disponivel}, solicitado {solicitado}.") { }
}
