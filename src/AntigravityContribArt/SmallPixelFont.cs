namespace AntigravityContribArt;

/// <summary>
/// Provides a compact 3-row × 3-column pixel bitmap font for rendering text
/// in two-line mode. When text is too wide for a single line on the
/// contribution graph, this smaller font is used so that two lines of text
/// can fit within the 7-row grid (rows 0-2 for line 1, row 3 gap, rows 4-6
/// for line 2).
/// </summary>
public static class SmallPixelFont
{
    /// <summary>
    /// The fixed height of every character glyph in the compact font.
    /// </summary>
    public const int CharHeight = 3;

    /// <summary>
    /// The fixed width of every character glyph in the compact font.
    /// </summary>
    public const int CharWidth = 3;

    /// <summary>
    /// Returns the pixel bitmap for a given character, or a solid block for
    /// unsupported characters.
    /// </summary>
    public static bool[,] GetGlyph(char c)
    {
        c = char.ToUpperInvariant(c);
        return Glyphs.TryGetValue(c, out var glyph) ? glyph : CreateSolidBlock();
    }

    /// <summary>
    /// Checks whether a glyph definition exists for the given character.
    /// </summary>
    public static bool HasGlyph(char c) => Glyphs.ContainsKey(char.ToUpperInvariant(c));

    private static bool[,] CreateSolidBlock()
    {
        var block = new bool[CharHeight, CharWidth];
        for (int r = 0; r < CharHeight; r++)
            for (int c = 0; c < CharWidth; c++)
                block[r, c] = true;
        return block;
    }

    /// <summary>
    /// Converts a compact string representation into a 3×3 boolean grid.
    /// '#' = lit, '.' = empty.
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
    /// Master dictionary of all supported compact character glyphs (3×3).
    /// </summary>
    private static readonly Dictionary<char, bool[,]> Glyphs = new()
    {
        ['A'] = Parse(".#.", "###", "#.#"),
        ['B'] = Parse("##.", "###", "##."),
        ['C'] = Parse(".##", "#..", ".##"),
        ['D'] = Parse("##.", "#.#", "##."),
        ['E'] = Parse("###", "##.", "###"),
        ['F'] = Parse("###", "##.", "#.."),
        ['G'] = Parse(".##", "#.#", ".##"),
        ['H'] = Parse("#.#", "###", "#.#"),
        ['I'] = Parse("###", ".#.", "###"),
        ['J'] = Parse(".##", "..#", "##."),
        ['K'] = Parse("#.#", "##.", "#.#"),
        ['L'] = Parse("#..", "#..", "###"),
        ['M'] = Parse("#.#", "###", "#.#"),
        ['N'] = Parse("#.#", "###", "#.#"),
        ['O'] = Parse(".#.", "#.#", ".#."),
        ['P'] = Parse("###", "###", "#.."),
        ['Q'] = Parse(".#.", "#.#", ".##"),
        ['R'] = Parse("##.", "###", "#.#"),
        ['S'] = Parse(".##", ".#.", "##."),
        ['T'] = Parse("###", ".#.", ".#."),
        ['U'] = Parse("#.#", "#.#", ".#."),
        ['V'] = Parse("#.#", "#.#", ".#."),
        ['W'] = Parse("#.#", "###", ".#."),
        ['X'] = Parse("#.#", ".#.", "#.#"),
        ['Y'] = Parse("#.#", ".#.", ".#."),
        ['Z'] = Parse("###", ".#.", "###"),

        // Digits
        ['0'] = Parse(".#.", "#.#", ".#."),
        ['1'] = Parse(".#.", "##.", ".#."),
        ['2'] = Parse("##.", ".#.", ".##"),
        ['3'] = Parse("##.", ".#.", "##."),
        ['4'] = Parse("#.#", "###", "..#"),
        ['5'] = Parse(".##", ".#.", "##."),
        ['6'] = Parse(".##", "###", ".#."),
        ['7'] = Parse("###", "..#", "..#"),
        ['8'] = Parse(".#.", "###", ".#."),
        ['9'] = Parse(".#.", "###", "##."),

        // Punctuation & special
        [' '] = Parse("...", "...", "..."),
        ['!'] = Parse(".#.", ".#.", ".#."),
        ['.'] = Parse("...", "...", ".#."),
        ['-'] = Parse("...", "###", "..."),
        ['_'] = Parse("...", "...", "###"),
        ['?'] = Parse("##.", ".#.", ".#."),
        ['+'] = Parse(".#.", "###", ".#."),
        ['='] = Parse("###", "...", "###"),
        ['/'] = Parse("..#", ".#.", "#.."),
        ['*'] = Parse("#.#", ".#.", "#.#"),
        ['#'] = Parse("###", "###", "###"),
        ['@'] = Parse("###", "#.#", "###"),
    };
}
