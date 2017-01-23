using System;

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
}
