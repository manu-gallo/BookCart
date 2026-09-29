using System.Text.Json.Serialization;

namespace BookCart.Classes;

// Permite salvar/carregar Cliente e ClienteVip no mesmo JSON
[JsonPolymorphic(TypeDiscriminatorPropertyName = "tipo")]
[JsonDerivedType(typeof(ClienteVip), "vip")]
public class Cliente : Pessoa
{
    public Cliente() { }
    public Cliente(int id, string nome, string email) : base(id, nome, email) { }

    [JsonIgnore]
    public virtual decimal PercentualDesconto => 0m;

    public override string Descricao() => $"[Cliente] {base.Descricao()}";
}

// ESPECIALIZAÇÃO: cliente VIP ganha 10% em todos os pedidos (RN05)
public class ClienteVip : Cliente
{
    public ClienteVip() { }
    public ClienteVip(int id, string nome, string email) : base(id, nome, email) { }

    [JsonIgnore]
    public override decimal PercentualDesconto => 0.10m;

    public override string Descricao() => $"[VIP -10%] {Nome} <{Email}>";
}
