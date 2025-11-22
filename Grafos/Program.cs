using System;
using System.IO;

namespace Grafos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool sair = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                     TRABALHO GRAFOS                        ║");
                Console.WriteLine("║                                                            ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n=== MENU PRINCIPAL ===");
                Console.WriteLine("1. Exemplo: Grafo Clássico (6 vértices - Ford-Fulkerson)");
                Console.WriteLine("2. Exemplo: Otimização com Busca Local");
                Console.WriteLine("3. Carregar Grafo de Arquivo (.txt)");
                Console.WriteLine("4. Criar Grafo Personalizado (Manual)");
                Console.WriteLine("0. Sair");
                Console.Write("\n⚡ Escolha uma opção: ");

                string opcao = Console.ReadLine();
                Console.WriteLine();

                switch (opcao)
                {
                    case "1":
                        ExemploGrafoClassico();
                        break;

                    case "2":
                        ExemploComBuscaLocal();
                        break;

                    case "3":
                        ExemploCarregarArquivo();
                        break;

                    case "4":
                        ExemploGrafoPersonalizado();
                        break;

                    case "0":
                        sair = true;
                        Console.WriteLine("Programa encerrado");
                        break;

                    default:
                        Console.WriteLine("Opção inválida! Por favor, escolha uma opção válida.");
                        break;
                }

                if (!sair)
                {
                    Console.WriteLine("\n⏎ Pressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();
                }
            }
        }

        static void ExemploGrafoClassico()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        EXEMPLO: GRAFO CLÁSSICO                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            var grafo = new GrafoLista(true, true);

            // Vértices 0 a 5
            for (int i = 0; i < 6; i++)
                grafo.InserirVertice($"V{i}");

            // Arestas com capacidades
            grafo.InserirAresta(0, 1, 16);
            grafo.InserirAresta(0, 2, 13);
            grafo.InserirAresta(1, 2, 10);
            grafo.InserirAresta(1, 3, 12);
            grafo.InserirAresta(2, 1, 4);
            grafo.InserirAresta(2, 4, 14);
            grafo.InserirAresta(3, 2, 9);
            grafo.InserirAresta(3, 5, 20);
            grafo.InserirAresta(4, 3, 7);
            grafo.InserirAresta(4, 5, 4);

            Console.WriteLine("\nEstrutura do Grafo:");
            grafo.ImprimeGrafo();

            Console.WriteLine("\n--- Calculando Fluxo Máximo ---");
            Console.WriteLine("Origem: V0 (vértice 0)");
            Console.WriteLine("Destino: V5 (vértice 5)");

            var fluxo = new FluxoMaximo(grafo, 0, 5);
            fluxo.ImprimirResultado();
        }

        static void ExemploComBuscaLocal()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           EXEMPLO: OTIMIZAÇÃO COM BUSCA LOCAL              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            var grafo = new GrafoLista(true, true);

            for (int i = 0; i < 5; i++)
                grafo.InserirVertice($"V{i}");

            grafo.InserirAresta(0, 1, 10);
            grafo.InserirAresta(0, 2, 10);
            grafo.InserirAresta(1, 3, 5);
            grafo.InserirAresta(2, 3, 8);
            grafo.InserirAresta(3, 4, 10);
            grafo.InserirAresta(1, 4, 7);

            Console.WriteLine("\nEstrutura do Grafo:");
            grafo.ImprimeGrafo();

            Console.WriteLine("\n--- Executando Ford-Fulkerson + Busca Local ---");
            Console.WriteLine("Origem: V0 (vértice 0)");
            Console.WriteLine("Destino: V4 (vértice 4)");

            var fluxo = new FluxoMaximo(grafo, 0, 4);
            fluxo.ImprimirResultadoComBuscaLocal();
        }

        static void ExemploCarregarArquivo()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              CARREGAR GRAFO DE ARQUIVO                     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            string caminho = "grafo_exemplo.txt";

            // Se o arquivo não existir, cria automaticamente com dados de exemplo
            if (!File.Exists(caminho))
            {
                Console.WriteLine($"\n  Arquivo '{caminho}' não encontrado.");
                Console.WriteLine(" Criando arquivo de exemplo automaticamente...\n");

                CriarArquivoExemplo(caminho);

                Console.WriteLine($" Arquivo '{caminho}' criado com sucesso!");
                Console.WriteLine("\n Conteúdo do arquivo criado:");
                Console.WriteLine("┌────────────────────────────────────────────┐");
                foreach (var linha in File.ReadAllLines(caminho))
                {
                    Console.WriteLine($"│ {linha,-42} │");
                }
                Console.WriteLine("└────────────────────────────────────────────┘\n");
            }

            try
            {
                var grafo = new GrafoLista(caminho);
                Console.WriteLine($" Grafo carregado de '{caminho}'!");

                grafo.ImprimeGrafo();

                Console.Write("\n ID do Vértice Origem: ");
                if (!int.TryParse(Console.ReadLine(), out int origem))
                {
                    Console.WriteLine(" Entrada inválida!");
                    return;
                }

                Console.Write(" ID do Vértice Destino: ");
                if (!int.TryParse(Console.ReadLine(), out int destino))
                {
                    Console.WriteLine("❌ Entrada inválida!");
                    return;
                }

                var fluxo = new FluxoMaximo(grafo, origem, destino);

                Console.WriteLine("\n Escolha o método:");
                Console.WriteLine("[1] Apenas Ford-Fulkerson");
                Console.WriteLine("[2] Ford-Fulkerson + Busca Local");
                Console.Write("Opção: ");

                string metodo = Console.ReadLine();

                if (metodo == "2")
                    fluxo.ImprimirResultadoComBuscaLocal();
                else
                    fluxo.ImprimirResultado();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao processar arquivo: {ex.Message}");
            }
        }

        static void CriarArquivoExemplo(string caminho)
        {
            var conteudo = @"6 10 1 1
                            0 1 16
                            0 2 13
                            1 2 10
                            1 3 12
                            2 1 4
                            2 4 14
                            3 2 9
                            3 5 20
                            4 3 7
                            4 5 4";

            File.WriteAllText(caminho, conteudo);
        }

        static void ExemploGrafoPersonalizado()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║             CRIAR GRAFO PERSONALIZADO                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            try
            {
                Console.Write("\n Número de vértices: ");
                if (!int.TryParse(Console.ReadLine(), out int numV) || numV <= 0)
                {
                    Console.WriteLine(" Número inválido!");
                    return;
                }

                var grafo = new GrafoLista(true, true);

                // Cria os vértices
                for (int i = 0; i < numV; i++)
                    grafo.InserirVertice($"V{i}");

                Console.WriteLine($"\n {numV} vértices criados (V0 até V{numV - 1})");
                Console.WriteLine("\n Digite as arestas no formato: origem destino peso");
                Console.WriteLine("   Exemplo: 0 1 10");
                Console.WriteLine("   Digite 'fim' quando terminar.\n");

                int contador = 0;
                while (true)
                {
                    Console.Write($"Aresta #{contador + 1}: ");
                    string linha = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(linha) || linha.ToLower() == "fim")
                        break;

                    var partes = linha.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (partes.Length >= 3)
                    {
                        try
                        {
                            int u = int.Parse(partes[0]);
                            int v = int.Parse(partes[1]);
                            float w = float.Parse(partes[2]);

                            if (u < 0 || u >= numV || v < 0 || v >= numV)
                            {
                                Console.WriteLine($"    Vértices devem estar entre 0 e {numV - 1}");
                                continue;
                            }

                            grafo.InserirAresta(u, v, w);
                            Console.WriteLine($"    Aresta ({u} → {v}, peso={w}) adicionada");
                            contador++;
                        }
                        catch
                        {
                            Console.WriteLine("    Formato inválido. Use: origem destino peso");
                        }
                    }
                    else
                    {
                        Console.WriteLine("    Formato inválido. Use: origem destino peso");
                    }
                }

                if (contador == 0)
                {
                    Console.WriteLine("\n Nenhuma aresta foi adicionada!");
                    return;
                }

                Console.WriteLine($"\n Total de {contador} aresta(s) adicionada(s)");
                Console.WriteLine("\nEstrutura do Grafo:");
                grafo.ImprimeGrafo();

                Console.Write("\n ID do vértice de origem: ");
                if (!int.TryParse(Console.ReadLine(), out int s) || s < 0 || s >= numV)
                {
                    Console.WriteLine(" Origem inválida!");
                    return;
                }

                Console.Write(" ID do vértice de destino: ");
                if (!int.TryParse(Console.ReadLine(), out int t) || t < 0 || t >= numV)
                {
                    Console.WriteLine(" Destino inválido!");
                    return;
                }

                if (s == t)
                {
                    Console.WriteLine(" Origem e destino devem ser diferentes!");
                    return;
                }

                Console.WriteLine("\n Escolha o método:");
                Console.WriteLine("[1] Apenas Ford-Fulkerson");
                Console.WriteLine("[2] Ford-Fulkerson + Busca Local");
                Console.Write("Opção: ");

                string metodo = Console.ReadLine();

                var fluxo = new FluxoMaximo(grafo, s, t);

                if (metodo == "2")
                    fluxo.ImprimirResultadoComBuscaLocal();
                else
                    fluxo.ImprimirResultado();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Erro: {ex.Message}");
            }
        }
    }
}