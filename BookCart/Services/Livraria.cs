using BookCart.Classes;
using BookCart.Interfaces;

namespace BookCart.Services;

public class Livraria
{
    public List<Livro> Livros { get; set; } = new();
    public List<Cliente> Clientes { get; set; } = new();
    public List<Pedido> Pedidos { get; set; } = new();

    private static int ProximoId(IEnumerable<int> ids) => ids.Any() ? ids.Max() + 1 : 1;

    public void CarregarCatalogoInicial()
    {
        Livros.Add(new Livro(1, "O Acordo", "Elle Kennedy", Genero.Romance, 43.14m, 10));
        Livros.Add(new Livro(2, "É Assim que Acaba", "Colleen Hoover", Genero.RomanceDrama, 24.90m, 15));
        Livros.Add(new Livro(3, "Verity", "Colleen Hoover", Genero.SuspenseThriller, 43.09m, 8));
        Livros.Add(new Livro(4, "The Kiss of Deception", "Mary E. Pearson", Genero.Fantasia, 74.90m, 5));
        Livros.Add(new Livro(5, "Gild", "Raven Kennedy", Genero.FantasiaDarkRomance, 53.41m, 6));
        Livros.Add(new Livro(6, "Reckless 2", "Lauren Roberts", Genero.FantasiaRomance, 42.62m, 7));
    }

    public Cliente CadastrarCliente(string nome, string email, bool vip)
    {
        if (Clientes.Any(c => c.Email == email.Trim().ToLower()))
            throw new InvalidOperationException("Já existe um cliente com esse e-mail.");
        int id = ProximoId(Clientes.Select(c => c.Id));
        Cliente c = vip ? new ClienteVip(id, nome, email) : new Cliente(id, nome, email);
        Clientes.Add(c);
        return c;
    }

    public Cliente BuscarCliente(int id) => Clientes.FirstOrDefault(c => c.Id == id);

    public void RemoverCliente(int id)
    {
        var c = BuscarCliente(id) ?? throw new KeyNotFoundException("Cliente não encontrado.");
        if (Pedidos.Any(p => p.ClienteId == id && p.Status != StatusPedido.Cancelado))
            throw new InvalidOperationException("Cliente possui pedidos ativos e não pode ser excluído.");
        Clientes.Remove(c);
    }

    public Livro CadastrarLivro(string titulo, string autor, Genero genero, decimal preco, int estoque)
    {
        var l = new Livro(ProximoId(Livros.Select(x => x.Id)), titulo, autor, genero, preco, estoque);
        Livros.Add(l);
        return l;
    }

    public Livro BuscarLivro(int id) => Livros.FirstOrDefault(l => l.Id == id);

    public List<Livro> BuscarLivros(string termo) =>
        Livros.Where(l => l.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase)
                       || l.Autor.Contains(termo, StringComparison.OrdinalIgnoreCase)
                       || Validador.NomeGenero(l.Genero).Contains(termo, StringComparison.OrdinalIgnoreCase)).ToList();

    public void RemoverLivro(int id)
    {
        var l = BuscarLivro(id) ?? throw new KeyNotFoundException("Livro não encontrado.");
        Livros.Remove(l);
    }

    public Pedido Comprar(int clienteId, List<(int livroId, int qtd)> itens, IPagamento pagamento)
    {
        var cliente = BuscarCliente(clienteId) ?? throw new KeyNotFoundException("Cliente não cadastrado.");
        if (itens.Count == 0) throw new InvalidOperationException("O pedido deve ter pelo menos um item."); // RN01

        var pedido = new Pedido { Id = ProximoId(Pedidos.Select(p => p.Id)), ClienteId = clienteId };
        foreach (var (livroId, qtd) in itens)
        {
            var livro = BuscarLivro(livroId) ?? throw new KeyNotFoundException($"Livro #{livroId} não encontrado.");
            pedido.AdicionarItem(livro, qtd);
        }

        // RN03: valida o estoque antes de baixar
        foreach (var item in pedido.Itens)
        {
            var livro = BuscarLivro(item.LivroId);
            if (item.Quantidade > livro.Estoque)
                throw new EstoqueInsuficienteException(livro.Titulo, livro.Estoque, item.Quantidade);
        }
        foreach (var item in pedido.Itens) BuscarLivro(item.LivroId).BaixarEstoque(item.Quantidade);

        pedido.PercentualDesconto = cliente.PercentualDesconto; // polimorfismo (RN05)
        decimal comDescontoCliente = pedido.Subtotal * (1 - pedido.PercentualDesconto);
        pedido.ValorFinal = pagamento.CalcularValorFinal(comDescontoCliente);
        pagamento.RealizarPagamento(pedido.ValorFinal);
        pedido.FormaPagamento = pagamento.Nome;
        pedido.Status = StatusPedido.Pago;

        Pedidos.Add(pedido);
        return pedido;
    }

    public void CancelarPedido(int id)
    {
        var p = Pedidos.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Pedido não encontrado.");
        if (p.Status == StatusPedido.Cancelado) throw new InvalidOperationException("Pedido já está cancelado.");
        foreach (var item in p.Itens) BuscarLivro(item.LivroId)?.Repor(item.Quantidade);
        p.Status = StatusPedido.Cancelado;
    }

    // DESAFIO 01: controle de estoque mínimo
    public List<Livro> LivrosComEstoqueBaixo() => Livros.Where(l => l.EstoqueBaixo()).OrderBy(l => l.Estoque).ToList();
}
