using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafos
{
    public abstract class Grafo
    {
        protected readonly bool Direcionado;
        protected readonly bool Ponderado;
        // Armazena os rótulos (labels) dos vértices pelo índice
        protected readonly List<string> _labels = new();

        protected Grafo(bool direcionado, bool ponderado)
        {
            Direcionado = direcionado;
            Ponderado = ponderado;
        }

        // Construtor para carregar grafo de arquivo
        protected Grafo(string caminhoArquivo)
        {
            var dados = LerArquivoGrafo(caminhoArquivo);
            Direcionado = dados.direcionado;
            Ponderado = dados.ponderado;

            // Criar vértices
            for (int i = 0; i < dados.numVertices; i++)
            {
                InserirVertice($"V{i}");
            }

            // Criar arestas
            foreach (var aresta in dados.arestas)
            {
                InserirAresta(aresta.origem, aresta.destino, aresta.peso);
            }
        }

        public int NumeroVertices => _labels.Count;

        // ---------- Métodos abstratos (implementados pelas classes filhas) ----------
        public abstract bool InserirVertice(string label);
        public abstract bool RemoverVertice(int indice);
        public abstract bool InserirAresta(int origem, int destino, float peso = 1);
        public abstract bool RemoverAresta(int origem, int destino);
        public abstract bool ExisteAresta(int origem, int destino);
        public abstract float PesoAresta(int origem, int destino);
        public abstract List<int> RetornarVizinhos(int vertice);
        public abstract void ImprimeGrafo();

        // ---------- Métodos de úteis ----------
        public string LabelVertice(int indice)
        {
            ValidarIndice(indice);
            return _labels[indice];
        }

        protected void ValidarIndice(int i)
        {
            if (i < 0 || i >= NumeroVertices)
                throw new ArgumentOutOfRangeException(nameof(i), $"Índice de vértice inválido: {i}");
        }

        public int IndiceDoVertice(string label)
        {
            if (label == null) return -1;
            return _labels.FindIndex(s => string.Equals(s, label, StringComparison.Ordinal));
        }

        // ---------- Leitura de Arquivo ----------
        private static (int numVertices, int numArestas, bool direcionado, bool ponderado, List<(int origem, int destino, float peso)> arestas)
            LerArquivoGrafo(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");

            var linhas = File.ReadAllLines(caminhoArquivo)
                .Select(linha => linha.Trim())
                .Where(linha => !string.IsNullOrWhiteSpace(linha)) 
                .ToArray();

            if (linhas.Length == 0)
                throw new InvalidOperationException("Arquivo vazio");

            // Primeira linha: V A D P
            var cabecalho = linhas[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (cabecalho.Length != 4)
                throw new InvalidOperationException("Formato de cabeçalho inválido. Esperado: V A D P");

            var numVertices = int.Parse(cabecalho[0]);
            var numArestas = int.Parse(cabecalho[1]);
            var direcionado = cabecalho[2] == "1";
            var ponderado = cabecalho[3] == "1";

            var arestas = new List<(int origem, int destino, float peso)>();

            // Ler arestas
            for (int i = 1; i <= numArestas && i < linhas.Length; i++)
            {
                var partes = linhas[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (ponderado && partes.Length != 3)
                    throw new InvalidOperationException($"Linha {i}: Esperado formato 'origem destino peso' para grafo ponderado. Encontrado: '{linhas[i]}'");
                if (!ponderado && partes.Length != 2)
                    throw new InvalidOperationException($"Linha {i}: Esperado formato 'origem destino' para grafo não ponderado. Encontrado: '{linhas[i]}'");

                var origem = int.Parse(partes[0]);
                var destino = int.Parse(partes[1]);
                var peso = ponderado && partes.Length > 2 ? float.Parse(partes[2], CultureInfo.InvariantCulture) : 1f;

                arestas.Add((origem, destino, peso));
            }

            return (numVertices, numArestas, direcionado, ponderado, arestas);
        }

        // ---------- Métodos de Grau ----------
        // Grau de um vértice (número de arestas conectadas)
        public int GrauVertice(int vertice)
        {
            ValidarIndice(vertice);
            return RetornarVizinhos(vertice).Count;
        }

        // Para grafos direcionados: grau de entrada
        public int GrauEntrada(int vertice)
        {
            if (!Direcionado)
                return GrauVertice(vertice); // Em grafo não-direcionado, grau entrada = grau total

            ValidarIndice(vertice);
            int grauEntrada = 0;

            for (int i = 0; i < NumeroVertices; i++)
            {
                if (i != vertice && ExisteAresta(i, vertice))
                    grauEntrada++;
            }

            return grauEntrada;
        }

        // Para grafos direcionados: grau de saída  
        public int GrauSaida(int vertice)
        {
            return GrauVertice(vertice); // Grau de saída é o número de vizinhos
        }

        // ---------- Algoritmos de Busca (Classe Grafo) ----------

        // BFS: varre o grafo a partir de 'origem' e retorna a ordem de visita
        public List<int> BuscaEmLargura(int origem)
        {
            ValidarIndice(origem);
            var visitado = new bool[NumeroVertices];
            var ordem = new List<int>();
            var fila = new Queue<int>();

            visitado[origem] = true;
            fila.Enqueue(origem);

            while (fila.Count > 0)
            {
                int v = fila.Dequeue();
                ordem.Add(v);

                foreach (var w in RetornarVizinhos(v))
                {
                    if (!visitado[w])
                    {
                        visitado[w] = true;
                        fila.Enqueue(w);
                    }
                }
            }

            return ordem;
        }

        public void ImprimeBuscaEmLargura(int origem)
        {
            Console.WriteLine($"=== Busca em Largura a partir do vértice {origem} ({LabelVertice(origem)}) ===");

            var ordem = BuscaEmLargura(origem);

            Console.Write("Ordem de visitação: ");
            for (int i = 0; i < ordem.Count; i++)
            {
                if (i > 0) Console.Write(" -> ");
                Console.Write($"{ordem[i]}({LabelVertice(ordem[i])})");
            }
            Console.WriteLine();

            if (ordem.Count < NumeroVertices)
            {
                Console.WriteLine($"Alcançados: {ordem.Count}/{NumeroVertices} vértices");
            }
            Console.WriteLine();
        }

        // DFS: varre o grafo a partir de 'origem' e retorna a ordem de visita
        public List<int> BuscaEmProfundidade(int origem)
        {
            ValidarIndice(origem);
            var visitado = new bool[NumeroVertices];
            var ordem = new List<int>();
            DfsRecursivo(origem, visitado, ordem);
            return ordem;
        }

        public void ImprimeBuscaEmProfundidade(int origem)
        {
            Console.WriteLine($"=== Busca em Profundidade a partir do vértice {origem} ({LabelVertice(origem)}) ===");

            var ordem = BuscaEmProfundidade(origem);

            Console.Write("Ordem de visitação: ");
            for (int i = 0; i < ordem.Count; i++)
            {
                if (i > 0) Console.Write(" -> ");
                Console.Write($"{ordem[i]}({LabelVertice(ordem[i])})");
            }
            Console.WriteLine();

            if (ordem.Count < NumeroVertices)
            {
                Console.WriteLine($"Alcançados: {ordem.Count}/{NumeroVertices} vértices");
            }
            Console.WriteLine();
        }

        private void DfsRecursivo(int v, bool[] visitado, List<int> ordem)
        {
            visitado[v] = true;
            ordem.Add(v);

            foreach (var w in RetornarVizinhos(v))
            {
                if (!visitado[w])
                {
                    DfsRecursivo(w, visitado, ordem);
                }
            }
        }

        // BFS para encontrar um caminho (se existir) entre origem e destino
        public List<int> BuscaCaminho(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);

            var visitado = new bool[NumeroVertices];
            var pai = Enumerable.Repeat(-1, NumeroVertices).ToArray();
            var fila = new Queue<int>();

            visitado[origem] = true;
            fila.Enqueue(origem);

            while (fila.Count > 0)
            {
                int v = fila.Dequeue();
                if (v == destino) break;

                foreach (var w in RetornarVizinhos(v))
                {
                    if (!visitado[w])
                    {
                        visitado[w] = true;
                        pai[w] = v;
                        fila.Enqueue(w);
                    }
                }
            }

            if (!visitado[destino]) return new List<int>(); // sem caminho

            // reconstrói o caminho
            var caminho = new List<int>();
            for (int at = destino; at != -1; at = pai[at])
                caminho.Add(at);
            caminho.Reverse();
            return caminho;
        }

        // ---------- Algoritmo de Dijkstra ----------
        public (float[] distancias, int[] predecessores) Dijkstra(int origem)
        {
            ValidarIndice(origem);

            if (!Ponderado)
                throw new InvalidOperationException("Dijkstra requer grafo ponderado");

            var dist = new float[NumeroVertices];
            var pred = new int[NumeroVertices];
            var visitado = new bool[NumeroVertices];

            // Inicialização
            for (int i = 0; i < NumeroVertices; i++)
            {
                dist[i] = float.MaxValue;
                pred[i] = -1;
            }
            dist[origem] = 0;

            for (int count = 0; count < NumeroVertices; count++)
            {
                // Encontra vértice com menor distância não visitado
                int u = -1;
                for (int v = 0; v < NumeroVertices; v++)
                {
                    if (!visitado[v] && (u == -1 || dist[v] < dist[u]))
                        u = v;
                }

                if (u == -1 || dist[u] == float.MaxValue)
                    break; // Não há mais vértices alcançáveis

                visitado[u] = true;

                // Atualiza distâncias dos vizinhos
                foreach (var v in RetornarVizinhos(u))
                {
                    if (!visitado[v])
                    {
                        var peso = PesoAresta(u, v);
                        if (dist[u] + peso < dist[v])
                        {
                            dist[v] = dist[u] + peso;
                            pred[v] = u;
                        }
                    }
                }
            }

            return (dist, pred);
        }

        public void ImprimeDijkstra(int origem)
        {
            Console.WriteLine($"=== Algoritmo de Dijkstra a partir do vértice {origem} ({LabelVertice(origem)}) ===");

            if (!Ponderado)
            {
                Console.WriteLine("ERRO: Dijkstra requer um grafo ponderado!");
                Console.WriteLine();
                return;
            }

            var (distancias, predecessores) = Dijkstra(origem);

            Console.WriteLine("Menores distâncias:");
            Console.WriteLine("Destino        | Distância | Caminho");
            Console.WriteLine("---------------|-----------|------------------------");

            for (int i = 0; i < NumeroVertices; i++)
            {
                if (i == origem) continue;

                var dist = distancias[i] == float.MaxValue ? "∞" : distancias[i].ToString("0.##");
                var caminho = ReconstruirCaminho(origem, i, predecessores);
                var caminhoStr = caminho.Count == 0 ? "Inalcançável" :
                    string.Join(" -> ", caminho.Select(v => $"{v}({LabelVertice(v)})"));

                Console.WriteLine($"{i,2}({LabelVertice(i),8}) | {dist,9} | {caminhoStr}");
            }
            Console.WriteLine();
        }

        private List<int> ReconstruirCaminho(int origem, int destino, int[] predecessores)
        {
            if (predecessores[destino] == -1 && origem != destino)
                return new List<int>(); // Sem caminho

            var caminho = new List<int>();
            for (int at = destino; at != -1; at = predecessores[at])
                caminho.Add(at);

            caminho.Reverse();
            return caminho.Count > 0 && caminho[0] == origem ? caminho : new List<int>();
        }

        // ---------- Métodos de conectividade ----------
        // Verifica se o grafo é conexo (para grafos não-direcionados)
        public bool EhConexo()
        {
            if (NumeroVertices == 0) return true;

            // Faz BFS a partir do vértice 0
            var visitados = BuscaEmLargura(0);

            // Se visitou todos os vértices, é conexo
            return visitados.Count == NumeroVertices;
        }

        // Verifica se existe caminho entre dois vértices
        public bool ExisteCaminho(int origem, int destino)
        {
            var caminho = BuscaCaminho(origem, destino);
            return caminho.Count > 0;
        }
    }
}