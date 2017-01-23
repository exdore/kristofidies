using System;
using System.Collections.Generic;
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
            var e = Init(sourceData);
            var verticiesCount = sourceData.CitiesCount;
            var mst = new List<Edge>();
            AlgorithmPrima(verticiesCount, e, mst);
            var odd = FindOddVerticies(verticiesCount, mst);
            listBox1.DataSource = odd;
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
            var hung = new HungarianAlgorithm(filteredData);
            var res = hung.Run();
        }

        public int[,] To2DArray(List<List<int>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int max = source.Select(l => l).Max(l => l.Count);

            var result = new int[source.Count, max];

            for (int i = 0; i < source.Count - 1; i++)
            {
                for (int j = 0; j < source[i].Count; j++)
                {
                    if (source[i][j] == 0)
                        result[i, j] = int.MaxValue;
                    else
                        result[i, j] = source[i][j];
                }
            }

            return result;
        }

        private static List<int> FindOddVerticies(int verticiesCount, List<Edge> mst)
        {
            var entry = new List<int>();
            for (int i = 0; i < verticiesCount; i++)
            {
                entry.Add(0);
            }
            foreach (var edge in mst)
            {
                entry[edge.Start]++;
                entry[edge.End]++;
            }
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

        public void AlgorithmPrima(int verticiesCount, List<Edge> e, List<Edge> mst)
        {
            var notUsedE = new List<Edge>(e);
            var usedV = new List<int>();
            var notUsedV = new List<int>();
            for (var i = 0; i < verticiesCount; i++)
                notUsedV.Add(i);
            var rand = new Random();
            usedV.Add(rand.Next(0, verticiesCount));
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
