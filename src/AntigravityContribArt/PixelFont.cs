namespace AntigravityContribArt;

/// <summary>
/// Provides a 5-wide × 7-tall pixel bitmap font for rendering text on the
/// GitHub contribution graph. Each character is represented as a boolean grid
/// where <c>true</c> means "lit" (commit) and <c>false</c> means "empty".
/// </summary>
public static class PixelFont
{
    /// <summary>
    /// The fixed height of every character glyph, matching the 7 rows of the
    /// GitHub contribution calendar (Sunday through Saturday).
    /// </summary>
    public const int CharHeight = 7;

    /// <summary>
    /// The fixed width of every character glyph.
    /// </summary>
    public const int CharWidth = 5;

    /// <summary>
    /// Returns the pixel bitmap for a given character, or a solid block for
    /// unsupported characters. Each entry in the outer array corresponds to a
    /// row (day of week), and each entry in the inner array to a column.
    /// </summary>
    /// <param name="c">The character to look up.</param>
    /// <returns>A 7×5 boolean grid representing the character glyph.</returns>
    public static bool[,] GetGlyph(char c)
    {
        c = char.ToUpperInvariant(c);
        return Glyphs.TryGetValue(c, out var glyph) ? glyph : CreateSolidBlock();
    }

    /// <summary>
    /// Checks whether a glyph definition exists for the given character.
    /// </summary>
    public static bool HasGlyph(char c) => Glyphs.ContainsKey(char.ToUpperInvariant(c));

    /// <summary>
    /// Creates a fully filled 7×5 block used as a fallback for unknown characters.
    /// </summary>
    private static bool[,] CreateSolidBlock()
    {
        var block = new bool[CharHeight, CharWidth];
        for (int r = 0; r < CharHeight; r++)
            for (int col = 0; col < CharWidth; col++)
                block[r, col] = true;
        return block;
    }

    /// <summary>
    /// Converts a compact string representation into a 7×5 boolean grid.
    /// Each string in the array represents one row; '#' = lit, '.' = empty.
    /// </summary>
    private static bool[,] Parse(params string[] rows)
    {
        var glyph = new bool[CharHeight, CharWidth];
        for (int r = 0; r < CharHeight; r++)
            for (int c = 0; c < CharWidth; c++)
                glyph[r, c] = r < rows.Length && c < rows[r].Length && rows[r][c] == '#';
        return glyph;
    }

