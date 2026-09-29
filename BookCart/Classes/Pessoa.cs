namespace BookCart.Classes;

// HERANÇA: Pessoa é a classe base de Cliente (e ClienteVip)
public class Pessoa
{
    private string nome;
    private string email;

    public int Id { get; set; }

    // ENCAPSULAMENTO: campos private + validação nas propriedades
    public string Nome
    {
        get => nome;
        set => nome = Validador.TextoObrigatorio(value, "Nome");
    }

    public string Email
    {
        get => email;
        set
        {
            if (!Validador.EmailValido(value))
                throw new ArgumentException("E-mail inválido.");
            email = value.Trim().ToLower();
        }
    }

    public Pessoa() { }

    // CONSTRUTOR com this
    public Pessoa(int id, string nome, string email)
    {
        this.Id = id;
        this.Nome = nome;
        this.Email = email;
    }

    // POLIMORFISMO: método virtual sobrescrito nas filhas
    public virtual string Descricao() => $"{Nome} <{Email}>";
}
