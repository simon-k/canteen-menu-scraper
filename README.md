# canteen-menu-scraper

## How it works
Reads canteen menu from website, parses it and writes it to a gist as a CSV file .

# Requirements
1. A login to the Canteen website
2. An existing Gist to store the menu

## How to run it
The console app has 4 parameters
1. The username for the canteen user
2. The password for the canteen user
3. PAT for a GitHub user that has permission to write to the Gist 
4. Gist ID for the Gist you want to write to

```bash
CanteenParser.exe <canteen-username> <canteen-password> <github-pat> <gist-id>
```

The menu can then be accessed from the Gist URL.

```
https://gist.githubusercontent.com/{Github Username of PAT}/{Gist ID}}/raw/LP42.csv
```

## Build a single executable
If you want a single executable file for easy distribution you can build it with the following command. 
From the root directory of the solution run the following command:

_Windows_
```bash
dotnet publish CanteenParser -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=embedded
```
The exe file is placed in `CanteenParser\bin\Release\net8.0\win-x64\publish`

_MacOS_ 
T.B.D.

## TODO
* Make canteen website url an optional parameter
* Make the canteen id a parameter
* Make number of days a parameter
* Dependency Injection. Fx the HttpClient should only be created once with a HttpClientFactory https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0
* Add some tests based on the json files in the testdata folder~~~~
