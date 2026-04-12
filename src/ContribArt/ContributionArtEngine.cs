namespace ContribArt;

/// <summary>
/// Orchestrates the full contribution art pipeline: rendering text into a
/// pixel grid, mapping grid cells onto calendar dates, initializing the git
/// repository, and creating backdated commits. This class encapsulates all
/// business logic so that <see cref="Program"/> remains a thin CLI wrapper.
/// </summary>
public class ContributionArtEngine
{
    private readonly GitService _gitService;

    /// <summary>
    /// Creates a new <see cref="ContributionArtEngine"/> with the specified committer identity.
    /// </summary>
    /// <param name="committerName">Name to use as the commit author/committer.</param>
    /// <param name="committerEmail">Email to use as the commit author/committer.</param>
    public ContributionArtEngine(string committerName, string committerEmail)
    {
        _gitService = new GitService(committerName, committerEmail);
    }

    /// <summary>
    /// Executes the full contribution art pipeline with the given parameters.
    /// </summary>
    /// <param name="folder">Target folder path for the git repository.</param>
    /// <param name="text">Text to render on the contribution graph.</param>
    /// <param name="skipColumn">Number of columns to skip from the left (default 0).</param>
    public void Run(string folder, string text, int skipColumn)
    {
        PrintBanner();

        // ── Step 1: Render text to pixel grid ────────────────────────────
        var grid = RenderText(text);
        if (grid is null) return;

        ShowPreview(grid, skipColumn);

        // ── Step 2: Map grid to calendar dates ───────────────────────────
        var instructions = MapToCalendar(grid, skipColumn);

        // ── Step 3: Ensure git repository ────────────────────────────────
        string fullPath = PrepareRepository(folder);

        // ── Step 4: Create backdated commits ─────────────────────────────
        CreateAllCommits(fullPath, instructions);

        // ── Done ─────────────────────────────────────────────────────────
        PrintSummary(fullPath, text);
    }

    /// <summary>
    /// Prints the application banner to the console.
    /// </summary>
    private static void PrintBanner()
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════╗");
        Console.WriteLine("║             ContribArt v1.0.0                   ║");
        Console.WriteLine("║    GitHub Contribution Graph Text Writer        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    /// <summary>
    /// Renders the input text into a 7-row boolean pixel grid.
    /// Returns null if the text cannot be rendered.
    /// </summary>
    private static bool[,]? RenderText(string text)
    {
        Console.WriteLine("► Step 1: Rendering text to pixel grid...");
        Console.WriteLine($"  Text: \"{text}\"");

        try
        {
            return TextRenderer.Render(text);
        }
        catch (ArgumentException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ Error: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    /// <summary>
    /// Displays grid statistics and a full 53×7 console preview showing
    /// exactly how the text will appear on GitHub's contribution graph.
    /// </summary>
    private static void ShowPreview(bool[,] grid, int skipColumn)
    {
        int litCells = GridHelper.CountLitCells(grid);
        Console.WriteLine($"  Grid size: {grid.GetLength(1)} columns × {grid.GetLength(0)} rows");
        if (skipColumn > 0)
            Console.WriteLine($"  Skip columns: {skipColumn} (text offset by {skipColumn} weeks)");
        Console.WriteLine($"  Lit cells: {litCells}");
        Console.WriteLine("  Preview:");

        var previewGrid = GridHelper.CreateFullPreviewGrid(grid, skipColumn);
        TextRenderer.PrintPreview(previewGrid);
    }

    /// <summary>
    /// Maps the rendered pixel grid onto calendar dates and returns commit
    /// instructions.
    /// </summary>
    private static List<CommitInstruction> MapToCalendar(bool[,] grid, int skipColumn)
    {
        Console.WriteLine("► Step 2: Mapping pixels to calendar dates...");
        var contributionGrid = new ContributionGrid(DateTime.Now);
        var instructions = contributionGrid.GetCommitInstructions(grid, skipColumn).ToList();
        int totalCommits = instructions.Sum(i => i.CommitCount);

        Console.WriteLine($"  Graph period: {contributionGrid.StartDate:yyyy-MM-dd} → " +
                          $"{contributionGrid.StartDate.AddDays(52 * 7 + 6):yyyy-MM-dd}");
        Console.WriteLine($"  Total commits to create: {totalCommits}");
        Console.WriteLine();

        return instructions;
    }

    /// <summary>
    /// Ensures the target folder exists and contains a clean git repository.
    /// </summary>
    private string PrepareRepository(string folder)
    {
        Console.WriteLine("► Step 3: Preparing git repository...");
        string fullPath = Path.GetFullPath(folder);
        Directory.CreateDirectory(fullPath);

        _gitService.EnsureGitRepository(fullPath);
        Console.WriteLine();

        return fullPath;
    }

    /// <summary>
    /// Creates all backdated commits using git fast-import for maximum speed.
    /// A single git process handles all commits via stdin streaming.
    /// </summary>
    private void CreateAllCommits(string folderPath, List<CommitInstruction> instructions)
    {
        Console.WriteLine("► Step 4: Creating backdated commits (fast-import)...");
        int totalCommits = instructions.Sum(i => i.CommitCount);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        _gitService.BulkCreateCommits(folderPath, instructions, completedCommits =>
        {
            double progress = (double)completedCommits / totalCommits * 100;
            Console.Write($"\r  Progress: {completedCommits}/{totalCommits} commits ({progress:F1}%)   ");
        });

        sw.Stop();
        Console.WriteLine();
        Console.WriteLine($"  ⚡ Completed in {sw.Elapsed.TotalSeconds:F1}s ({totalCommits / Math.Max(sw.Elapsed.TotalSeconds, 0.1):F0} commits/sec)");
        Console.WriteLine();
    }

    /// <summary>
    /// Prints the final success message and next steps for the user.
    /// </summary>
    private static void PrintSummary(string fullPath, string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Done! All commits have been created successfully.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  Next steps:");
        Console.WriteLine($"  1. cd \"{fullPath}\"");
        Console.WriteLine("  2. git remote add origin <your-repo-url>");
        Console.WriteLine("  3. git push -u origin master");
        Console.WriteLine();
        Console.WriteLine("  After pushing, your GitHub profile contribution graph will display");
        Console.WriteLine($"  the text \"{text}\" within a few minutes.");
        Console.WriteLine();
    }
}
