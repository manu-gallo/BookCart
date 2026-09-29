using System.Globalization;
using System.Text.RegularExpressions;
using BookCart.Classes;

// STATIC: métodos utilitários sem necessidade de instância
public static class Validador
{
    private static readonly CultureInfo Br = new("pt-BR");

    public static string TextoObrigatorio(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"{campo} é obrigatório.");
        return valor.Trim();
    }

    public static bool EmailValido(string email) =>
        !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static string Moeda(decimal v) => v.ToString("C2", Br);

    public static string NomeGenero(Genero g) => g switch
    {
        Genero.Romance => "Romance",
        Genero.RomanceDrama => "Romance/drama",
        Genero.SuspenseThriller => "Suspense/thriller",
        Genero.Fantasia => "Fantasia",
        Genero.FantasiaDarkRomance => "Fantasia/dark romance",
        Genero.FantasiaRomance => "Fantasia/romance",
        _ => g.ToString()
    };
}
