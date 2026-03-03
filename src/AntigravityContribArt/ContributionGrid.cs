namespace AntigravityContribArt;

/// <summary>
/// Maps a 7×N boolean pixel grid onto calendar dates that align with GitHub's
/// contribution graph. The contribution graph displays the most recent 53 weeks
/// starting on Sunday, so this class calculates the appropriate start date and
/// converts each (row, column) cell into a concrete <see cref="DateTime"/>
/// along with the number of commits required for the desired intensity level.
/// </summary>
public class ContributionGrid
{
    /// <summary>
    /// Base number of commits for a "lit" cell. This value is set high to
    /// ensure a solid dark green (level 4) on GitHub's contribution graph
    /// for maximum text readability.
    /// </summary>
    private const int BaseCommitCount = 20;

    /// <summary>
    /// Random variation applied to commit counts. Set to 0 for uniform
    /// intensity, ensuring all lit cells appear at the same green level.
    /// </summary>
    private const int CommitVariation = 0;

    /// <summary>
    /// The first Sunday of the contribution graph period.
    /// </summary>
    public DateTime StartDate { get; }

    /// <summary>
    /// Initializes a new contribution grid anchored to the current date.
    /// The start date is calculated as the Sunday 52 weeks before the most
    /// recent Sunday relative to <paramref name="referenceDate"/>.
    /// </summary>
    /// <param name="referenceDate">
    /// The reference date (typically today) used to compute the graph period.
    /// </param>
    public ContributionGrid(DateTime referenceDate)
    {
        // Find the most recent Sunday (or today if it is Sunday).
        var lastSunday = referenceDate.Date;
        while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
            lastSunday = lastSunday.AddDays(-1);

        // The graph covers 53 weeks: from 52 weeks before last Sunday up to
        // (and including) next Saturday.
        StartDate = lastSunday.AddDays(-52 * 7);
    }

    /// <summary>
    /// Converts a pixel grid position (row = day of week, col = week index)
    /// into a calendar date.
    /// </summary>
    /// <param name="row">
    /// Row index (0 = Sunday, 1 = Monday, … , 6 = Saturday).
    /// </param>
    /// <param name="col">Week index (0 = oldest week, 52 = most recent).</param>
    /// <returns>The corresponding calendar date.</returns>
    public DateTime GetDate(int row, int col)
    {
        return StartDate.AddDays(col * 7 + row);
    }

    /// <summary>
    /// Enumerates all cells in the provided pixel grid, returning
    /// <see cref="CommitInstruction"/> entries only for "lit" cells.
    /// Each instruction contains the target date and the number of commits
    /// to create on that date.
    /// </summary>
    /// <param name="grid">
    /// A 7×N boolean grid produced by <see cref="TextRenderer.Render"/>.
    /// </param>
    /// <param name="skipColumns">
    /// Number of columns (weeks) to skip from the left before placing the
    /// text. Default is 0 (start at the beginning of the graph).
    /// </param>
    /// <returns>
    /// A sequence of <see cref="CommitInstruction"/> for every lit cell.
    /// </returns>
    public IEnumerable<CommitInstruction> GetCommitInstructions(bool[,] grid, int skipColumns = 0)
    {
        var random = new Random(42); // Deterministic seed for reproducible results
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, col])
                {
                    int commitCount = BaseCommitCount + random.Next(-CommitVariation, CommitVariation + 1);
                    yield return new CommitInstruction(GetDate(row, col + skipColumns), Math.Max(1, commitCount));
                }
            }
        }
    }
}

/// <summary>
/// Represents a single instruction to create a specific number of git commits
/// on a specific date to light up one cell on the contribution graph.
/// </summary>
/// <param name="Date">The target date for the commits.</param>
/// <param name="CommitCount">The number of commits to create on that date.</param>
public record CommitInstruction(DateTime Date, int CommitCount);
