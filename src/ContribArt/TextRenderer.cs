namespace ContribArt;

/// <summary>
/// Converts a text string into a 7-row boolean pixel grid by looking up each
/// character in <see cref="PixelFont"/> and concatenating the glyphs
/// horizontally with single-column spacing between characters. If the text
/// is too wide for a single line, it automatically switches to two-line mode
/// using <see cref="SmallPixelFont"/> (3-row compact font).
/// </summary>
public static class TextRenderer
{
    /// <summary>
    /// The maximum number of columns that fit inside GitHub's contribution
    /// graph (53 weeks = 53 columns).
    /// </summary>
    public const int MaxGridWidth = 53;

    /// <summary>
    /// Number of empty columns inserted between consecutive characters.
    /// </summary>
    private const int CharSpacing = 1;

    /// <summary>
    /// Number of empty rows between two lines of text in two-line mode.
    /// </summary>
    private const int LineGap = 1;

    /// <summary>
    /// Renders the given text into a 7×N boolean grid suitable for mapping
    /// onto the GitHub contribution calendar. Automatically uses two-line
    /// mode with a compact font if the text is too wide for a single line.
    /// </summary>
    /// <param name="text">The text to render (case-insensitive).</param>
    /// <returns>
    /// A boolean grid of dimensions [7, totalWidth] where <c>true</c> indicates
    /// a cell that should receive commits and <c>false</c> an empty cell.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the rendered text exceeds <see cref="MaxGridWidth"/> columns
    /// even in two-line mode.
    /// </exception>
    public static bool[,] Render(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(text));

        // Try single-line mode first (full 7-tall font)
        var singleLineGlyphs = text.Select(c => PixelFont.GetGlyph(c)).ToList();
        int singleLineWidth = CalculateTotalWidth(singleLineGlyphs);

        if (singleLineWidth <= MaxGridWidth)
        {
            Console.WriteLine("  Mode: Single line (5×7 font)");
            return ComposeGrid(singleLineGlyphs, singleLineWidth, PixelFont.CharHeight);
        }

        // Fall back to two-line mode (compact 3-tall font)
        Console.WriteLine("  Mode: Two lines (3×3 compact font — text too wide for single line)");
        return RenderTwoLines(text);
    }

    /// <summary>
    /// Renders text across two lines using the compact <see cref="SmallPixelFont"/>.
    /// The text is split in half — preferring a space as the break point.
    /// Line 1 occupies rows 0-2, line 2 occupies rows 4-6, with row 3 as a gap.
    /// </summary>
    private static bool[,] RenderTwoLines(string text)
    {
        var (line1, line2) = SplitText(text);

        var glyphs1 = line1.Select(c => SmallPixelFont.GetGlyph(c)).ToList();
        var glyphs2 = line2.Select(c => SmallPixelFont.GetGlyph(c)).ToList();

        int width1 = CalculateTotalWidth(glyphs1);
        int width2 = CalculateTotalWidth(glyphs2);
        int maxWidth = Math.Max(width1, width2);

        if (maxWidth > MaxGridWidth)
            throw new ArgumentException(
                $"Text \"{text}\" requires {maxWidth} columns even in two-line mode, " +
                $"but the GitHub contribution graph only has {MaxGridWidth}. Please use shorter text.");

        Console.WriteLine($"  Line 1: \"{line1}\" ({width1} cols)");
        Console.WriteLine($"  Line 2: \"{line2}\" ({width2} cols)");

        // Compose each line into its own small grid
        var grid1 = ComposeGrid(glyphs1, width1, SmallPixelFont.CharHeight);
        var grid2 = ComposeGrid(glyphs2, width2, SmallPixelFont.CharHeight);

        // Merge into one 7-row grid
        int totalWidth = Math.Max(width1, width2);
        var merged = new bool[7, totalWidth];

        // Line 1 → rows 0-2
        CopyInto(merged, grid1, rowOffset: 0);
        // Line 2 → rows 4-6 (row 3 is the gap)
        CopyInto(merged, grid2, rowOffset: SmallPixelFont.CharHeight + LineGap);

        return merged;
    }

    /// <summary>
    /// Splits text into two roughly equal lines. If the text contains spaces,
    /// the split is made at the space nearest to the middle. Otherwise, the
    /// text is split exactly in half.
    /// </summary>
    private static (string line1, string line2) SplitText(string text)
    {
        int mid = text.Length / 2;

        // Try to find a space near the middle to split on
        int bestSpace = -1;
        int bestDist = int.MaxValue;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                int dist = Math.Abs(i - mid);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestSpace = i;
                }
            }
        }

        if (bestSpace > 0 && bestSpace < text.Length - 1)
            return (text[..bestSpace], text[(bestSpace + 1)..]);

        // No suitable space found, split in the middle
        return (text[..mid], text[mid..]);
    }

    /// <summary>
    /// Copies a source grid into a target grid at the given row offset.
    /// </summary>
    private static void CopyInto(bool[,] target, bool[,] source, int rowOffset)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                target[r + rowOffset, c] = source[r, c];
    }

    /// <summary>
    /// Calculates the total width in columns required to display all glyphs
    /// including inter-character spacing.
    /// </summary>
    private static int CalculateTotalWidth(List<bool[,]> glyphs)
    {
        if (glyphs.Count == 0) return 0;

        int width = 0;
        for (int i = 0; i < glyphs.Count; i++)
        {
            width += glyphs[i].GetLength(1);
            if (i < glyphs.Count - 1)
                width += CharSpacing;
        }
        return width;
    }

    /// <summary>
    /// Composes the final pixel grid by placing each glyph side-by-side with
    /// spacing columns in between.
    /// </summary>
    private static bool[,] ComposeGrid(List<bool[,]> glyphs, int totalWidth, int charHeight)
    {
        var grid = new bool[charHeight, totalWidth];
        int offsetX = 0;

        foreach (var glyph in glyphs)
        {
            int glyphWidth = glyph.GetLength(1);
            int glyphHeight = glyph.GetLength(0);
            for (int row = 0; row < glyphHeight; row++)
                for (int col = 0; col < glyphWidth; col++)
                    grid[row, offsetX + col] = glyph[row, col];

            offsetX += glyphWidth + CharSpacing;
        }

        return grid;
    }

    /// <summary>
    /// Prints a visual preview of the rendered grid to the console using
    /// block characters for lit cells and spaces for empty cells.
    /// </summary>
    /// <param name="grid">The 7×N boolean grid to preview.</param>
    public static void PrintPreview(bool[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        Console.WriteLine();
        Console.WriteLine("  ┌" + new string('─', cols * 2) + "┐");

        for (int r = 0; r < rows; r++)
        {
            Console.Write("  │");
            for (int c = 0; c < cols; c++)
                Console.Write(grid[r, c] ? "██" : "  ");
            Console.WriteLine("│");
        }

        Console.WriteLine("  └" + new string('─', cols * 2) + "┘");
        Console.WriteLine();
    }
}
