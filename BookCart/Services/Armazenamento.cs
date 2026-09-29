using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookCart.Services;

// SERIALIZAÇÃO: salva e carrega toda a livraria em JSON
public static class Armazenamento
{
    private const string Arquivo = "dados.json";
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static bool Existe() => File.Exists(Arquivo);

    public static void Salvar(Livraria livraria) =>
        File.WriteAllText(Arquivo, JsonSerializer.Serialize(livraria, Opcoes));

    public static Livraria Carregar()
    {
        if (!Existe()) throw new FileNotFoundException("Nenhum arquivo dados.json encontrado. Salve os dados primeiro.");
        return JsonSerializer.Deserialize<Livraria>(File.ReadAllText(Arquivo), Opcoes);
    }
}
