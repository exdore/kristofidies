using System;
using System.Collections.Generic;

namespace kristofidies
{
    public class Edge
    {
        public int Start, End;
        public double Weight;

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Start, End, Weight);
        }
    }

    public class EdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge x, Edge y)
        {
            return x.Start == y.Start && x.End == y.End && x.Weight == y.Weight;
        }

        public int GetHashCode(Edge obj)
        {
            return obj.Weight.GetHashCode() + obj.Start.GetHashCode() + obj.End.GetHashCode();
        }
    }
}
