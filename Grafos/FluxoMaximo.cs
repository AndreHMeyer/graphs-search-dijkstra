using System;
using System.Collections.Generic;
using System.Linq;

namespace Grafos
{
    public class FluxoMaximo
    {
        private readonly Grafo _grafoOriginal;
        private readonly int _origem;
        private readonly int _destino;

        public FluxoMaximo(Grafo grafo, int origem, int destino)
        {
            if (!grafo.Direcionado)
                throw new ArgumentException("O grafo deve ser direcionado para fluxo máximo.");
            if (!grafo.Ponderado)
                throw new ArgumentException("O grafo deve ser ponderado para fluxo máximo.");

            grafo.ValidarIndice(origem);
            grafo.ValidarIndice(destino);

            _grafoOriginal = grafo;
            _origem = origem;
            _destino = destino;
        }


        public float CalcularFluxoMaximo()
        {
            var grafoResidual = CriarGrafoResidual(_grafoOriginal);
            float fluxoMaximo = 0;
            List<int> caminho;

            // Enquanto existir um caminho aumentante da origem ao destino
            while ((caminho = EncontrarCaminhoAumentanteDFS(grafoResidual, _origem, _destino)).Count > 0)
            {
                float fluxoCaminho = float.MaxValue;
                for (int i = 0; i < caminho.Count - 1; i++)
                {
                    float cap = grafoResidual.PesoAresta(caminho[i], caminho[i + 1]);
                    fluxoCaminho = Math.Min(fluxoCaminho, cap);
                }

                // Atualiza as capacidades residuais das arestas e arestas reversas
                for (int i = 0; i < caminho.Count - 1; i++)
                {
                    int u = caminho[i];
                    int v = caminho[i + 1];

                    // Diminui a capacidade da aresta direta
                    float capAtual = grafoResidual.PesoAresta(u, v);
                    float novaCap = capAtual - fluxoCaminho;

                    // Remove aresta se capacidade ficar zero (ou muito pequena)
                    if (novaCap > 0.0001f)
                        grafoResidual.InserirAresta(u, v, novaCap);
                    else
                        grafoResidual.RemoverAresta(u, v);

                    // Aumenta a capacidade da aresta reversa
                    float capRev = grafoResidual.ExisteAresta(v, u) ? grafoResidual.PesoAresta(v, u) : 0;
                    grafoResidual.InserirAresta(v, u, capRev + fluxoCaminho);
                }

                fluxoMaximo += fluxoCaminho;
            }

            return fluxoMaximo;
        }

        private List<int> EncontrarCaminhoAumentanteDFS(Grafo grafo, int origem, int destino)
        {
            var visitado = new bool[grafo.NumeroVertices];
            var pai = Enumerable.Repeat(-1, grafo.NumeroVertices).ToArray();

            if (DFSCaminhoAumentante(grafo, origem, destino, visitado, pai))
            {
                var caminho = new List<int>();
                for (int v = destino; v != -1; v = pai[v])
                    caminho.Add(v);
                caminho.Reverse();
                return caminho;
            }

            return new List<int>();
        }

        private bool DFSCaminhoAumentante(Grafo grafo, int atual, int destino, bool[] visitado, int[] pai)
        {
            visitado[atual] = true;

            if (atual == destino)
                return true;

            foreach (var vizinho in grafo.RetornarVizinhos(atual))
            {
                float capacidade = grafo.PesoAresta(atual, vizinho);

                // Ignora arestas com capacidade zero ou muito pequena
                if (capacidade <= 0.0001f)
                    continue;

                if (!visitado[vizinho])
                {
                    pai[vizinho] = atual;
                    if (DFSCaminhoAumentante(grafo, vizinho, destino, visitado, pai))
                        return true;
                }
            }

            return false;
        }

        private Grafo CriarCopiaGrafo(Grafo grafoOriginal)
        {
            Grafo copia = new GrafoLista(true, true);

            for (int i = 0; i < grafoOriginal.NumeroVertices; i++)
                copia.InserirVertice(grafoOriginal.LabelVertice(i));

            for (int i = 0; i < grafoOriginal.NumeroVertices; i++)
            {
                foreach (var vizinho in grafoOriginal.RetornarVizinhos(i))
                {
                    copia.InserirAresta(i, vizinho, grafoOriginal.PesoAresta(i, vizinho));
                }
            }

            return copia;
        }

        private Grafo CriarGrafoResidual(Grafo grafoOriginal)
        {
            return CriarCopiaGrafo(grafoOriginal);
        }

