using System.CommandLine;

namespace ContribArt;

/// <summary>
/// CLI entry point for ContribArt. Defines command-line options
/// and delegates execution to <see cref="ContributionArtEngine"/>.
/// </summary>
public class Program
{
    /// <summary>
    /// Application entry point. Configures the CLI options and invokes the
    /// main handler.
    /// </summary>
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            "ContribArt — Draw text on GitHub's contribution graph using backdated commits.");

        var folderOption = new Option<string>(
            name: "--folder",
            description: "Path to the target folder where commits will be created. " +
                         "A new git repository will be initialized if one does not exist.")
        { IsRequired = true };

        var textOption = new Option<string>(
            name: "--text",
            description: "The text to draw on the contribution graph (A-Z, 0-9, basic punctuation). " +
                         "Max ~8 characters depending on width.")
        { IsRequired = true };

        var skipColumnOption = new Option<int>(
            name: "--skip-column",
            description: "Number of columns (weeks) to skip from the left before placing the text. " +
                         "Use this to position text at a specific offset on the graph.")
        { IsRequired = false };
        skipColumnOption.SetDefaultValue(0);

        var committerNameOption = new Option<string>(
            name: "--committer-name",
            description: "Name to use as the commit author/committer.")
        { IsRequired = true };

        var committerEmailOption = new Option<string>(
            name: "--committer-email",
            description: "Email to use as the commit author/committer.")
        { IsRequired = true };

        rootCommand.AddOption(folderOption);
        rootCommand.AddOption(textOption);
        rootCommand.AddOption(skipColumnOption);
        rootCommand.AddOption(committerNameOption);
        rootCommand.AddOption(committerEmailOption);

        rootCommand.SetHandler((folder, text, skipColumn, committerName, committerEmail) =>
        {
            var engine = new ContributionArtEngine(committerName, committerEmail);
            engine.Run(folder, text, skipColumn);
        }, folderOption, textOption, skipColumnOption, committerNameOption, committerEmailOption);

        return await rootCommand.InvokeAsync(args);
    }
}
