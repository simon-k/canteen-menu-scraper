# Canteen Menu Scraper

This is a .NET 8.0 console application that scrapes canteen menus from Kanpla website and Hub Nordic, parses them using AI, and writes the results to GitHub Gists as CSV files. The application runs on a scheduled GitHub Actions workflow to automatically update multiple canteen menus.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Build and Test Commands
Follow these exact commands in order for any development work:

```bash
# STEP 1: Restore dependencies (takes ~55 seconds)
dotnet restore
# NEVER CANCEL - Set timeout to 120+ seconds

# STEP 2: Build the solution (takes ~17 seconds) 
dotnet build --no-restore
# NEVER CANCEL - Set timeout to 60+ seconds

# STEP 3: Run tests (takes ~8 seconds, but network-dependent tests will fail)
dotnet test --no-build --verbosity normal
# NEVER CANCEL - Set timeout to 30+ seconds
# Note: Tests fail due to external network dependencies (madkastel.dk), this is expected in isolated environments
```

### Running the Application
The console application requires 7 parameters:
```bash
dotnet run --project CanteenParser <canteen-school-id> <canteen-username> <canteen-password> <openai-api-key> <github-pat> <gist-id> <gist-filename>
```

**Example for Hub1 (AI-based):**
```bash
dotnet run --project CanteenParser "Hub1" "dummy" "dummy" "sk-..." "ghp_..." "gist-id" "HUB1.csv"
```

**Example for Kanpla canteens:**
```bash  
dotnet run --project CanteenParser "LP42" "user@example.com" "password" "sk-..." "ghp_..." "gist-id" "LP42.csv"
```

**Important:** The application requires internet access to external services (Google Identity Toolkit for Kanpla, OpenAI for Hub1). It will fail in isolated environments.

## Validation

### Manual Testing Scenarios
After making code changes, always run these validation steps:

1. **Build Validation**: `dotnet build` must complete successfully with 0 errors (warnings are acceptable)
2. **JSON Parser Testing**: The parser should successfully process the test data in `CanteenParser.Test/frontend.json` (24,719 lines of Kanpla API response data)
3. **Output Format Testing**: Verify CSV output format matches: `Date,When,Kind,Name,Description` with CET timezone formatting `dd-MM-yyyy HH:mm`

### Test Data Validation
Use the existing test data for offline validation:
- **File**: `CanteenParser.Test/frontend.json` - Large JSON response from Kanpla API (24,719 lines)
- **Parser**: `KanplaMenuParser.GetAllDishes(7, content)` can parse this offline
- **Expected Output**: Dishes with vegetarian and meat variants for multiple days

**Quick Validation Test**: Create a temporary console application to test JSON parsing:
```csharp
// Test that JSON parsing works offline (save as temp test file)
var json = File.ReadAllText("CanteenParser.Test/frontend.json");
var content = JsonSerializer.Deserialize<KanplaWebsiteContent>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
var dishes = KanplaMenuParser.GetAllDishes(7, content);
Console.WriteLine($"Parsed {dishes.Count} dishes successfully");
```

### CI Pipeline Requirements
The GitHub Actions workflows expect:
- **test.yml**: Runs on push/PR to main branch
- **run.yml**: Scheduled execution (Mon-Fri at 8 UTC) for production menu updates
- **Secrets Required**: `CANTEEN_USERNAME`, `CANTEEN_PASSWORD`, `GIST_WRITE_PAT`, `GIST_ID`, `OPENAI_KEY`, multiple `CANTEEN_SCHOOL_ID_*` secrets

## Project Structure

### Key Directories and Files
```
├── CanteenParser/              # Main console application
│   ├── Program.cs              # Entry point (7 required parameters)
│   ├── Domain/                 # Data models
│   │   ├── Dish.cs             # Output dish model
│   │   ├── KanplaWebsiteContent.cs  # Kanpla API response models
│   │   └── Hub1Menu.cs         # Hub Nordic menu structure
│   ├── Readers/                # Data source readers
│   │   ├── KanplaWebsiteReader.cs   # Kanpla website scraper
│   │   ├── HubNordicAiReader.cs     # AI-based Hub Nordic reader
│   │   └── HubNordicWebsiteReader.cs # Web scraper for Hub Nordic
│   ├── Writers/                # Output writers
│   │   └── GistCsvWriter.cs    # GitHub Gist CSV writer
│   └── Utils/                  # Utilities
│       └── KindParser.cs       # Dish type classification
├── CanteenParser.Test/         # Test project (xUnit)
│   ├── MadkastelTest.cs        # Integration test (requires network)
│   ├── frontend.json           # Large Kanpla API test data (24K+ lines)
│   └── JsonReader.cs           # Test data helper
├── .github/workflows/          # CI/CD pipelines
│   ├── test.yml                # Build and test on PR/push
│   └── run.yml                 # Scheduled production runs
└── CanteenParser.sln           # Solution file
```

