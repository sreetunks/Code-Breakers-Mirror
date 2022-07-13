

public struct GridPosition
{
    public int X;
    public int Z;

    public GridPosition(int x, int z)
    {
        X = x;
        Z = z;
    }

    public override string ToString()
    {
        return $"x: {X}; z: {Z}";
    }
}
