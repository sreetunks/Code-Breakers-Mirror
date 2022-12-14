using System;

// TODO: Overhaul this class

namespace Grid
{
    [Serializable]
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int X; // Rename to x
        public int Z; // Rename to z

        public static GridPosition Invalid { get => new GridPosition(-1, -1); } // Can be changed to be more optimized

        public GridPosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition position &&
                   X == position.X &&
                   Z == position.Z;
        }

        public bool Equals(GridPosition other)
        {
            return this == other;
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        public override string ToString()
        {
            return $"x: {X}; z: {Z}";
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.X == b.X && a.Z == b.Z;
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !(a == b);
        }
    }
}
