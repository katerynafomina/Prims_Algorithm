using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prims_Algorithm
{
    public class Graph
    {
        List<List<int>> graph;
        int size;

        public Graph(int size)
        {
            this.size = size;
            this.graph = new List<List<int>>(size);
            for (int i = 0; i < size; i++)
            {
                graph.Add(new List<int>(size));
            }
        }

        public int this[int i, int j]
        {
            get => graph[i][j];
            set => graph[i][j] = value;
        }

        public int Size => size;

        public Graph Generate(float noPathRate = 0.35F)
        {
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (rand.NextDouble() < noPathRate || i == j)
                    {
                        graph[i].Add(int.MaxValue);
                    }
                    else
                    {
                        graph[i].Add(rand.Next(100));
                    }
                }
            }
            return this;
        }
    }
}
