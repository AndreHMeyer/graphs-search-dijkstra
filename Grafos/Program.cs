using System;
using System.IO;
using System.Linq;

namespace Grafos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("    PARTE 3 - COLORAÇÃO    ");
            Console.WriteLine("=====================================\n");

            ExecucaoParte3();

            Console.WriteLine("\n");
            Console.WriteLine("Pressione ENTER para continuar para a Parte 4...");
            Console.ReadLine();

            Console.WriteLine("\n=====================================");
            Console.WriteLine("    PARTE 4 - MST    ");
            Console.WriteLine("=====================================\n");

            ExecucaoParte4();

            Console.WriteLine("\n");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void ExecucaoParte3()
        {
            Console.WriteLine("ALGORITMOS DE COLORAÇÃO DE GRAFOS");
            Console.WriteLine("=" + new string('=', 50) + "\n");

            // Teste 1: slides_modificado.txt
            Console.WriteLine("1. TESTE COM GRAFO (slides_modificado.txt)");
            Console.WriteLine("=" + new string('=', 43) + "\n");

            string caminhoSlides = "ArquivosTeste\\slides_modificado.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoSlides}");
                var grafoSlidesOriginal = new GrafoLista(caminhoSlides);

                // Converter para grafo não-ponderado (coloração ignora pesos)
                var grafoSlides = ConverterParaNaoPonderado(grafoSlidesOriginal);

                if (grafoSlidesOriginal.Ponderado)
                {
                    Console.WriteLine("   [Grafo convertido de ponderado para não-ponderado]");
                }

                Console.WriteLine("\n-> Grafo carregado (para coloração):");
                grafoSlides.ImprimeGrafo();

                PausarExecucao();

                // Força Bruta
                Console.WriteLine("-> Executando FORÇA BRUTA:");
                Console.WriteLine("   (Testa todas as combinações possíveis)");
                Console.WriteLine("   (Timeout: 30 segundos - se demorar muito, pula automaticamente)");
                var (numCores1, coloracao1, tempo1) = grafoSlides.ColoracaoForcaBruta(30);
                grafoSlides.ImprimeColoracao("Coloração por Força Bruta", numCores1, coloracao1, tempo1);

                PausarExecucao();

                // Heurística Sequencial Simples
                Console.WriteLine("-> Executando HEURÍSTICA SEQUENCIAL SIMPLES:");
                Console.WriteLine("   (Sem ordem específica de vértices)");
                var (numCores2, coloracao2, tempo2) = grafoSlides.ColoracaoSequencial();
                grafoSlides.ImprimeColoracao("Coloração Sequencial Simples", numCores2, coloracao2, tempo2);

                PausarExecucao();

                // Heurística Welsh-Powell
                Console.WriteLine("-> Executando HEURÍSTICA WELSH-POWELL:");
                Console.WriteLine("   (Ordena vértices por grau decrescente)");
                var (numCores3, coloracao3, tempo3) = grafoSlides.ColoracaoWelshPowell();
                grafoSlides.ImprimeColoracao("Coloração Welsh-Powell", numCores3, coloracao3, tempo3);

                PausarExecucao();

                // Heurística DSATUR
                Console.WriteLine("-> Executando HEURÍSTICA DSATUR:");
                Console.WriteLine("   (Escolhe vértice com maior grau de saturação)");
                var (numCores4, coloracao4, tempo4) = grafoSlides.ColoracaoDSATUR();
                grafoSlides.ImprimeColoracao("Coloração DSATUR", numCores4, coloracao4, tempo4);

                PausarExecucao();

                // Comparação de resultados
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (slides_modificado.txt) ===\n");
                if (numCores1 > 0) // Se não deu timeout
                    Console.WriteLine($"  Força Bruta:     {numCores1} cores em {tempo1:F6} ms");
                else
                    Console.WriteLine($"  Força Bruta:     TIMEOUT após {tempo1:F6} ms");
                Console.WriteLine($"  Sequencial:      {numCores2} cores em {tempo2:F6} ms");
                Console.WriteLine($"  Welsh-Powell:    {numCores3} cores em {tempo3:F6} ms");
                Console.WriteLine($"  DSATUR:          {numCores4} cores em {tempo4:F6} ms");

                PausarExecucao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoSlides}: {ex.Message}\n");
                PausarExecucao();
            }

            // Teste 2: slides.txt
            Console.WriteLine("\n2. TESTE COM GRAFO (slides.txt)");
            Console.WriteLine("=" + new string('=', 32) + "\n");

            string caminhoSlidesNormal = "ArquivosTeste\\slides.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoSlidesNormal}");
                var grafoSlidesNormalOriginal = new GrafoLista(caminhoSlidesNormal);

                // Converter para grafo não-ponderado
                var grafoSlidesNormal = ConverterParaNaoPonderado(grafoSlidesNormalOriginal);

                if (grafoSlidesNormalOriginal.Ponderado)
                {
                    Console.WriteLine("   [Grafo convertido de ponderado para não-ponderado]");
                }

                Console.WriteLine("\n-> Grafo carregado:");
                grafoSlidesNormal.ImprimeGrafo();

                PausarExecucao();

                // Força Bruta
                Console.WriteLine("-> Executando FORÇA BRUTA:");
                var (numCores5, coloracao5, tempo5) = grafoSlidesNormal.ColoracaoForcaBruta();
                grafoSlidesNormal.ImprimeColoracao("Coloração por Força Bruta", numCores5, coloracao5, tempo5);

                PausarExecucao();

                // Heurística Sequencial
                Console.WriteLine("-> Executando HEURÍSTICA SEQUENCIAL:");
                var (numCores6, coloracao6, tempo6) = grafoSlidesNormal.ColoracaoSequencial();
                grafoSlidesNormal.ImprimeColoracao("Coloração Sequencial Simples", numCores6, coloracao6, tempo6);

                // Heurística Welsh-Powell
                Console.WriteLine("-> Executando HEURÍSTICA WELSH-POWELL:");
                var (numCores7, coloracao7, tempo7) = grafoSlidesNormal.ColoracaoWelshPowell();
                grafoSlidesNormal.ImprimeColoracao("Coloração Welsh-Powell", numCores7, coloracao7, tempo7);

                // Heurística DSATUR
                Console.WriteLine("-> Executando HEURÍSTICA DSATUR:");
                var (numCores8, coloracao8, tempo8) = grafoSlidesNormal.ColoracaoDSATUR();
                grafoSlidesNormal.ImprimeColoracao("Coloração DSATUR", numCores8, coloracao8, tempo8);

                PausarExecucao();

                // Comparação de resultados
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (slides.txt) ===\n");
                Console.WriteLine($"  Força Bruta:     {numCores5} cores em {tempo5:F6} ms");
                Console.WriteLine($"  Sequencial:      {numCores6} cores em {tempo6:F6} ms");
                Console.WriteLine($"  Welsh-Powell:    {numCores7} cores em {tempo7:F6} ms");
                Console.WriteLine($"  DSATUR:          {numCores8} cores em {tempo8:F6} ms");

                PausarExecucao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoSlidesNormal}: {ex.Message}\n");
                PausarExecucao();
            }

            // Teste 3: espacoaereo.txt
            Console.WriteLine("\n3. TESTE COM GRAFO (espacoaereo.txt)");
            Console.WriteLine("=" + new string('=', 37) + "\n");

            string caminhoEspaco = "ArquivosTeste\\espacoaereo.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoEspaco}");
                var grafoEspacoOriginal = new GrafoLista(caminhoEspaco);

                // Converter para grafo não-ponderado
                var grafoEspaco = ConverterParaNaoPonderado(grafoEspacoOriginal);

                if (grafoEspacoOriginal.Ponderado)
                {
                    Console.WriteLine("   [Grafo convertido de ponderado para não-ponderado]");
                }

                Console.WriteLine($"\n-> Grafo carregado: {grafoEspaco.NumeroVertices} vértices");
                Console.WriteLine("\n-> Estrutura do grafo:");
                grafoEspaco.ImprimeGrafo();

                PausarExecucao();

                // Força Bruta (AVISO: pode demorar muito!)
                Console.WriteLine("-> Executando FORÇA BRUTA:");
                Console.WriteLine("   A execução pode levar MUITO tempo ou não terminar.");
                Console.WriteLine("   Pressione Ctrl+C se quiser cancelar.");
                //var (numCores9, coloracao9, tempo9) = grafoEspaco.ColoracaoForcaBruta();
                //grafoEspaco.ImprimeColoracao("Coloração por Força Bruta", numCores9, coloracao9, tempo9);

                PausarExecucao();

                // Heurística Sequencial
                Console.WriteLine("-> Executando HEURÍSTICA SEQUENCIAL:");
                var (numCores10, coloracao10, tempo10) = grafoEspaco.ColoracaoSequencial();
                grafoEspaco.ImprimeColoracao("Coloração Sequencial Simples", numCores10, coloracao10, tempo10);

                // Heurística Welsh-Powell
                Console.WriteLine("-> Executando HEURÍSTICA WELSH-POWELL:");
                var (numCores11, coloracao11, tempo11) = grafoEspaco.ColoracaoWelshPowell();
                grafoEspaco.ImprimeColoracao("Coloração Welsh-Powell", numCores11, coloracao11, tempo11);

                // Heurística DSATUR
                Console.WriteLine("-> Executando HEURÍSTICA DSATUR:");
                var (numCores12, coloracao12, tempo12) = grafoEspaco.ColoracaoDSATUR();
                grafoEspaco.ImprimeColoracao("Coloração DSATUR", numCores12, coloracao12, tempo12);

                PausarExecucao();

                // Comparação final
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (espacoaereo.txt) ===\n");
                //Console.WriteLine($"  Força Bruta:     {numCores9} cores em {tempo9:F6} ms");
                Console.WriteLine($"  Sequencial:      {numCores10} cores em {tempo10:F6} ms");
                Console.WriteLine($"  Welsh-Powell:    {numCores11} cores em {tempo11:F6} ms");
                Console.WriteLine($"  DSATUR:          {numCores12} cores em {tempo12:F6} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoEspaco}: {ex.Message}\n");
            }
        }

        static void ExecucaoParte4()
        {
            Console.WriteLine("ALGORITMOS DE ÁRVORE GERADORA MÍNIMA");
            Console.WriteLine("=" + new string('=', 50) + "\n");

            // Teste 1: slides_modificado.txt
            Console.WriteLine("1. TESTE COM GRAFO (slides_modificado.txt)");
            Console.WriteLine("=" + new string('=', 43) + "\n");

            string caminhoSlides = "ArquivosTeste\\slides_modificado.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoSlides}");
                var grafoSlides = new GrafoLista(caminhoSlides);

                Console.WriteLine("\n-> Grafo carregado:");
                grafoSlides.ImprimeGrafo();

                PausarExecucao();

                // Algoritmo de Prim
                Console.WriteLine("-> Executando ALGORITMO DE PRIM:");
                Console.WriteLine("   (Constrói MST a partir de um vértice inicial)");
                var (arestas1, peso1, tempo1) = grafoSlides.Prim();
                grafoSlides.ImprimeMST("Algoritmo de Prim", arestas1, peso1, tempo1);

                PausarExecucao();

                // Algoritmo de Kruskal
                Console.WriteLine("-> Executando ALGORITMO DE KRUSKAL:");
                Console.WriteLine("   (Ordena arestas e usa Union-Find)");
                var (arestas2, peso2, tempo2) = grafoSlides.Kruskal();
                grafoSlides.ImprimeMST("Algoritmo de Kruskal", arestas2, peso2, tempo2);

                PausarExecucao();

                // Comparação
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (slides_modificado.txt) ===\n");
                Console.WriteLine($"  Prim:     Peso = {peso1:0.##}, Tempo = {tempo1:F6} ms, Arestas = {arestas1.Count}");
                Console.WriteLine($"  Kruskal:  Peso = {peso2:0.##}, Tempo = {tempo2:F6} ms, Arestas = {arestas2.Count}");

                PausarExecucao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoSlides}: {ex.Message}\n");
                PausarExecucao();
            }

            // Teste 2: slides.txt
            Console.WriteLine("\n2. TESTE COM GRAFO (slides.txt)");
            Console.WriteLine("=" + new string('=', 32) + "\n");

            string caminhoSlidesNormal = "ArquivosTeste\\slides.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoSlidesNormal}");
                var grafoSlidesNormal = new GrafoLista(caminhoSlidesNormal);

                Console.WriteLine("\n-> Grafo carregado:");
                grafoSlidesNormal.ImprimeGrafo();

                PausarExecucao();

                Console.WriteLine("-> Executando PRIM:");
                var (arestas3, peso3, tempo3) = grafoSlidesNormal.Prim();
                grafoSlidesNormal.ImprimeMST("Algoritmo de Prim", arestas3, peso3, tempo3);

                Console.WriteLine("-> Executando KRUSKAL:");
                var (arestas4, peso4, tempo4) = grafoSlidesNormal.Kruskal();
                grafoSlidesNormal.ImprimeMST("Algoritmo de Kruskal", arestas4, peso4, tempo4);

                PausarExecucao();

                // Comparação
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (slides.txt) ===\n");
                Console.WriteLine($"  Prim:     Peso = {peso3:0.##}, Tempo = {tempo3:F6} ms, Arestas = {arestas3.Count}");
                Console.WriteLine($"  Kruskal:  Peso = {peso4:0.##}, Tempo = {tempo4:F6} ms, Arestas = {arestas4.Count}");

                PausarExecucao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoSlidesNormal}: {ex.Message}\n");
                PausarExecucao();
            }

            // Teste 3: espacoaereo.txt
            Console.WriteLine("\n3. TESTE COM GRAFO (espacoaereo.txt)");
            Console.WriteLine("=" + new string('=', 37) + "\n");

            string caminhoEspaco = "ArquivosTeste\\espacoaereo.txt";

            try
            {
                Console.WriteLine($"-> Carregando grafo de: {caminhoEspaco}");
                var grafoEspaco = new GrafoLista(caminhoEspaco);

                Console.WriteLine($"\n-> Grafo carregado: {grafoEspaco.NumeroVertices} vértices");
                Console.WriteLine("\n-> Estrutura do grafo:");
                grafoEspaco.ImprimeGrafo();

                PausarExecucao();

                Console.WriteLine("-> Executando PRIM:");
                var (arestas5, peso5, tempo5) = grafoEspaco.Prim();
                grafoEspaco.ImprimeMST("Algoritmo de Prim", arestas5, peso5, tempo5);

                Console.WriteLine("-> Executando KRUSKAL:");
                var (arestas6, peso6, tempo6) = grafoEspaco.Kruskal();
                grafoEspaco.ImprimeMST("Algoritmo de Kruskal", arestas6, peso6, tempo6);

                PausarExecucao();

                // Comparação final
                Console.WriteLine("\n=== COMPARAÇÃO DOS RESULTADOS (espacoaereo.txt) ===\n");
                Console.WriteLine($"  Prim:     Peso = {peso5:0.##}, Tempo = {tempo5:F6} ms, Arestas = {arestas5.Count}");
                Console.WriteLine($"  Kruskal:  Peso = {peso6:0.##}, Tempo = {tempo6:F6} ms, Arestas = {arestas6.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao processar {caminhoEspaco}: {ex.Message}\n");
            }
        }


        /// <summary>
        /// Converte um grafo ponderado em não-ponderado para coloração.
        /// Se o grafo já for não-ponderado, retorna ele mesmo sem conversão.
        /// </summary>
        static GrafoLista ConverterParaNaoPonderado(Grafo grafoOriginal)
        {
            // Se já é não-ponderado, não precisa converter
            if (!grafoOriginal.Ponderado)
            {
                return (GrafoLista)grafoOriginal;
            }

            // Se é ponderado, converte para não-ponderado
            var grafoNovo = new GrafoLista(grafoOriginal.Direcionado, false);

            // Copiar vértices
            for (int i = 0; i < grafoOriginal.NumeroVertices; i++)
            {
                grafoNovo.InserirVertice(grafoOriginal.LabelVertice(i));
            }

            // Copiar arestas (sem peso)
            for (int i = 0; i < grafoOriginal.NumeroVertices; i++)
            {
                var vizinhos = grafoOriginal.RetornarVizinhos(i);
                foreach (var j in vizinhos)
                {
                    // Para grafos não-direcionados, adicionar apenas se i < j para evitar duplicatas
                    if (grafoOriginal.Direcionado || i < j)
                    {
                        grafoNovo.InserirAresta(i, j);
                    }
                }
            }

            return grafoNovo;
        }

        static void PausarExecucao()
        {
            Console.WriteLine("\n" + new string('-', 50));
            Console.WriteLine("[Pressione ENTER para continuar...]");
            Console.ReadLine();
        }
    }
}