        public (float fluxoInicial, float fluxoFinal, int passos, List<(int, int)> arestasInvertidas) BuscaLocal()
        {
            float fluxoInicial = CalcularFluxoMaximo();

            Grafo solucaoAtual = CriarCopiaGrafo(_grafoOriginal);
            float fluxoAtual = fluxoInicial;

            int passos = 0;
            int melhorias = 0;
            bool melhorou = true;
            var arestasInvertidas = new List<(int, int)>();

            Console.WriteLine($"\n=== Iniciando Busca Local ===");
            Console.WriteLine($"Fluxo máximo inicial: {fluxoInicial:F2}");

            // Continua enquanto houver melhorias
            while (melhorou)
            {
                melhorou = false;
                Grafo melhorVizinho = null;
                float melhorFluxo = fluxoAtual;
                (int, int) melhorInversao = (-1, -1);

                // Obtém todas as arestas da solução atual
                var arestas = ObterTodasArestas(solucaoAtual);
                Console.WriteLine($"\nIteração {melhorias + 1}: Avaliando {arestas.Count} vizinhos...");

                foreach (var (origem, destino) in arestas)
                {
                    passos++;

                    // Cria vizinho com aresta invertida
                    var vizinho = CriarVizinhoComInversao(solucaoAtual, origem, destino);

                    // Verifica se ainda existe caminho após a inversão
                    if (!vizinho.ExisteCaminho(_origem, _destino))
                        continue;

                    // Calcula fluxo do vizinho
                    var fluxoVizinho = new FluxoMaximo(vizinho, _origem, _destino).CalcularFluxoMaximo();

                    if (fluxoVizinho > melhorFluxo + 0.0001f)
                    {
                        melhorFluxo = fluxoVizinho;
                        melhorVizinho = vizinho;
                        melhorInversao = (origem, destino);
                        melhorou = true;
                    }
                }

                if (melhorou)
                {
                    solucaoAtual = melhorVizinho;
                    fluxoAtual = melhorFluxo;
                    arestasInvertidas.Add(melhorInversao);
                    melhorias++;
                    Console.WriteLine($"✓ Melhoria encontrada: aresta ({melhorInversao.Item1} → {melhorInversao.Item2}) invertida");
                    Console.WriteLine($"  Novo fluxo: {fluxoAtual:F2}");
                }
                else
                {
                    Console.WriteLine($"✗ Nenhuma melhoria encontrada. Busca local finalizada.");
                }
            }

            return (fluxoInicial, fluxoAtual, passos, arestasInvertidas);
        }

        private List<(int, int)> ObterTodasArestas(Grafo grafo)
        {
            var arestas = new List<(int, int)>();
            for (int i = 0; i < grafo.NumeroVertices; i++)
                foreach (var vizinho in grafo.RetornarVizinhos(i))
                    arestas.Add((i, vizinho));
            return arestas;
        }

        private Grafo CriarVizinhoComInversao(Grafo grafo, int origem, int destino)
        {
            Grafo vizinho = CriarCopiaGrafo(grafo);
            float peso = vizinho.PesoAresta(origem, destino);

            vizinho.RemoverAresta(origem, destino);
            vizinho.InserirAresta(destino, origem, peso);

            return vizinho;
        }


        public void ImprimirResultado()
        {
            float fluxo = CalcularFluxoMaximo();
            Console.WriteLine($"\n╔════════════════════════════════════════╗");
            Console.WriteLine($"║         FLUXO MÁXIMO (Ford-Fulkerson)  ║");
            Console.WriteLine($"╚════════════════════════════════════════╝");
            Console.WriteLine($"Fluxo Máximo: {fluxo:F2}");
        }

        public void ImprimirResultadoComBuscaLocal()
        {
            var (fIni, fFim, passos, inv) = BuscaLocal();

            Console.WriteLine($"\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║              RESULTADOS FINAIS - BUSCA LOCAL               ║");
            Console.WriteLine($"╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"Fluxo Original (Ford-Fulkerson):  {fIni:F2}");
            Console.WriteLine($"Fluxo Otimizado (Busca Local):    {fFim:F2}");
            Console.WriteLine($"Melhoria Absoluta:                {(fFim - fIni):F2}");

            if (fIni > 0)
                Console.WriteLine($"Melhoria Percentual:              {((fFim / fIni - 1) * 100):F2}%");

            Console.WriteLine($"Número de Passos Avaliados:       {passos}");
            Console.WriteLine($"Número de Inversões Realizadas:   {inv.Count}");

            if (inv.Count > 0)
            {
                Console.WriteLine($"\n--- Arestas Invertidas ---");
                for (int i = 0; i < inv.Count; i++)
                {
                    var (u, v) = inv[i];
                    Console.WriteLine($"  {i + 1}. Aresta ({u} → {v}) invertida para ({v} → {u})");
                }
            }
            else
            {
                Console.WriteLine($"\nNenhuma inversão melhorou o fluxo. Solução inicial já é ótima local.");
            }
        }
    }
}