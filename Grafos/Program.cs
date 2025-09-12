using System;
using System.IO;
using System.Linq;

namespace Grafos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("    PARTE 1    ");
            Console.WriteLine("=====================================\n");

            ExecucaoParte1();

            Console.WriteLine("\n");
            Console.WriteLine("Pressione ENTER para continuar para a Parte 2...");
            Console.ReadLine();

            Console.WriteLine("\n=====================================");
            Console.WriteLine("    PARTE 2    ");
            Console.WriteLine("=====================================\n");

            ExecucaoParte2();

            Console.WriteLine("\n");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void ExecucaoParte1()
        {
            Console.WriteLine("1. IMPLEMENTAÇÃO LISTA DE ADJACÊNCIA");
            Console.WriteLine("=" + new string('=', 48) + "\n");

            // Criação do grafo com lista
            var grafoLista = new GrafoLista(false, true); // não-direcionado, ponderado

            Console.WriteLine("-> Inserindo vértices A, B, C, D:");
            grafoLista.InserirVertice("A");
            grafoLista.InserirVertice("B");
            grafoLista.InserirVertice("C");
            grafoLista.InserirVertice("D");

            Console.WriteLine("-> Inserindo arestas ponderadas:");
            Console.WriteLine("   A-B (peso 2.5), B-C (peso 3.0), C-D (peso 1.5), A-D (peso 4.0)");
            grafoLista.InserirAresta(0, 1, 2.5f);
            grafoLista.InserirAresta(1, 2, 3.0f);
            grafoLista.InserirAresta(2, 3, 1.5f);
            grafoLista.InserirAresta(0, 3, 4.0f);

            Console.WriteLine("\n-> Resultado:");
            grafoLista.ImprimeGrafo();

            Console.WriteLine("-> Funções básicas:");
            Console.WriteLine($"   - Existe aresta A->B? {grafoLista.ExisteAresta(0, 1)}");
            Console.WriteLine($"   - Peso da aresta A->B: {grafoLista.PesoAresta(0, 1)}");
            Console.WriteLine($"   - Label do vértice 0: '{grafoLista.LabelVertice(0)}'");
            Console.WriteLine($"   - Vizinhos do vértice A: [{string.Join(", ", grafoLista.RetornarVizinhos(0))}]");

            PausarExecucao();

            // Testando remoções
            Console.WriteLine("\n2. TESTANDO REMOÇÕES");
            Console.WriteLine("=" + new string('=', 19) + "\n");

            Console.WriteLine("-> Estado atual do grafo:");
            grafoLista.ImprimeGrafo();

            Console.WriteLine($"-> Removendo aresta A-B: {grafoLista.RemoverAresta(0, 1)}");
            Console.WriteLine("-> Grafo após remoção da aresta A-B:");
            grafoLista.ImprimeGrafo();

            Console.WriteLine($"-> Removendo vértice D (índice 3): {grafoLista.RemoverVertice(3)}");
            Console.WriteLine("-> Grafo após remoção do vértice D:");
            grafoLista.ImprimeGrafo();

            PausarExecucao();

            Console.WriteLine("\n-> Testando acesso a aresta inexistente:");
            try
            {
                Console.WriteLine("   Tentando acessar peso da aresta A-B (removida)...");
                grafoLista.PesoAresta(0, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.GetType().Name} - {ex.Message}");
            }

            PausarExecucao();

            // Matriz de adjacência
            Console.WriteLine("\n4. IMPLEMENTAÇÃO MATRIZ DE ADJACÊNCIA");
            Console.WriteLine("=" + new string('=', 39) + "\n");

            var grafoMatriz = new GrafoMatriz(false, true);// não-direcionado, ponderado

            Console.WriteLine("-> Criando o mesmo grafo com matriz de adjacência:");
            grafoMatriz.InserirVertice("A");
            grafoMatriz.InserirVertice("B");
            grafoMatriz.InserirVertice("C");
            grafoMatriz.InserirVertice("D");

            grafoMatriz.InserirAresta(0, 1, 2.5f);
            grafoMatriz.InserirAresta(1, 2, 3.0f);
            grafoMatriz.InserirAresta(2, 3, 1.5f);
            grafoMatriz.InserirAresta(0, 3, 4.0f);

            grafoMatriz.ImprimeGrafo();

            PausarExecucao();

            // Comparação matriz vs lista
            Console.WriteLine("\n5. COMPARAÇÃO: MATRIZ vs LISTA");
            Console.WriteLine("=" + new string('=', 30) + "\n");

            Console.WriteLine("-> Verificação de consistência:");
            Console.WriteLine($"   Lista  - Existe aresta A->B: {grafoLista.ExisteAresta(0, 1)}");
            Console.WriteLine($"   Matriz - Existe aresta A->B: {grafoMatriz.ExisteAresta(0, 1)}");

            Console.WriteLine($"\n   Lista - Vizinhos de A: [{string.Join(", ", grafoLista.RetornarVizinhos(0))}]");
            Console.WriteLine($"   Matriz - Vizinhos de A: [{string.Join(", ", grafoMatriz.RetornarVizinhos(0))}]");

            PausarExecucao();

            // Grafo direcionado
            Console.WriteLine("\n6. GRAFO DIRECIONADO NÃO-PONDERADO");
            Console.WriteLine("=" + new string('=', 34) + "\n");

            var grafoDirecionado = new GrafoLista(true, false); // direcionado, não-ponderado

            grafoDirecionado.InserirVertice("X");
            grafoDirecionado.InserirVertice("Y");
            grafoDirecionado.InserirVertice("Z");

            Console.WriteLine("-> Inserindo arestas direcionadas: X->Y, Y->Z, Z->X");
            grafoDirecionado.InserirAresta(0, 1); // X -> Y
            grafoDirecionado.InserirAresta(1, 2); // Y -> Z
            grafoDirecionado.InserirAresta(2, 0); // Z -> X

            grafoDirecionado.ImprimeGrafo();

            PausarExecucao();

            // Algoritmos de busca
            Console.WriteLine("\n7. ALGORITMOS DE BUSCA (CLASSE GRAFO)");
            Console.WriteLine("=" + new string('=', 53) + "\n");

            Console.WriteLine("-> Usando o grafo com lista de adjacência:");
            grafoLista.ImprimeGrafo();

            Console.WriteLine("-> Busca em Largura a partir do vértice 0 (A):");
            var bfs = grafoLista.BuscaEmLargura(0);
            Console.WriteLine($"   Ordem: [{string.Join(" -> ", bfs.Select(v => $"{v}({grafoLista.LabelVertice(v)})"))}]");

            Console.WriteLine("\n-> Busca em Profundidade a partir do vértice 0 (A):");
            var dfs = grafoLista.BuscaEmProfundidade(0);
            Console.WriteLine($"   Ordem: [{string.Join(" -> ", dfs.Select(v => $"{v}({grafoLista.LabelVertice(v)})"))}]");

            Console.WriteLine("\n-> Busca de Caminho de A para C:");
            var caminho = grafoLista.BuscaCaminho(0, 2);
            Console.WriteLine($"   Caminho: [{string.Join(" -> ", caminho.Select(v => $"{v}({grafoLista.LabelVertice(v)})"))}]");

            Console.WriteLine("\n-> MESMOS ALGORITMOS executando na matriz:");
            var bfsMatriz = grafoMatriz.BuscaEmLargura(0);
            var dfsMatriz = grafoMatriz.BuscaEmProfundidade(0);
            Console.WriteLine($"   BFS Matriz: [{string.Join(" -> ", bfsMatriz.Select(v => $"{v}({grafoMatriz.LabelVertice(v)})"))}]");
            Console.WriteLine($"   DFS Matriz: [{string.Join(" -> ", dfsMatriz.Select(v => $"{v}({grafoMatriz.LabelVertice(v)})"))}]");
        }

        static void ExecucaoParte2()
        {
            var (caminhoEx, caminhoDijkstra, caminhoNaoPonderado, caminhoDesconexo, caminhoDirecionado) = CriarArquivosExemplo();

            Console.WriteLine("1. LEITURA DE ARQUIVOS DE GRAFOS");
            Console.WriteLine("=" + new string('=', 31) + "\n");

            Console.WriteLine("-> Conteúdo do arquivo 'exemplo.txt':");
            Console.WriteLine("   4 4 0 1    <- 4 vértices, 4 arestas, não-direcionado, ponderado");
            Console.WriteLine("   0 1 2.5    <- aresta 0->1 com peso 2.5");
            Console.WriteLine("   1 2 3.0    <- aresta 1->2 com peso 3.0");
            Console.WriteLine("   2 3 1.5    <- aresta 2->3 com peso 1.5");
            Console.WriteLine("   0 3 4.0    <- aresta 0->3 com peso 4.0");

            try
            {
                Console.WriteLine("\n-> Carregando com LISTA DE ADJACÊNCIA:");
                var grafoLista = new GrafoLista(caminhoEx);
                grafoLista.ImprimeGrafo();

                Console.WriteLine("-> Carregando com MATRIZ DE ADJACÊNCIA:");
                var grafoMatriz = new GrafoMatriz(caminhoEx);
                grafoMatriz.ImprimeGrafo();

                PausarExecucao();

                Console.WriteLine("\n2. ALGORITMOS DE BUSCA E DIJKSTRA");
                Console.WriteLine("=" + new string('=', 33) + "\n");

                Console.WriteLine("\n ALGORITMOS COM LISTA DE ADJACÊNCIA:");

                grafoLista.ImprimeBuscaEmLargura(0);
                grafoLista.ImprimeBuscaEmProfundidade(0);

                PausarExecucao();

                Console.WriteLine("\n3. TESTANDO DIJKSTRA");
                Console.WriteLine("=" + new string('=', 20) + "\n");

                Console.WriteLine("-> Carregando grafo direcionado ponderado para Dijkstra:");
                Console.WriteLine("\n   Conteúdo do arquivo 'dijkstra.txt':");
                Console.WriteLine("   5 8 1 1    <- 5 vértices, 8 arestas, direcionado, ponderado");

                var grafoDijkstra = new GrafoLista(caminhoDijkstra);
                grafoDijkstra.ImprimeGrafo();

                grafoDijkstra.ImprimeDijkstra(0);

                PausarExecucao();

                Console.WriteLine("\n-> MESMOS ALGORITMOS COM MATRIZ DE ADJACÊNCIA:");
                var grafoDijkstraMatriz = new GrafoMatriz(caminhoDijkstra);

                grafoDijkstraMatriz.ImprimeBuscaEmLargura(0);
                grafoDijkstraMatriz.ImprimeBuscaEmProfundidade(0);
                grafoDijkstraMatriz.ImprimeDijkstra(0);

                PausarExecucao();

                Console.WriteLine("\n4. DIFERENTES FORMATOS DE ARQUIVO");
                Console.WriteLine("=" + new string('=', 33) + "\n");

                // Grafo não-ponderado
                Console.WriteLine("-> Testando grafo NÃO-PONDERADO:");
                var grafoNaoPonderado = new GrafoLista(caminhoNaoPonderado);
                grafoNaoPonderado.ImprimeGrafo();

                Console.WriteLine("-> Tentando executar Dijkstra em grafo não-ponderado:");
                try
                {
                    grafoNaoPonderado.ImprimeDijkstra(0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }

                PausarExecucao();

                // Grafo direcionado
                Console.WriteLine("\n-> Testando grafo DIRECIONADO:");
                var grafoDirecionado = new GrafoLista(caminhoDirecionado);
                grafoDirecionado.ImprimeGrafo();

                Console.WriteLine("BFS a partir do vértice 0:");
                grafoDirecionado.ImprimeBuscaEmLargura(0);

                PausarExecucao();

                Console.WriteLine("\n5. TESTANDO CONECTIVIDADE E CASOS ESPECIAIS");
                Console.WriteLine("=" + new string('=', 43) + "\n");

                Console.WriteLine("-> Grafo DESCONEXO (componentes separados):");
                var grafoDesconexo = new GrafoLista(caminhoDesconexo);
                grafoDesconexo.ImprimeGrafo();

                Console.WriteLine($"-> Grafo é conexo? {grafoDesconexo.EhConexo()}");
                Console.WriteLine($"-> Existe caminho 0->4? {grafoDesconexo.ExisteCaminho(0, 4)}");
                Console.WriteLine($"-> Existe caminho 3->4? {grafoDesconexo.ExisteCaminho(3, 4)}");

                Console.WriteLine("\n-> BFS a partir do vértice 0 (não alcança todos os vértices):");
                grafoDesconexo.ImprimeBuscaEmLargura(0);

                PausarExecucao();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        static (string, string, string, string, string) CriarArquivosExemplo()
        {
            string caminhoArquivoExemplo = @"C:\Users\usuario\Downloads\exemplo.txt";
            string caminhoArquivoDijkstra = @"C:\Users\usuario\Downloads\dijkstra.txt";
            string caminhoNaoPonderado = @"C:\Users\usuario\Downloads\nao_ponderado.txt";
            string caminhoDesconexo = @"C:\Users\usuario\Downloads\desconexo.txt";
            string caminhoDirecionado = @"C:\Users\usuario\Downloads\direcionado.txt";

            // Arquivo exemplo (não-direcionado, ponderado)
            string exemplo = @"4 4 0 1
                               0 1 2.5
                               1 2 3.0
                               2 3 1.5
                               0 3 4.0";

            File.WriteAllText(caminhoArquivoExemplo, exemplo);

            // Arquivo para Dijkstra (direcionado, ponderado)
            string dijkstra = @"5 8 1 1
                                0 1 4.0
                                0 2 2.0
                                1 2 1.0
                                1 3 5.0
                                2 3 8.0
                                2 4 10.0
                                3 4 2.0
                                4 1 3.0";

            File.WriteAllText(caminhoArquivoDijkstra, dijkstra);

            // Arquivo não-ponderado
            string naoPonderado = @"4 4 0 0
                                    0 1
                                    1 2
                                    2 3
                                    3 0";

            File.WriteAllText(caminhoNaoPonderado, naoPonderado);

            // Arquivo desconexo
            string desconexo = @"5 3 0 1
                                 0 1 1.0
                                 1 2 2.0
                                 3 4 1.5";

            File.WriteAllText(caminhoDesconexo, desconexo);

            // Arquivo direcionado não-ponderado
            string direcionado = @"4 4 1 0
                                   0 1
                                   1 2
                                   2 3
                                   3 0";

            File.WriteAllText(caminhoDirecionado, direcionado);

            return (caminhoArquivoExemplo, caminhoArquivoDijkstra, caminhoNaoPonderado, caminhoDesconexo, caminhoDirecionado);
        }

        static void PausarExecucao()
        {
            Console.WriteLine("\n" + new string('-', 50));
            Console.WriteLine("[Pressione ENTER para continuar...]");
            Console.ReadLine();
        }
    }
}