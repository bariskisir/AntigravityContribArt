namespace AntigravityContribArt;

/// <summary>
/// Utility class for grid manipulation operations such as counting lit cells
/// and creating full-size preview grids that match GitHub's contribution
/// graph layout (53 columns × 7 rows).
/// </summary>
public static class GridHelper
{
    /// <summary>
    /// Width of GitHub's contribution graph in columns (weeks).
    /// </summary>
    public const int GraphWidth = 53;

    /// <summary>
    /// Height of GitHub's contribution graph in rows (days of the week).
    /// </summary>
    public const int GraphHeight = 7;

    /// <summary>
    /// Counts the number of lit (true) cells in the pixel grid.
    /// </summary>
    /// <param name="grid">The boolean grid to count.</param>
    /// <returns>The number of cells set to <c>true</c>.</returns>
    public static int CountLitCells(bool[,] grid)
    {
        int count = 0;
        foreach (bool cell in grid)
            if (cell) count++;
        return count;
    }

    /// <summary>
    /// Creates a full 53-column × 7-row grid matching GitHub's contribution
    /// graph layout. The text grid is placed at the specified column offset
    /// so the preview shows exactly how the contribution graph will look.
    /// </summary>
    /// <param name="grid">The rendered text grid to place.</param>
    /// <param name="skipColumns">Number of columns to offset from the left.</param>
    /// <returns>A full-size preview grid matching GitHub's layout.</returns>
    public static bool[,] CreateFullPreviewGrid(bool[,] grid, int skipColumns)
    {
        var padded = new bool[GraphHeight, GraphWidth];

        int rows = Math.Min(grid.GetLength(0), GraphHeight);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols && (c + skipColumns) < GraphWidth; c++)
                padded[r, c + skipColumns] = grid[r, c];

        return padded;
    }
}
