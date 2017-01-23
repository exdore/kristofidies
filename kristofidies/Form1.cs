using System;
using System.Collections.Generic;
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
            var size = 0;
            var dynamicValues = new List<dynamic>();
            for (int i = 0; i < (verticiesCount - size) * (verticiesCount - size - 1) / 2; i++)
            {
                dynamicValues.Add(new List<dynamic>());
            }
            size++;
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