    /// <summary>
    /// Master dictionary of all supported character glyphs.
    /// </summary>
    private static readonly Dictionary<char, bool[,]> Glyphs = new()
    {
        ['A'] = Parse(
            ".###.",
            "#...#",
            "#...#",
            "#####",
            "#...#",
            "#...#",
            "#...#"),
        ['B'] = Parse(
            "####.",
            "#...#",
            "#...#",
            "####.",
            "#...#",
            "#...#",
            "####."),
        ['C'] = Parse(
            ".###.",
            "#...#",
            "#....",
            "#....",
            "#....",
            "#...#",
            ".###."),
        ['D'] = Parse(
            "####.",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            "####."),
        ['E'] = Parse(
            "#####",
            "#....",
            "#....",
            "####.",
            "#....",
            "#....",
            "#####"),
        ['F'] = Parse(
            "#####",
            "#....",
            "#....",
            "####.",
            "#....",
            "#....",
            "#...."),
        ['G'] = Parse(
            ".###.",
            "#...#",
            "#....",
            "#.###",
            "#...#",
            "#...#",
            ".###."),
        ['H'] = Parse(
            "#...#",
            "#...#",
            "#...#",
            "#####",
            "#...#",
            "#...#",
            "#...#"),
        ['I'] = Parse(
            "#####",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            "#####"),
        ['J'] = Parse(
            "..###",
            "...#.",
            "...#.",
            "...#.",
            "...#.",
            "#..#.",
            ".##.."),
        ['K'] = Parse(
            "#...#",
            "#..#.",
            "#.#..",
            "##...",
            "#.#..",
            "#..#.",
            "#...#"),
        ['L'] = Parse(
            "#....",
            "#....",
            "#....",
            "#....",
            "#....",
            "#....",
            "#####"),
        ['M'] = Parse(
            "#...#",
            "##.##",
            "#.#.#",
            "#.#.#",
            "#...#",
            "#...#",
            "#...#"),
        ['N'] = Parse(
            "#...#",
            "##..#",
            "#.#.#",
            "#..##",
            "#...#",
            "#...#",
            "#...#"),
        ['O'] = Parse(
            ".###.",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            ".###."),
        ['P'] = Parse(
            "####.",
            "#...#",
            "#...#",
            "####.",
            "#....",
            "#....",
            "#...."),
        ['Q'] = Parse(
            ".###.",
            "#...#",
            "#...#",
            "#...#",
            "#.#.#",
            "#..#.",
            ".##.#"),
        ['R'] = Parse(
            "####.",
            "#...#",
            "#...#",
            "####.",
            "#.#..",
            "#..#.",
            "#...#"),
        ['S'] = Parse(
            ".####",
            "#....",
            "#....",
            ".###.",
            "....#",
            "....#",
            "####."),
        ['T'] = Parse(
            "#####",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            "..#.."),
        ['U'] = Parse(
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            ".###."),
        ['V'] = Parse(
            "#...#",
            "#...#",
            "#...#",
            "#...#",
            ".#.#.",
            ".#.#.",
            "..#.."),
        ['W'] = Parse(
            "#...#",
            "#...#",
            "#...#",
            "#.#.#",
            "#.#.#",
            "##.##",
            "#...#"),
        ['X'] = Parse(
            "#...#",
            "#...#",
            ".#.#.",
            "..#..",
            ".#.#.",
            "#...#",
            "#...#"),
        ['Y'] = Parse(
            "#...#",
            "#...#",
            ".#.#.",
            "..#..",
            "..#..",
            "..#..",
            "..#.."),
        ['Z'] = Parse(
            "#####",
            "....#",
            "...#.",
            "..#..",
            ".#...",
            "#....",
            "#####"),

        // Digits
        ['0'] = Parse(
            ".###.",
            "#..##",
            "#.#.#",
            "#.#.#",
            "#.#.#",
            "##..#",
            ".###."),
        ['1'] = Parse(
            "..#..",
            ".##..",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            ".###."),
        ['2'] = Parse(
            ".###.",
            "#...#",
            "....#",
            "..##.",
            ".#...",
            "#....",
            "#####"),
        ['3'] = Parse(
            ".###.",
            "#...#",
            "....#",
            "..##.",
            "....#",
            "#...#",
            ".###."),
        ['4'] = Parse(
            "...#.",
            "..##.",
            ".#.#.",
            "#..#.",
            "#####",
            "...#.",
            "...#."),
        ['5'] = Parse(
            "#####",
            "#....",
            "####.",
            "....#",
            "....#",
            "#...#",
            ".###."),
        ['6'] = Parse(
            ".###.",
            "#....",
            "#....",
            "####.",
            "#...#",
            "#...#",
            ".###."),
        ['7'] = Parse(
            "#####",
            "....#",
            "...#.",
            "..#..",
            ".#...",
            ".#...",
            ".#..."),
        ['8'] = Parse(
            ".###.",
            "#...#",
            "#...#",
            ".###.",
            "#...#",
            "#...#",
            ".###."),
        ['9'] = Parse(
            ".###.",
            "#...#",
            "#...#",
            ".####",
            "....#",
            "....#",
            ".###."),

        // Punctuation & special
        [' '] = Parse(
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            "....."),
        ['!'] = Parse(
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            "..#..",
            ".....",
            "..#.."),
        ['.'] = Parse(
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            "..#.."),
        ['-'] = Parse(
            ".....",
            ".....",
            ".....",
            ".###.",
            ".....",
            ".....",
            "....."),
        ['_'] = Parse(
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            "#####"),
        ['?'] = Parse(
            ".###.",
            "#...#",
            "....#",
            "..##.",
            "..#..",
            ".....",
            "..#.."),
        ['+'] = Parse(
            ".....",
            "..#..",
            "..#..",
            "#####",
            "..#..",
            "..#..",
            "....."),
        ['='] = Parse(
            ".....",
            ".....",
            "#####",
            ".....",
            "#####",
            ".....",
            "....."),
        ['<'] = Parse(
            "...#.",
            "..#..",
            ".#...",
            "#....",
            ".#...",
            "..#..",
            "...#."),
        ['>'] = Parse(
            ".#...",
            "..#..",
            "...#.",
            "....#",
            "...#.",
            "..#..",
            ".#..."),
        ['/'] = Parse(
            "....#",
            "...#.",
            "...#.",
            "..#..",
            ".#...",
            ".#...",
            "#...."),
        ['*'] = Parse(
            ".....",
            "#.#.#",
            ".###.",
            "#####",
            ".###.",
            "#.#.#",
            "....."),
        ['#'] = Parse(
            ".#.#.",
            ".#.#.",
            "#####",
            ".#.#.",
            "#####",
            ".#.#.",
            ".#.#."),
        ['@'] = Parse(
            ".###.",
            "#...#",
            "#.###",
            "#.#.#",
            "#.##.",
            "#....",
            ".####"),
        ['('] = Parse(
            "..#..",
            ".#...",
            "#....",
            "#....",
            "#....",
            ".#...",
            "..#.."),
        [')'] = Parse(
            "..#..",
            "...#.",
            "....#",
            "....#",
            "....#",
            "...#.",
            "..#.."),
    };
}
