using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafos
{
    public class GrafoLista : Grafo
    {
        public class Aresta
        {
            public int Destino { get; set; }
            public float Peso { get; set; }

            public override string ToString()
            {
                return $"-> {Destino} (peso: {Peso})";
            }
        }

        private readonly List<List<Aresta>> _adj = new();

        public GrafoLista(bool direcionado, bool ponderado)
            : base(direcionado, ponderado) { }

        // Construtor para carregar de arquivo
        public GrafoLista(string caminhoArquivo) : base(caminhoArquivo) { }

        public override bool InserirVertice(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return false;
            if (IndiceDoVertice(label) != -1) return false; // evita duplicados

            _labels.Add(label);
            _adj.Add(new List<Aresta>());
            return true;
        }

        public override bool RemoverVertice(int indice)
        {
            ValidarIndice(indice);

            // Remove todas as arestas que apontam para o vértice a ser removido
            for (int i = 0; i < _adj.Count; i++)
            {
                if (i != indice)
                {
                    _adj[i].RemoveAll(a => a.Destino == indice);
                }
            }

            // Remove a lista de adjacência do vértice
            _adj.RemoveAt(indice);
            _labels.RemoveAt(indice);

            // Ajusta os índices dos destinos > indice (decrementa em 1)
            for (int i = 0; i < _adj.Count; i++)
            {
                foreach (var aresta in _adj[i])
                {
                    if (aresta.Destino > indice)
                        aresta.Destino--;
                }
            }

            return true;
        }

        public override bool InserirAresta(int origem, int destino, float peso = 1)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            if (!Ponderado) peso = 1;

            // Evita duplicatas simples (opcional)
            if (!_adj[origem].Any(a => a.Destino == destino))
                _adj[origem].Add(new Aresta { Destino = destino, Peso = peso });
            else
            {
                // Se já existe, atualiza peso
                _adj[origem].First(a => a.Destino == destino).Peso = peso;
            }

            if (!Direcionado)
            {
                if (!_adj[destino].Any(a => a.Destino == origem))
                    _adj[destino].Add(new Aresta { Destino = origem, Peso = peso });
                else
                {
                    _adj[destino].First(a => a.Destino == origem).Peso = peso;
                }
            }

            return true;
        }

        public override bool RemoverAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);

            bool removed = _adj[origem].RemoveAll(a => a.Destino == destino) > 0;
            if (!Direcionado)
                removed |= _adj[destino].RemoveAll(a => a.Destino == origem) > 0;

            return removed;
        }

        public override bool ExisteAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            return _adj[origem].Any(a => a.Destino == destino);
        }

        public override float PesoAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            var aresta = _adj[origem].FirstOrDefault(a => a.Destino == destino);
            if (aresta == null)
                throw new InvalidOperationException("Aresta inexistente.");
            return Ponderado ? aresta.Peso : 1f;
        }

        public override List<int> RetornarVizinhos(int vertice)
        {
            ValidarIndice(vertice);
            return _adj[vertice].Select(a => a.Destino).ToList();
        }

        public override void ImprimeGrafo()
        {
            Console.WriteLine("=== Grafo (Lista de Adjacência) ===");
            Console.WriteLine($"Direcionado: {Direcionado} | Ponderado: {Ponderado}");
            Console.WriteLine($"Número de vértices: {NumeroVertices}");

            if (NumeroVertices == 0)
            {
                Console.WriteLine("Grafo vazio.");
                Console.WriteLine();
                return;
            }

            for (int i = 0; i < NumeroVertices; i++)
            {
                Console.Write($"{i} ({LabelVertice(i)}) -> ");
                if (_adj[i].Count == 0)
                {
                    Console.WriteLine("(sem vizinhos)");
                }
                else
                {
                    var arestas = _adj[i]
                        .Select(a => Ponderado
                            ? $"{a.Destino}({LabelVertice(a.Destino)}; w={a.Peso.ToString("0.##", CultureInfo.InvariantCulture)})"
                            : $"{a.Destino}({LabelVertice(a.Destino)})");
                    Console.WriteLine(string.Join(", ", arestas));
                }
            }
            Console.WriteLine();
        }
    }
}