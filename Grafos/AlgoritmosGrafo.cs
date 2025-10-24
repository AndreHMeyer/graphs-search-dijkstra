using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grafos
{
    public static class AlgoritmosGrafo
    {
        /// <summary>
        /// Coloração por Força Bruta - testa todas as combinações possíveis
        /// Retorna (0, null, tempo) se exceder o timeout
        /// </summary>
        public static (int numCores, int[] cores, double tempoMs) ColoracaoForcaBruta(this Grafo grafo, int timeoutSegundos = 30)
        {
            var sw = Stopwatch.StartNew();
            int n = grafo.NumeroVertices;

            if (n == 0)
            {
                sw.Stop();
                return (0, new int[0], sw.Elapsed.TotalMilliseconds);
            }

            int[] melhorColoracao = null;
            int numCores = 0;

            // Verificar se o grafo tem arestas
            bool temArestas = false;
            for (int i = 0; i < n && !temArestas; i++)
            {
                if (grafo.RetornarVizinhos(i).Count > 0)
                    temArestas = true;
            }

            // Se não tem arestas, 1 cor é suficiente
            int kInicial = temArestas ? 2 : 1;

            // Tenta com k cores, começando de kInicial
            for (int k = kInicial; k <= n; k++)
            {
                // Verifica timeout
                if (sw.Elapsed.TotalSeconds > timeoutSegundos)
                {
                    sw.Stop();
                    Console.WriteLine($"\n   TIMEOUT! Força bruta cancelado após {timeoutSegundos} segundos.");
                    Console.WriteLine($"   O algoritmo estava testando com {k} cores e não terminou.");
                    Console.WriteLine($"   Continuando com as heurísticas...\n");
                    return (0, null, sw.Elapsed.TotalMilliseconds);
                }

                var resultado = TestarCombinacoes(grafo, n, k);
                if (resultado != null)
                {
                    melhorColoracao = resultado;
                    numCores = k;
                    break;
                }
            }

            sw.Stop();
            return (numCores, melhorColoracao ?? new int[n], sw.Elapsed.TotalMilliseconds);
        }

        private static int[] TestarCombinacoes(Grafo grafo, int n, int k)
        {
            int[] cores = new int[n];
            return TestarCombinacoesRecursivo(grafo, cores, 0, k) ? cores : null;
        }

        private static bool TestarCombinacoesRecursivo(Grafo grafo, int[] cores, int vertice, int k)
        {
            if (vertice == grafo.NumeroVertices)
            {
                return true; // Todas as cores foram atribuídas validamente
            }

            for (int cor = 1; cor <= k; cor++)
            {
                // Verifica se a cor é válida ANTES de continuar a recursão
                if (EhCorValida(grafo, vertice, cor, cores))
                {
                    cores[vertice] = cor;
                    if (TestarCombinacoesRecursivo(grafo, cores, vertice + 1, k))
                        return true;
                    cores[vertice] = 0; // Backtrack
                }
            }

            return false;
        }

        private static bool EhCorValida(Grafo grafo, int vertice, int cor, int[] cores)
        {
            var vizinhos = grafo.RetornarVizinhos(vertice);
            foreach (var u in vizinhos)
            {
                if (cores[u] == cor)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Coloração Sequencial Simples - sem ordem específica
        /// </summary>
        public static (int numCores, int[] cores, double tempoMs) ColoracaoSequencial(this Grafo grafo)
        {
            var sw = Stopwatch.StartNew();
            int n = grafo.NumeroVertices;
            int[] cores = new int[n];

            if (n == 0)
            {
                sw.Stop();
                return (0, cores, sw.Elapsed.TotalMilliseconds);
            }

            for (int v = 0; v < n; v++)
            {
                var coresVizinhos = new HashSet<int>();
                var vizinhos = grafo.RetornarVizinhos(v);

                foreach (var u in vizinhos)
                {
                    if (cores[u] != 0)
                        coresVizinhos.Add(cores[u]);
                }

                // Encontra a menor cor disponível
                int cor = 1;
                while (coresVizinhos.Contains(cor))
                    cor++;

                cores[v] = cor;
            }

            int numCores = cores.Length > 0 ? cores.Max() : 0;
            sw.Stop();
            return (numCores, cores, sw.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Coloração Welsh-Powell - ordena por grau decrescente
        /// </summary>
        public static (int numCores, int[] cores, double tempoMs) ColoracaoWelshPowell(this Grafo grafo)
        {
            var sw = Stopwatch.StartNew();
            int n = grafo.NumeroVertices;
            int[] cores = new int[n];

            if (n == 0)
            {
                sw.Stop();
                return (0, cores, sw.Elapsed.TotalMilliseconds);
            }

            // Criar lista de vértices com seus graus
            var verticesOrdenados = new List<(int vertice, int grau)>();
            for (int v = 0; v < n; v++)
            {
                verticesOrdenados.Add((v, grafo.GrauVertice(v)));
            }

            // Ordenar por grau decrescente (e por índice como desempate para consistência)
            verticesOrdenados.Sort((a, b) =>
            {
                int cmp = b.grau.CompareTo(a.grau);
                return cmp != 0 ? cmp : a.vertice.CompareTo(b.vertice);
            });

            // Colorir vértices na ordem
            foreach (var (v, _) in verticesOrdenados)
            {
                var coresVizinhos = new HashSet<int>();
                var vizinhos = grafo.RetornarVizinhos(v);

                foreach (var u in vizinhos)
                {
                    if (cores[u] != 0)
                        coresVizinhos.Add(cores[u]);
                }

                // Encontra a menor cor disponível
                int cor = 1;
                while (coresVizinhos.Contains(cor))
                    cor++;

                cores[v] = cor;
            }

            int numCores = cores.Max();
            sw.Stop();
            return (numCores, cores, sw.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Coloração DSATUR - grau de saturação
        /// </summary>
        public static (int numCores, int[] cores, double tempoMs) ColoracaoDSATUR(this Grafo grafo)
        {
            var sw = Stopwatch.StartNew();
            int n = grafo.NumeroVertices;
            int[] cores = new int[n];
            bool[] colorido = new bool[n];
            int verticesColoridos = 0;

            if (n == 0)
            {
                sw.Stop();
                return (0, cores, sw.Elapsed.TotalMilliseconds);
            }

            // Começar com o vértice de maior grau
            int primeiroVertice = 0;
            int maiorGrau = -1;
            for (int v = 0; v < n; v++)
            {
                int grau = grafo.GrauVertice(v);
                if (grau > maiorGrau)
                {
                    maiorGrau = grau;
                    primeiroVertice = v;
                }
            }

            cores[primeiroVertice] = 1;
            colorido[primeiroVertice] = true;
            verticesColoridos++;

            while (verticesColoridos < n)
            {
                int proximoVertice = -1;
                int maiorSaturacao = -1;
                int maiorGrauDesempate = -1;

                // Encontrar vértice não colorido com maior grau de saturação
                for (int v = 0; v < n; v++)
                {
                    if (colorido[v]) continue;

                    int saturacao = CalcularGrauSaturacao(grafo, v, cores);
                    int grau = grafo.GrauVertice(v);

                    if (saturacao > maiorSaturacao ||
                        (saturacao == maiorSaturacao && grau > maiorGrauDesempate))
                    {
                        maiorSaturacao = saturacao;
                        maiorGrauDesempate = grau;
                        proximoVertice = v;
                    }
                }

                if (proximoVertice == -1) break;

                // Colorir o vértice escolhido
                var coresVizinhos = new HashSet<int>();
                var vizinhos = grafo.RetornarVizinhos(proximoVertice);

                foreach (var u in vizinhos)
                {
                    if (cores[u] != 0)
                        coresVizinhos.Add(cores[u]);
                }

                int cor = 1;
                while (coresVizinhos.Contains(cor))
                    cor++;

                cores[proximoVertice] = cor;
                colorido[proximoVertice] = true;
                verticesColoridos++;
            }

            int numCores = cores.Max();
            sw.Stop();
            return (numCores, cores, sw.Elapsed.TotalMilliseconds);
        }

        private static int CalcularGrauSaturacao(Grafo grafo, int vertice, int[] cores)
        {
            var coresVizinhos = new HashSet<int>();
            var vizinhos = grafo.RetornarVizinhos(vertice);

            foreach (var u in vizinhos)
            {
                if (cores[u] != 0)
                    coresVizinhos.Add(cores[u]);
            }

            return coresVizinhos.Count;
        }

        public static void ImprimeColoracao(this Grafo grafo, string nomeAlgoritmo,
            int numCores, int[] cores, double tempoMs)
        {
            Console.WriteLine($"=== {nomeAlgoritmo} ===");
            Console.WriteLine($"Tempo de execução: {tempoMs:F6} ms");

            // Verifica se houve timeout (força bruta retorna numCores = 0 e cores = null)
            if (numCores == 0 && cores == null)
            {
                Console.WriteLine("Status: TIMEOUT - Algoritmo cancelado por exceder o tempo limite");
                Console.WriteLine("Solução: Não foi possível encontrar no tempo estabelecido");
            }
            else
            {
                Console.WriteLine($"Número de cores utilizadas: {numCores}");

                if (grafo.NumeroVertices < 10 && cores != null && cores.Length > 0)
                {
                    Console.WriteLine("\nColoração dos vértices:");
                    for (int v = 0; v < grafo.NumeroVertices; v++)
                    {
                        Console.WriteLine($"  Vértice {v} ({grafo.LabelVertice(v)}): Cor {cores[v]}");
                    }
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Algoritmo de Prim para Árvore Geradora Mínima
        /// </summary>
        public static (List<(int origem, int destino, float peso)> arestas, float pesoTotal, double tempoMs)
            Prim(this Grafo grafo)
        {
            var sw = Stopwatch.StartNew();

            if (grafo.NumeroVertices == 0)
            {
                sw.Stop();
                return (new List<(int, int, float)>(), 0, sw.Elapsed.TotalMilliseconds);
            }

            // Aceita grafos não-ponderados tratando peso como 1
            if (grafo.Direcionado)
            {
                sw.Stop();
                throw new InvalidOperationException("Algoritmo de Prim requer grafo não-direcionado");
            }

            int n = grafo.NumeroVertices;
            bool[] naMST = new bool[n];
            float[] chave = new float[n];
            int[] pai = new int[n];

            // Inicialização
            for (int i = 0; i < n; i++)
            {
                chave[i] = float.MaxValue;
                pai[i] = -1;
            }

            chave[0] = 0; // Começa do vértice 0

            for (int count = 0; count < n; count++)
            {
                // Encontra vértice com menor chave que não está na MST
                int u = -1;
                float menorChave = float.MaxValue;

                for (int v = 0; v < n; v++)
                {
                    if (!naMST[v] && chave[v] < menorChave)
                    {
                        menorChave = chave[v];
                        u = v;
                    }
                }

                if (u == -1 || menorChave == float.MaxValue) break;

                naMST[u] = true;

                // Atualiza chaves dos vizinhos
                var vizinhos = grafo.RetornarVizinhos(u);
                foreach (var v in vizinhos)
                {
                    if (!naMST[v] && grafo.ExisteAresta(u, v))
                    {
                        float peso = grafo.PesoAresta(u, v);
                        if (peso < chave[v])
                        {
                            chave[v] = peso;
                            pai[v] = u;
                        }
                    }
                }
            }

            // Construir lista de arestas da MST
            var arestas = new List<(int origem, int destino, float peso)>();
            float pesoTotal = 0;

            for (int v = 0; v < n; v++)
            {
                if (pai[v] != -1)
                {
                    float peso = grafo.PesoAresta(pai[v], v);
                    arestas.Add((pai[v], v, peso));
                    pesoTotal += peso;
                }
            }

            sw.Stop();
            return (arestas, pesoTotal, sw.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Algoritmo de Kruskal para Árvore Geradora Mínima
        /// </summary>
        public static (List<(int origem, int destino, float peso)> arestas, float pesoTotal, double tempoMs)
            Kruskal(this Grafo grafo)
        {
            var sw = Stopwatch.StartNew();

            if (grafo.NumeroVertices == 0)
            {
                sw.Stop();
                return (new List<(int, int, float)>(), 0, sw.Elapsed.TotalMilliseconds);
            }

            // Aceita grafos não-ponderados tratando peso como 1
            if (grafo.Direcionado)
            {
                sw.Stop();
                throw new InvalidOperationException("Algoritmo de Kruskal requer grafo não-direcionado");
            }

            int n = grafo.NumeroVertices;

            // Coletar todas as arestas
            var todasArestas = new List<(int origem, int destino, float peso)>();
            for (int u = 0; u < n; u++)
            {
                var vizinhos = grafo.RetornarVizinhos(u);
                foreach (var v in vizinhos)
                {
                    // Para evitar duplicatas em grafos não direcionados, u < v
                    if (u < v || grafo.Direcionado)
                    {
                        float peso = grafo.PesoAresta(u, v);
                        todasArestas.Add((u, v, peso));
                    }
                }
            }

            // Ordena arestas por peso (e por vértices como desempate para consistência)
            todasArestas.Sort((a, b) =>
            {
                int cmp = a.peso.CompareTo(b.peso);
                if (cmp != 0) return cmp;
                cmp = a.origem.CompareTo(b.origem);
                if (cmp != 0) return cmp;
                return a.destino.CompareTo(b.destino);
            });

            var unionFind = new UnionFind(n);
            var mstArestas = new List<(int origem, int destino, float peso)>();
            float pesoTotal = 0;

            foreach (var (u, v, peso) in todasArestas)
            {
                // Se u e v não estão no mesmo conjunto, adiciona a aresta
                if (unionFind.Find(u) != unionFind.Find(v))
                {
                    unionFind.Union(u, v);
                    mstArestas.Add((u, v, peso));
                    pesoTotal += peso;

                    // MST tem n-1 arestas
                    if (mstArestas.Count == n - 1)
                        break;
                }
            }

            sw.Stop();
            return (mstArestas, pesoTotal, sw.Elapsed.TotalMilliseconds);
        }

        public static void ImprimeMST(this Grafo grafo, string nomeAlgoritmo,
            List<(int origem, int destino, float peso)> arestas, float pesoTotal, double tempoMs)
        {
            Console.WriteLine($"=== {nomeAlgoritmo} ===");
            Console.WriteLine($"Tempo de execução: {tempoMs:F6} ms");
            Console.WriteLine($"Peso total da MST: {pesoTotal:0.##}");
            Console.WriteLine($"Número de arestas: {arestas.Count}");

            // Verifica se o grafo é conexo
            int n = grafo.NumeroVertices;
            if (arestas.Count < n - 1)
            {
                Console.WriteLine("AVISO: O grafo não é conexo. MST parcial gerada.");
            }

            if (grafo.NumeroVertices < 20 && arestas.Count > 0)
            {
                Console.WriteLine("\nArestas da MST:");
                foreach (var (origem, destino, peso) in arestas)
                {
                    Console.WriteLine($"  {origem}({grafo.LabelVertice(origem)}) -- " +
                                    $"{destino}({grafo.LabelVertice(destino)}) [peso: {peso:0.##}]");
                }
            }

            Console.WriteLine();
        }

        private class UnionFind
        {
            private int[] pai;
            private int[] rank;

            public UnionFind(int n)
            {
                pai = new int[n];
                rank = new int[n];
                for (int i = 0; i < n; i++)
                {
                    pai[i] = i;
                    rank[i] = 0;
                }
            }

            public int Find(int x)
            {
                if (pai[x] != x)
                    pai[x] = Find(pai[x]); // Compressão de caminho
                return pai[x];
            }

            public void Union(int x, int y)
            {
                int raizX = Find(x);
                int raizY = Find(y);

                if (raizX == raizY) return;

                // Union por rank
                if (rank[raizX] < rank[raizY])
                    pai[raizX] = raizY;
                else if (rank[raizX] > rank[raizY])
                    pai[raizY] = raizX;
                else
                {
                    pai[raizY] = raizX;
                    rank[raizX]++;
                }
            }
        }
    }
}