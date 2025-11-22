using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Grafos
{
    public abstract class Grafo
    {
        public readonly bool Direcionado;
        public readonly bool Ponderado;
        protected readonly List<string> _labels = new();

        protected Grafo(bool direcionado, bool ponderado)
        {
            Direcionado = direcionado;
            Ponderado = ponderado;
        }

        protected Grafo(string caminhoArquivo)
        {
            var dados = LerArquivoGrafo(caminhoArquivo);
            Direcionado = dados.direcionado;
            Ponderado = dados.ponderado;

            for (int i = 0; i < dados.numVertices; i++)
                InserirVertice($"V{i}");

            foreach (var aresta in dados.arestas)
                InserirAresta(aresta.origem, aresta.destino, aresta.peso);
        }

        public int NumeroVertices => _labels.Count;

        public abstract bool InserirVertice(string label);
        public abstract bool RemoverVertice(int indice);
        public abstract bool InserirAresta(int origem, int destino, float peso = 1);
        public abstract bool RemoverAresta(int origem, int destino);
        public abstract bool ExisteAresta(int origem, int destino);
        public abstract float PesoAresta(int origem, int destino);
        public abstract List<int> RetornarVizinhos(int vertice);
        public abstract void ImprimeGrafo();

        public string LabelVertice(int indice)
        {
            ValidarIndice(indice);
            return _labels[indice];
        }

        public void ValidarIndice(int i)
        {
            if (i < 0 || i >= NumeroVertices)
                throw new ArgumentOutOfRangeException(nameof(i), $"Índice de vértice inválido: {i}");
        }

        public int IndiceDoVertice(string label)
        {
            if (label == null) return -1;
            return _labels.FindIndex(s => string.Equals(s, label, StringComparison.Ordinal));
        }

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

            var cabecalho = linhas[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (cabecalho.Length != 4)
                throw new InvalidOperationException("Formato de cabeçalho inválido. Esperado: V A D P");

            var numVertices = int.Parse(cabecalho[0]);
            var numArestas = int.Parse(cabecalho[1]);
            var direcionado = cabecalho[2] == "1";
            var ponderado = cabecalho[3] == "1";

            var arestas = new List<(int origem, int destino, float peso)>();

            for (int i = 1; i <= numArestas && i < linhas.Length; i++)
            {
                var partes = linhas[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (ponderado && partes.Length != 3)
                    throw new InvalidOperationException($"Linha {i}: Esperado formato 'origem destino peso'.");
                if (!ponderado && partes.Length != 2)
                    throw new InvalidOperationException($"Linha {i}: Esperado formato 'origem destino'.");

                var origem = int.Parse(partes[0]);
                var destino = int.Parse(partes[1]);
                var peso = ponderado && partes.Length > 2 ? float.Parse(partes[2], CultureInfo.InvariantCulture) : 1f;

                arestas.Add((origem, destino, peso));
            }

            return (numVertices, numArestas, direcionado, ponderado, arestas);
        }

        public int GrauVertice(int vertice)
        {
            ValidarIndice(vertice);
            return RetornarVizinhos(vertice).Count;
        }

        public int GrauEntrada(int vertice)
        {
            if (!Direcionado) return GrauVertice(vertice);
            ValidarIndice(vertice);
            int grauEntrada = 0;
            for (int i = 0; i < NumeroVertices; i++)
            {
                if (i != vertice && ExisteAresta(i, vertice))
                    grauEntrada++;
            }
            return grauEntrada;
        }

        public int GrauSaida(int vertice) => GrauVertice(vertice);

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

        public List<int> BuscaEmProfundidade(int origem)
        {
            ValidarIndice(origem);
            var visitado = new bool[NumeroVertices];
            var ordem = new List<int>();
            DfsRecursivo(origem, visitado, ordem);
            return ordem;
        }

        private void DfsRecursivo(int v, bool[] visitado, List<int> ordem)
        {
            visitado[v] = true;
            ordem.Add(v);
            foreach (var w in RetornarVizinhos(v))
            {
                if (!visitado[w]) DfsRecursivo(w, visitado, ordem);
            }
        }

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

            if (!visitado[destino]) return new List<int>();

            var caminho = new List<int>();
            for (int at = destino; at != -1; at = pai[at]) caminho.Add(at);
            caminho.Reverse();
            return caminho;
        }

        public bool ExisteCaminho(int origem, int destino)
        {
            var caminho = BuscaCaminho(origem, destino);
            return caminho.Count > 0;
        }
    }
}