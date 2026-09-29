using BookCart.Classes;
using BookCart.Interfaces;
using BookCart.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Livraria livraria;
if (Armazenamento.Existe())
{
    try { livraria = Armazenamento.Carregar(); Console.WriteLine("Dados carregados de dados.json."); }
    catch (Exception ex) { Console.WriteLine($"Erro ao carregar: {ex.Message}"); livraria = new Livraria(); livraria.CarregarCatalogoInicial(); }
}
else
{
    livraria = new Livraria();
    livraria.CarregarCatalogoInicial();
}

int opcao = -1;
while (opcao != 0)
{
    Console.WriteLine("\n====================================");
    Console.WriteLine("   📚 BOOKCART — Livraria Virtual");
    Console.WriteLine("====================================");
    Console.WriteLine(" 1 - Cadastrar cliente");
    Console.WriteLine(" 2 - Cadastrar livro");
    Console.WriteLine(" 3 - Listar catálogo");
    Console.WriteLine(" 4 - Buscar livro (título/autor/gênero)");
    Console.WriteLine(" 5 - Listar clientes");
    Console.WriteLine(" 6 - Comprar livros (novo pedido)");
    Console.WriteLine(" 7 - Listar pedidos");
    Console.WriteLine(" 8 - Cancelar pedido");
    Console.WriteLine(" 9 - Excluir cadastro (cliente/livro)");
    Console.WriteLine("10 - Alerta de estoque mínimo");
    Console.WriteLine("11 - Salvar dados");
    Console.WriteLine("12 - Carregar dados");
    Console.WriteLine(" 0 - Sair");
    opcao = LerInt("\nDigite uma opção: ", -1);

    try
    {
        switch (opcao)
        {
            case 1:
                string nome = Ler("Nome: ");
                string email = Ler("E-mail: ");
                bool vip = Ler("Cliente VIP (10% de desconto)? (s/n): ").ToLower() == "s";
                var c = livraria.CadastrarCliente(nome, email, vip);
                Console.WriteLine($"Cliente cadastrado: {c.Descricao()} (id {c.Id})");
                break;

            case 2:
                string titulo = Ler("Título: ");
                string autor = Ler("Autor: ");
                Console.WriteLine("Gêneros: " + string.Join(" | ", Enum.GetValues<Genero>().Select(g => $"{(int)g}={Validador.NomeGenero(g)}")));
                var genero = (Genero)LerInt("Gênero (número): ");
                if (!Enum.IsDefined(genero)) throw new ArgumentException("Gênero inválido.");
                decimal preco = LerDecimal("Preço (R$): ");
                int est = LerInt("Estoque inicial: ");
                var l = livraria.CadastrarLivro(titulo, autor, genero, preco, est);
                Console.WriteLine($"Livro cadastrado com id {l.Id}.");
                break;

            case 3:
                if (livraria.Livros.Count == 0) throw new InvalidOperationException("Nenhum livro cadastrado.");
                livraria.Livros.ForEach(x => Console.WriteLine(x));
                break;

            case 4:
                var achados = livraria.BuscarLivros(Ler("Buscar por: "));
                if (achados.Count == 0) Console.WriteLine("Nenhum livro encontrado.");
                else achados.ForEach(x => Console.WriteLine(x));
                break;

            case 5:
                if (livraria.Clientes.Count == 0) throw new InvalidOperationException("Nenhum cliente cadastrado.");
                livraria.Clientes.ForEach(x => Console.WriteLine($"#{x.Id} {x.Descricao()}")); // polimorfismo
                break;

            case 6:
                if (livraria.Clientes.Count == 0) throw new InvalidOperationException("Cadastre um cliente antes de comprar.");
                int clienteId = LerInt("ID do cliente: ");
                var itens = new List<(int, int)>();
                while (true)
                {
                    int lid = LerInt("ID do livro (0 para finalizar): ");
                    if (lid == 0) break;
                    int q = LerInt("Quantidade: ");
                    itens.Add((lid, q));
                }
                Console.WriteLine("Pagamento: 1-Pix (5% off) | 2-Cartão | 3-Boleto");
                IPagamento pg = LerInt("Forma: ") switch
                {
                    1 => new PagamentoPix(),
                    2 => new PagamentoCartao(LerInt("Parcelas (1-6): ")),
                    3 => new PagamentoBoleto(),
                    _ => throw new ArgumentException("Forma de pagamento inválida.")
                };
                var pedido = livraria.Comprar(clienteId, itens, pg);
                Console.WriteLine("\n" + pedido);
                break;

            case 7:
                if (livraria.Pedidos.Count == 0) throw new InvalidOperationException("Nenhum pedido realizado.");
                livraria.Pedidos.ForEach(p => Console.WriteLine(p + "\n"));
                break;

            case 8:
                livraria.CancelarPedido(LerInt("ID do pedido: "));
                Console.WriteLine("Pedido cancelado e estoque devolvido.");
                break;

            case 9:
                if (Ler("Excluir (c)liente ou (l)ivro? ").ToLower() == "c") livraria.RemoverCliente(LerInt("ID do cliente: "));
                else livraria.RemoverLivro(LerInt("ID do livro: "));
                Console.WriteLine("Cadastro removido.");
                break;

            case 10:
                var baixos = livraria.LivrosComEstoqueBaixo();
                if (baixos.Count == 0) Console.WriteLine("Todos os livros estão com estoque saudável.");
                else
                {
                    Console.WriteLine("⚠ Livros para repor:");
                    baixos.ForEach(x => Console.WriteLine($"  {x.Titulo} — estoque {x.Estoque} (mínimo {x.EstoqueMinimo})"));
                    if (Ler("Repor algum? (s/n): ").ToLower() == "s")
                    {
                        var livroRep = livraria.BuscarLivro(LerInt("ID do livro: ")) ?? throw new KeyNotFoundException("Livro não encontrado.");
                        livroRep.Repor(LerInt("Quantidade a repor: "));
                        Console.WriteLine("Estoque atualizado.");
                    }
                }
                break;

            case 11:
                Armazenamento.Salvar(livraria);
                Console.WriteLine("Dados salvos em dados.json.");
                break;

            case 12:
                livraria = Armazenamento.Carregar();
                Console.WriteLine("Dados carregados.");
                break;

            case 0: Console.WriteLine("Até logo!"); break;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException
                                  or EstoqueInsuficienteException or FormatException or FileNotFoundException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Erro: {ex.Message}");
        Console.ResetColor();
    }
}

static string Ler(string msg) { Console.Write(msg); return Console.ReadLine() ?? ""; }

static int LerInt(string msg, int? padraoInvalido = null)
{
    Console.Write(msg);
    if (int.TryParse(Console.ReadLine(), out int v)) return v;
    if (padraoInvalido.HasValue) return padraoInvalido.Value;
    throw new FormatException("Digite um número inteiro válido.");
}

static decimal LerDecimal(string msg)
{
    Console.Write(msg);
    var txt = (Console.ReadLine() ?? "").Replace(',', '.');
    if (decimal.TryParse(txt, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal v)) return v;
    throw new FormatException("Digite um valor numérico válido.");
}
