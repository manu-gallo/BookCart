namespace BookCart.Classes;

public class Livro
{
    private string titulo;
    private string autor;
    private decimal preco;
    private int estoque;

    public int Id { get; set; }

    public string Titulo { get => titulo; set => titulo = Validador.TextoObrigatorio(value, "Título"); }
    public string Autor { get => autor; set => autor = Validador.TextoObrigatorio(value, "Autor"); }
    public Genero Genero { get; set; }

    public decimal Preco
    {
        get => preco;
        set
        {
            if (value <= 0) throw new ArgumentException("O preço deve ser maior que zero.");
            preco = value;
        }
    }

    public int Estoque
    {
        get => estoque;
        set
        {
            if (value < 0) throw new ArgumentException("O estoque não pode ser negativo.");
            estoque = value;
        }
    }

    // DESAFIO 01: estoque mínimo (RN08)
    public int EstoqueMinimo { get; set; } = 3;

    public Livro() { }

    public Livro(int id, string titulo, string autor, Genero genero, decimal preco, int estoque, int estoqueMinimo = 3)
    {
        this.Id = id;
        this.Titulo = titulo;
        this.Autor = autor;
        this.Genero = genero;
        this.Preco = preco;
        this.Estoque = estoque;
        this.EstoqueMinimo = estoqueMinimo;
    }

    public bool EstoqueBaixo() => Estoque <= EstoqueMinimo;

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade > Estoque)
            throw new EstoqueInsuficienteException(Titulo, Estoque, quantidade);
        Estoque -= quantidade;
    }

    public void Repor(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("A quantidade deve ser maior que zero.");
        Estoque += quantidade;
    }

    public override string ToString() =>
        $"#{Id,-3} {Titulo,-24} {Autor,-16} {Validador.NomeGenero(Genero),-22} {Validador.Moeda(Preco),10}  Estoque: {Estoque}";
}
