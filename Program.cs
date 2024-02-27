using System.Diagnostics;
namespace Prims_Algorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Розмір графу
            int graphSize = 1000;

            

            // Створюємо граф та генеруємо його
            Graph graph = new Graph(graphSize);
            graph.Generate();

            Prims prim = new Prims(graph);

            // Довільно задана початкова вершина
            int startVertex = 0;

            // Без розпаралелення
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            prim.Run(startVertex);
            stopwatch.Stop();
            Console.WriteLine($"Час виконання без розпаралелення: {stopwatch.ElapsedMilliseconds} мс");

            for (int threadCount = 2; threadCount <= 16; ++threadCount)
            {
                // З розпаралеленням
                stopwatch.Reset();
                stopwatch.Start();
                prim.Run(startVertex, threadCount);
                stopwatch.Stop();

                Console.WriteLine($"Час виконання з {threadCount} потоками: {stopwatch.ElapsedMilliseconds} мс");
            }
        }

    }
}
