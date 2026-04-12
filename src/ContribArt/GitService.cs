using System.Diagnostics;
using System.Text;

namespace ContribArt;

/// <summary>
/// Handles all git operations: initializing repositories and creating
/// backdated commits. Uses <c>git fast-import</c> for bulk commit creation
/// which is orders of magnitude faster than spawning individual
/// <c>git commit</c> processes.
/// </summary>
public class GitService
{
    private readonly string _committerName;
    private readonly string _committerEmail;

    /// <summary>
    /// Creates a new <see cref="GitService"/> with the specified committer identity.
    /// </summary>
    /// <param name="committerName">Name to use as the commit author/committer.</param>
    /// <param name="committerEmail">Email to use as the commit author/committer.</param>
    public GitService(string committerName, string committerEmail)
    {
        _committerName = committerName;
        _committerEmail = committerEmail;
    }

    /// <summary>
    /// Ensures that the specified folder has a clean git repository. If a .git
    /// directory already exists, it is removed and re-initialized so that only
    /// the new contribution art commits will be present. If no .git directory
    /// is found, <c>git init</c> is executed to create one.
    /// </summary>
    /// <param name="folderPath">Absolute path to the target folder.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the git init command fails.
    /// </exception>
    public void EnsureGitRepository(string folderPath)
    {
        // Clean the entire folder contents before starting fresh
        if (Directory.Exists(folderPath))
        {
            Console.WriteLine($"  ⚠ Existing folder found at: {folderPath}");
            Console.WriteLine("  ⚙ Cleaning all existing files and directories...");
            CleanFolder(folderPath);
        }

        Console.WriteLine($"  ⚙ Initializing new git repository at: {folderPath}");
        RunGit(folderPath, "init");
        Console.WriteLine("  ✓ Git repository initialized (clean state).");
    }

    /// <summary>
    /// Creates all backdated commits in a single operation using
    /// <c>git fast-import</c>. This streams all commit data through stdin
    /// to a single git process, avoiding the overhead of spawning thousands
    /// of individual processes. Typically 50-100x faster than individual
    /// <c>git commit</c> calls.
    /// </summary>
    /// <param name="folderPath">Absolute path to the git repository.</param>
    /// <param name="instructions">All commit instructions to execute.</param>
    /// <param name="onProgress">
    /// Optional callback invoked after each instruction is streamed,
    /// receiving the number of commits completed so far.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <c>git fast-import</c> fails.
    /// </exception>
    public void BulkCreateCommits(
        string folderPath,
        List<CommitInstruction> instructions,
        Action<int>? onProgress = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = folderPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("fast-import");
        psi.ArgumentList.Add("--quiet");

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start git fast-import process.");

        // Read stderr asynchronously to prevent deadlock
        var stderrBuilder = new StringBuilder();
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) stderrBuilder.AppendLine(e.Data); };
        process.BeginErrorReadLine();

        using var writer = process.StandardInput;

        int markId = 1;
        int completedCommits = 0;

        foreach (var instruction in instructions)
        {
            // Use noon UTC to ensure the commit lands on the correct calendar day
            // regardless of the user's timezone (any offset from UTC-12 to UTC+12
            // will still resolve to the same date). Previously, midnight local time
            // was converted to UTC, which shifted the date back by one day for
            // timezones ahead of UTC (e.g. UTC+3 → 21:00 UTC the previous day).
            var utcNoon = new DateTimeOffset(instruction.Date.Year, instruction.Date.Month,
                instruction.Date.Day, 12, 0, 0, TimeSpan.Zero);
            long unixTimestamp = utcNoon.ToUnixTimeSeconds();

            for (int i = 0; i < instruction.CommitCount; i++)
            {
                string commitMsg = $"contribution: {instruction.Date:yyyy-MM-dd} ({i + 1}/{instruction.CommitCount})";
                byte[] msgBytes = Encoding.UTF8.GetBytes(commitMsg);

                // A single log line that changes each commit so fast-import accepts it
                string fileContent = $"{instruction.Date:yyyy-MM-dd} #{markId}\n";
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                writer.Write($"commit refs/heads/master\n");
                writer.Write($"mark :{markId}\n");
                writer.Write($"author {_committerName} <{_committerEmail}> {unixTimestamp} +0000\n");
                writer.Write($"committer {_committerName} <{_committerEmail}> {unixTimestamp} +0000\n");
                writer.Write($"data {msgBytes.Length}\n");
                writer.Write(commitMsg);
                writer.Write("\n");

                // Reference previous commit to build a linear history
                if (markId > 1)
                    writer.Write($"from :{markId - 1}\n");

                // Modify a file so the commit is valid
                writer.Write($"M 644 inline .contribution-art\n");
                writer.Write($"data {fileBytes.Length}\n");
                writer.Write(fileContent);

                markId++;
                completedCommits++;
            }

            // Report progress per instruction
            onProgress?.Invoke(completedCommits);
        }

        writer.Flush();
        writer.Close();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"git fast-import failed (exit code {process.ExitCode}): {stderrBuilder}");
        }
    }

    /// <summary>
    /// Removes all files and subdirectories inside the given folder, leaving
    /// the folder itself intact. Handles read-only files that git commonly
    /// creates inside .git directories.
    /// </summary>
    private static void CleanFolder(string folderPath)
    {
        // Remove all files (handle read-only)
        foreach (var file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        // Remove all subdirectories
        foreach (var dir in Directory.GetDirectories(folderPath))
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    /// <summary>
    /// Runs a git command in the specified working directory and waits for
    /// it to complete. Throws if the exit code is non-zero.
    /// </summary>
    private static void RunGit(string workingDir, params string[] args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in args)
            psi.ArgumentList.Add(arg);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start git process.");

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            string error = process.StandardError.ReadToEnd();
            throw new InvalidOperationException(
                $"git {string.Join(' ', args)} failed (exit code {process.ExitCode}): {error}");
        }
    }
}
