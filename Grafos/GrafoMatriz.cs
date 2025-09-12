using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafos
{
    public class GrafoMatriz : Grafo
    {
        private List<List<float?>> _matriz = new();

        public GrafoMatriz(bool direcionado, bool ponderado)
            : base(direcionado, ponderado) { }

        // Construtor para carregar de arquivo
        public GrafoMatriz(string caminhoArquivo) : base(caminhoArquivo) { }

        public override bool InserirVertice(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return false;
            if (IndiceDoVertice(label) != -1) return false; // evita duplicados

            _labels.Add(label);

            // adiciona nova coluna para cada linha existente
            foreach (var linha in _matriz)
                linha.Add(null);

            // adiciona a nova linha
            var novaLinha = new List<float?>(_labels.Count);
            for (int i = 0; i < _labels.Count; i++) novaLinha.Add(null);
            _matriz.Add(novaLinha);

            return true;
        }

        public override bool RemoverVertice(int indice)
        {
            ValidarIndice(indice);

            // remove linha
            _matriz.RemoveAt(indice);
            // remove coluna
            foreach (var linha in _matriz)
                linha.RemoveAt(indice);

            _labels.RemoveAt(indice);
            return true;
        }

        public override bool InserirAresta(int origem, int destino, float peso = 1)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            if (!Ponderado) peso = 1;

            _matriz[origem][destino] = peso;

            if (!Direcionado)
                _matriz[destino][origem] = peso;

            return true;
        }

        public override bool RemoverAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);

            bool existia = _matriz[origem][destino].HasValue;
            _matriz[origem][destino] = null;
            if (!Direcionado)
                _matriz[destino][origem] = null;

            return existia;
        }

        public override bool ExisteAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            return _matriz[origem][destino].HasValue;
        }

        public override float PesoAresta(int origem, int destino)
        {
            ValidarIndice(origem);
            ValidarIndice(destino);
            var val = _matriz[origem][destino];
            if (!val.HasValue)
                throw new InvalidOperationException("Aresta inexistente.");

            return Ponderado ? val.Value : 1f;
        }

        public override List<int> RetornarVizinhos(int vertice)
        {
            ValidarIndice(vertice);
            var viz = new List<int>();
            for (int j = 0; j < NumeroVertices; j++)
            {
                if (_matriz[vertice][j].HasValue)
                    viz.Add(j);
            }
            return viz;
        }

        public override void ImprimeGrafo()
        {
            Console.WriteLine("=== Grafo (Matriz de Adjacência) ===");
            Console.WriteLine($"Direcionado: {Direcionado} | Ponderado: {Ponderado}");
            Console.WriteLine($"Número de vértices: {NumeroVertices}");

            if (NumeroVertices == 0)
            {
                Console.WriteLine("Grafo vazio.");
                Console.WriteLine();
                return;
            }

            // cabeçalho
            Console.Write("     ");
            for (int j = 0; j < NumeroVertices; j++)
                Console.Write($"{j,4}");
            Console.WriteLine();

            for (int i = 0; i < NumeroVertices; i++)
            {
                Console.Write($"{i,3} ");
                for (int j = 0; j < NumeroVertices; j++)
                {
                    var val = _matriz[i][j];
                    string s = val.HasValue
                        ? (Ponderado ? val.Value.ToString("0.##", CultureInfo.InvariantCulture) : "1")
                        : ".";
                    Console.Write($"{s,4}");
                }
                Console.Write($"   | {LabelVertice(i)}");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}