### Dependencies (from .csproj files)
- **Target Framework**: .NET 8.0
- **Main Packages**: 
  - CsvHelper (CSV output generation)
  - HtmlAgilityPack (HTML parsing for web scraping)
  - Microsoft.SemanticKernel.Connectors.OpenAI (AI integration)
- **Test Packages**: xunit, Microsoft.NET.Test.Sdk

## Common Development Tasks

### Adding a New Canteen
1. Add secrets to GitHub repository settings for the new canteen
2. Add new step in `.github/workflows/run.yml` following existing pattern
3. Test locally with dummy credentials (will fail auth but validate parsing)

### Modifying Menu Parsing Logic
1. **Always** test changes against `CanteenParser.Test/frontend.json` data
2. Key parsing logic in `KanplaMenuParser.GetAllDishes()`
3. Output format is standardized in `Dish.cs` - preserve `When` property format
4. Verify timezone handling (CET/CEST) in date formatting

### Debugging Authentication Issues
1. Kanpla authentication uses Google Identity Toolkit API (`identitytoolkit.googleapis.com`)
2. Hub Nordic uses direct AI API calls (OpenAI)
3. Network connectivity required for both authentication methods

## Troubleshooting

### Build Issues
- **Missing .NET 8.0**: Install .NET 8.0 SDK from https://dot.net
- **Package restore fails**: Run `dotnet clean` then `dotnet restore`
- **Nullable warnings**: Expected due to nullable reference types enabled

### Test Issues  
- **Network failures**: Expected in isolated environments - tests require external website access
- **Authentication failures**: Expected without valid credentials
- **JSON parsing errors**: Check if `frontend.json` test data is accessible

### Runtime Issues
- **IndexOutOfRangeException**: Application requires exactly 7 command-line parameters
- **Network timeouts**: External APIs (Kanpla, OpenAI) may be unavailable
- **Authentication errors**: Verify credentials and network access

## Time Expectations and Cancellation Warnings
**CRITICAL**: Always set these minimum timeout values to prevent premature cancellation:

- **dotnet restore**: ~55 seconds on first run, ~1 second on subsequent runs - NEVER CANCEL, set timeout to 120+ seconds
- **dotnet build**: ~17 seconds on first run, ~1 second on subsequent runs - NEVER CANCEL, set timeout to 60+ seconds  
- **dotnet test**: ~2 seconds (network tests fail, expected) - NEVER CANCEL, set timeout to 30+ seconds
- **Full CI pipeline**: ~2-3 minutes total
- **Production runs**: Variable based on canteen data volume and API response times (5-15 minutes per canteen)

## Quick Reference Commands
```bash
# Development workflow (run in repository root)
dotnet restore                              # Restore packages  
dotnet build --no-restore                   # Build solution
dotnet test --no-build --verbosity normal   # Run tests (network failures expected)

# Application execution
dotnet run --project CanteenParser <7-parameters>

# Structure validation
ls -la CanteenParser/Domain/                # Core models
ls -la CanteenParser/Readers/               # Data source integrations  
ls -la CanteenParser/Writers/               # Output generation
ls -la .github/workflows/                   # CI/CD pipelines
```

## Output Format
The application generates CSV files with this exact format:
```csv
Date,When,Kind,Name,Description
02-08-2024,02-08-2024 11:00,Vegetarian,"Mexican bean casule with rice sour cream, jalapenos, vegan tortilla chips","Mexicansk bønnecasule med ris creme fraiche, jalapenos, vegansk tortillia chips"
```

- **Date**: dd-MM-yyyy format
- **When**: dd-MM-yyyy HH:mm format (CET timezone, +11 hours from midnight)
- **Kind**: "Vegetarian", "Meat", or parsed from dish name
- **Name**: English dish name (cleaned of extra whitespace/formatting)
- **Description**: Danish dish description