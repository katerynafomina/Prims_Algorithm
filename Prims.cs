using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Prims_Algorithm
{
    // Клас Prims представляє реалізацію алгоритму Пріма для знаходження мінімального кістякового дерева в графі.
    public class Prims
    {
        // Граф, над яким виконується алгоритм Пріма        Graph graph;
        Graph graph;
        // Масив, що зберігає індекси вершин, що утворюють мінімальне кістякове дерево.
        int[] vertexes;

        // Масив, що зберігає ваги ребер мінімального кістякового дерева.
        int[] weights;

        // Масив, що вказує, чи була відвідана кожна вершина.
        bool[] visited;

        // Змінні для визначення мінімального ребра.
        int minEdgeValue;
        int minEdgeIndex;

        // Конструктор класу Prims.
        public Prims(Graph graph)
        {
            // Ініціалізація об'єкта класу з переданим графом.
            this.graph = graph;

            // Створення масивів для зберігання інформації про вершини, ваги ребер та відвідані вершини.
            int size = graph.Size;
            vertexes = new int[size];
            weights = new int[size];
            visited = new bool[size];

            // Скидання даних алгоритму до початкового стану.
            Reset();
        }

        // Метод скидає дані алгоритму до початкового стану.
        public void Reset()
        {
            int size = graph.Size;
            for (int i = 0; i < size; i++)
            {
                vertexes[i] = -1;
                weights[i] = int.MaxValue;
                visited[i] = false;
            }
            ResetIndex();
        }

        // Метод скидає значення індексу мінімального ребра до початкового стану.
        public void ResetIndex()
        {
            minEdgeValue = int.MaxValue;
            minEdgeIndex = -1;
        }

        // Властивість, що повертає масив вершин, які утворюють мінімальне кістякове дерево.
        public int[] Path => vertexes;

        // Властивість, що повертає масив ваг ребер мінімального кістякового дерева.
        public int[] Weights => weights;

        // Метод знаходить наступне ребро для включення в мінімальне кістякове дерево в заданому діапазоні вершин.
        public void SearchingNextEdge(int from, int to)
        {
            for (int i = 0; i < graph.Size; i++)
            {
                for (int j = from; j < to; j++)
                {
                    // Перевіряємо, чи вершина не відвідана і вага ребра менше поточної мінімальної ваги.
                    if (!visited[j] && graph[i, j] < minEdgeValue)
                    {
                        minEdgeValue = graph[i, j];
                        minEdgeIndex = j;
                    }
                }
            }
        }

        // Метод запускає алгоритм Пріма для знаходження мінімального кістякового дерева з заданою стартовою вершиною.
        public void Run(int startVertex)
        {
            Reset();
            int nextVertexIndex = 1;
            weights[0] = 0;
            vertexes[0] = startVertex;
            visited[startVertex] = true;

            // Продовжуємо вибір вершин та ребер, поки не відвідані всі вершини графа.
            while (nextVertexIndex < graph.Size)
            {
                SearchingNextEdge(0, graph.Size);
                visited[minEdgeIndex] = true;
                weights[nextVertexIndex] = minEdgeValue;
                vertexes[nextVertexIndex] = minEdgeIndex;
                nextVertexIndex++;
                ResetIndex();
            }
        }

        // Метод запускає алгоритм Пріма для знаходження мінімального кістякового дерева з використанням паралельних потоків.
        public void Run(int startVertex, int threadAmount)
        {
            Reset();
            int nextVertexIndex = 1;
            weights[0] = 0;
            vertexes[0] = startVertex;
            visited[startVertex] = true;

            List<int> minEdgeValues = new List<int>(threadAmount);
            List<int> minEdgeIndices = new List<int>(threadAmount);

            // Продовжуємо вибір вершин та ребер, поки не відвідані всі вершини графа.
            while (nextVertexIndex < graph.Size)
            {
                minEdgeValues.Clear();
                minEdgeIndices.Clear();

                // Використовуємо паралельний цикл для розділення роботи між потоками.
                Parallel.ForEach(Partitioner.Create(0, threadAmount), range =>
                {
                    int from = range.Item1 * (graph.Size / threadAmount);
                    int to = range.Item2 == threadAmount - 1 ? graph.Size : from + (graph.Size / threadAmount);

                    int localMinEdgeValue = int.MaxValue;
                    int localMinEdgeIndex = -1;

                    // Знаходимо мінімальне ребро в своєму діапазоні.
                    for (int i = 0; i < graph.Size; i++)
                    {
                        for (int j = from; j < to; j++)
                        {
                            if (!visited[j] && graph[i, j] < localMinEdgeValue)
                            {
                                localMinEdgeValue = graph[i, j];
                                localMinEdgeIndex = j;
                            }
                        }
                    }

                    // Додаємо значення мінімального ребра та його індекс до відповідних списків.
                    lock (minEdgeValues)
                    {
                        minEdgeValues.Add(localMinEdgeValue);
                    }
                    lock (minEdgeIndices)
                    {
                        minEdgeIndices.Add(localMinEdgeIndex);
                    }
                });

                // Знаходимо глобальне мінімальне ребро та його індекс.
                int globalMinEdgeValue = minEdgeValues.Min();
                int globalMinEdgeIndex = minEdgeIndices[minEdgeValues.IndexOf(globalMinEdgeValue)];

                // Відзначаємо вершину як відвідану, оновлюємо вагу ребра та індекс вершини.
                visited[globalMinEdgeIndex] = true;
                weights[nextVertexIndex] = globalMinEdgeValue;
                vertexes[nextVertexIndex] = globalMinEdgeIndex;
                nextVertexIndex++;
            }
        }
    }
}
