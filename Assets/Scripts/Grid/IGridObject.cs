namespace Grid
{
    /// <summary>
    /// Represents an Object that can occupy a Grid Tile in the Game
    /// </summary>
    public interface IGridObject
    {
        public GridPosition Position { get; }
    }
}
