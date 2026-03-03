# AntigravityContribArt

**Draw text on your GitHub contribution graph using backdated commits.**

AntigravityContribArt is a .NET CLI tool that converts any text into pixel art and writes it onto GitHub's contribution calendar by creating precisely-dated git commits. No push is performed — you decide when and where to publish.

---

## Features

- **Pixel Font Engine** — Built-in 5×7 bitmap font supporting A–Z, 0–9, and common punctuation
- **Two-Line Mode** — Automatically switches to a compact 3×3 font for longer text
- **Smart Date Mapping** — Automatically computes calendar dates aligned with GitHub's 53-week contribution grid
- **Skip Column** — Position text at a specific offset on the graph with `--skip-column`
- **Full Preview** — 53×7 console preview matching exactly how GitHub's contribution graph will look
- **Blazing Fast** — Uses `git fast-import` to create thousands of commits in seconds
- **Solid Green** — Uniform high-intensity commits ensure all text is crisp and fully readable
- **Git Integration** — Auto-initializes a clean repository; cleans existing files before starting
- **Progress Tracking** — Real-time progress bar with commits/sec speed indicator

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- [Git](https://git-scm.com/downloads) installed and available in PATH

---

## Installation

```bash
git clone https://github.com/bariskisir/AntigravityContribArt.git
cd AntigravityContribArt/src/AntigravityContribArt
dotnet restore
dotnet build
```

---

## Usage

```bash
dotnet run -- --folder <path> --text <text> --committer-name <name> --committer-email <email> [--skip-column <n>]
```

### Parameters

| Parameter          | Required | Default | Description |
|--------------------|----------|---------|-------------|
| `--folder`         | ✅       | —       | Path to the target folder. A git repo will be initialized here if needed. |
| `--text`           | ✅       | —       | Text to render on the contribution graph. |
| `--committer-name` | ✅       | —       | Name to use as the commit author/committer. |
| `--committer-email`| ✅       | —       | Email to use as the commit author/committer. |
| `--skip-column`    | ❌       | `0`     | Number of columns (weeks) to skip from the left before placing the text. |

### Examples

```bash
# Write "HELLO" to a new repo
dotnet run -- --folder C:\contrib-art --text "HELLO" --committer-name "ContribArt" --committer-email "contribart@users.noreply.github.com"

# Write "HI" with 5 columns offset
dotnet run -- --folder ./my-repo --text "HI" --committer-name "John" --committer-email "john@example.com" --skip-column 5

# Longer text automatically uses two-line mode
dotnet run -- --folder ./my-repo --text "HELLO WORLD" --committer-name "ContribArt" --committer-email "contribart@users.noreply.github.com"

# Show help
dotnet run -- --help
```

---

## How It Works

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐     ┌──────────────┐
│  Input Text  │ ──► │  Pixel Font  │ ──► │ Contribution    │ ──► │ Git Commits  │
│  "--text HI" │     │  5×7 / 3×3   │     │ Grid Date Map   │     │ (fast-import)│
└─────────────┘     └──────────────┘     └─────────────────┘     └──────────────┘
```

1. **Text Rendering** — Each character is converted into a pixel bitmap (5×7 standard or 3×3 compact for two-line mode)
2. **Grid Composition** — Characters are placed side-by-side with 1-column spacing on a 7×N grid
3. **Date Mapping** — Each lit pixel is mapped to a specific date within the last 53 weeks
4. **Bulk Commit Creation** — All backdated commits are streamed through `git fast-import` in a single process

---

## After Running

The tool only creates **local commits**. To see the text on your GitHub profile:

```bash
cd <your-folder>
git remote add origin https://github.com/<your-username>/<repo-name>.git
git branch -M master
git push -u origin master
```

Your contribution graph will update within a few minutes after pushing.

---

## Limitations

- Maximum text length depends on character widths (~8 uppercase characters fit in single-line mode)
- Two-line mode supports longer text but with smaller characters (~13 per line)
- The contribution graph shows the **last 53 weeks**, so commits are dated within that window
- GitHub may take a few minutes to refresh the contribution graph after pushing

---

## Project Structure

```
src/AntigravityContribArt/
├── Program.cs                  # CLI entry point (thin wrapper)
├── ContributionArtEngine.cs    # Pipeline orchestrator
├── TextRenderer.cs             # Text → pixel grid renderer (single & two-line)
├── PixelFont.cs                # 5×7 bitmap character definitions
├── SmallPixelFont.cs           # 3×3 compact font for two-line mode
├── ContributionGrid.cs         # Pixel grid → calendar date mapper
├── GridHelper.cs               # Grid utility methods
├── GitService.cs               # Git init, cleanup & fast-import commit creation
└── AntigravityContribArt.csproj
```

---

## Author

**bariskisir** — [github.com/bariskisir](https://github.com/bariskisir)

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
