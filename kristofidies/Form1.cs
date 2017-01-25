using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace kristofidies
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var sourceData = Data.ReadData();
            sourceData.CalculateDistanciesList();
            var edges = Init(sourceData);
            var verticesCount = sourceData.CitiesCount;
            var minimumSpanningTree = new List<Edge>();
            var cruscalTree = CruscalAlgorithm(edges, verticesCount);
            //AlgorithmPrima(verticesCount, edges, minimumSpanningTree);
            minimumSpanningTree = cruscalTree.ToList();
            var odd = FindOddVertices(verticesCount, minimumSpanningTree);
            if (odd.Count%2 != 0)
                MessageBox.Show(string.Format("{0}", odd.Count));
            var filteredData = new int[odd.Count, odd.Count];
            for (int i = 0; i < odd.Count; i++)
            {
                for (int j = 0; j < odd.Count; j++)
                {
                    if (i == j)
                        filteredData[i, j] = int.MaxValue;
                    else
                        filteredData[i, j] = sourceData.DistanciesList[odd[i]][odd[j]];
                }
            }
            var pm = new HungarianAlgorithm(filteredData).Run();
            var perfectMatching = new List<Edge>();
            for (int i = 0; i < pm.Length; i++)
            {
                var edge = new Edge
                {
                    Start = odd[i],
                    End = odd[pm[i]],
                    Weight = sourceData.DistanciesList[odd[i]][odd[pm[i]]]
                };
                if (perfectMatching.Exists(item => (item.Start == edge.End) && (item.End == edge.Start)))
                    continue;
                perfectMatching.Add(edge);
            }
            var eulerGraph = perfectMatching.Concat(minimumSpanningTree).ToList();
            //var eulerGraph = minimumSpanningTree.Concat(perfectMatching).Distinct(new EdgeComparer()).ToList();
            var oddEuler = FindOddVertices(verticesCount, eulerGraph);
            if (oddEuler.Count > 2)
                MessageBox.Show(@"Fail");
            var result = GetMatrix(eulerGraph, verticesCount);
            PrintMatrix(result);
            var eulerPath = FindEulerPath(eulerGraph, new Random().Next(0, verticesCount));
            var eulerPathString = "";
            foreach (var item in eulerPath)
            {
                eulerPathString += item + " ";
            }
            MessageBox.Show(eulerPathString);
            var hamiltonianPath = eulerPath.Distinct().ToList();
            var hamiltonianPathString = "";
            foreach (var item in hamiltonianPath)
            {
                hamiltonianPathString += item + " ";
            }
            MessageBox.Show(hamiltonianPathString);
            var pathLenght = GetPathLenght(hamiltonianPath, sourceData);
            MessageBox.Show(pathLenght.ToString());
        }

        private static int GetPathLenght(List<int> vertices, Data sourceData)
        {
            var cost = 0;
            for (int i = 0; i < vertices.Count - 1; i++)
                cost += sourceData.DistanciesList[i][i + 1];
            cost += sourceData.DistanciesList[vertices[0]][vertices.Last()];
            return cost;
        }
        private static List<Edge> CruscalAlgorithm(List<Edge> edges, int verticesCount)
        {
            var sortedEdges = edges.OrderBy(item => item.Weight).ToList();
            var cruscalTree = new List<Edge>();
            var treeId = new List<int>();
            for (int i = 0; i < verticesCount; i++)
            {
                treeId.Add(i);
            }
            foreach (var edge in sortedEdges)
            {
                int start = edge.Start;
                int end = edge.End;
                if (treeId[start] != treeId[end])
                {
                    cruscalTree.Add(edge);
                    var oldId = treeId[end];
                    var newId = treeId[start];
                    for (int i = 0; i < verticesCount; i++)
                    {
                        if (treeId[i] == oldId)
                            treeId[i] = newId;
                    }
                }
            }
            return cruscalTree;
        }

        private void PrintMatrix(List<List<int>> matrix)
        {
            var strings = new List<string>();
            foreach (var row in matrix)
            {
                var currentRow = "";
                foreach (var element in row)
                {
                    currentRow += element + ", ";
                }
                strings.Add(currentRow);
            }
            File.WriteAllLines(@"matrix.txt", strings);
        }

        private List<List<int>> GetMatrix(List<Edge> edges, int verticesCount)
        {
            var matrix = new List<List<int>>();
            for (int i = 0; i < verticesCount; i++)
            {
                matrix.Add(new List<int>());
            }
            foreach (var row in matrix)
            {
                for (int i = 0; i < verticesCount; i++)
                {
                    row.Add(0);
                }
            }
            foreach (var edge in edges)
            {
                matrix[edge.Start][edge.End] = 1;
                matrix[edge.End][edge.Start] = 1;
            }
            return matrix;
        }

        private List<int> FindEulerPath(List<Edge> edges, int start)
        {
            var path = new List<int>();
            var stack = new Stack<int>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var edge = edges.Find(item => item.Start == current || item.End == current);
                if (edge != null)
                {
                    stack.Push(current == edge.Start ? edge.End : edge.Start);
                    edges.Remove(edge);
                }
                if (current == stack.Peek())
                {
                    path.Add(stack.Peek());
                    stack.Pop();
                }
            }
            return path;
        }

        private static List<int> GetVerticesPowers(int verticesCount, List<Edge> edges)
        {
            var entry = new List<int>();
            for (int i = 0; i < verticesCount; i++)
            {
                entry.Add(0);
            }
            foreach (var edge in edges)
            {
                entry[edge.Start]++;
                entry[edge.End]++;
            }
            return entry;
        }

        private static List<int> FindOddVertices(int verticesCount, List<Edge> edges)
        {
            var entry = GetVerticesPowers(verticesCount, edges);
            var odd = new List<int>();
            for (int i = 0; i < entry.Count; i++)
            {
                if (entry[i]%2 != 0)
                    odd.Add(i);
            }
            return odd;
        }

        public List<Edge> Init(Data data)
        {
            var edges = new List<Edge>();
            for (var i = 0; i < data.DistanciesList.Count; i++)
            {
                for (var j = i + 1; j < data.DistanciesList[i].Count; j++)
                {
                    edges.Add(new Edge
                    {
                        Start = i,
                        End = j,
                        Weight = data.DistanciesList[i][j]
                    });
                }
            }
            return edges;
        }

        public void AlgorithmPrima(int verticesCount, List<Edge> e, List<Edge> mst)
        {
            var notUsedE = new List<Edge>(e);
            var usedV = new List<int>();
            var notUsedV = new List<int>();
            for (var i = 0; i < verticesCount; i++)
                notUsedV.Add(i);
            var rand = new Random();
            usedV.Add(rand.Next(0, verticesCount));
            notUsedV.RemoveAt(usedV[0]);
            while (notUsedV.Count > 0)
            {
                var minE = -1;
                for (var i = 0; i < notUsedE.Count; i++)
                {
                    if ((usedV.IndexOf(notUsedE[i].Start) != -1) && (notUsedV.IndexOf(notUsedE[i].End) != -1) ||
                        (usedV.IndexOf(notUsedE[i].End) != -1) && (notUsedV.IndexOf(notUsedE[i].Start) != -1))
                    {
                        if (minE != -1)
                        {
                            if (notUsedE[i].Weight < notUsedE[minE].Weight)
                                minE = i;
                        }
                        else
                            minE = i;
                    }
                }
                if (usedV.IndexOf(notUsedE[minE].Start) != -1)
                {
                    usedV.Add(notUsedE[minE].End);
                    notUsedV.Remove(notUsedE[minE].End);
                }
                else
                {
                    usedV.Add(notUsedE[minE].Start);
                    notUsedV.Remove(notUsedE[minE].Start);
                }
                mst.Add(notUsedE[minE]);
                notUsedE.RemoveAt(minE);
            }
        }
    }
}
