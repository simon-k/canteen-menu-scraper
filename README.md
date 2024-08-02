# canteen-menu-scraper

## How it works
Reads canteen menu from website, parses it and writes it to a gist as a CSV file .

# Requirements
1. A user with login to the Canteen website. The user must have assigned the canteens to the profilæe under its settings.
2. An existing Gist to store the menu

## How to run it
The console app has 6 parameters
1. The Canteen "School ID"
1. The username for the canteen user
2. The password for the canteen user
3. PAT for a GitHub user that has permission to write to the Gist 
4. Gist ID for the Gist you want to write to
5. Gidt Filename for the CSV file~~~~

```bash
CanteenParser.exe <canten-school-id> <canteen-username> <canteen-password> <github-pat> <gist-id> <gist-filename>
```

The menu can then be accessed from the Gist URL.

For example
```
https://gist.githubusercontent.com/{Github Username of PAT}/{Gist ID}}/raw/LP42.csv
```

And looks like this
```csv
Date,When,Kind,Name,Description
02-08-2024,02-08-2024 11:00,Vegetarian,"Mexican bean casule with rice sour cream, jalapenos, vegan tortilla chips","Mexicansk bønnecasule med ris creme fraiche, jalapenos, vegansk tortillia chips"
02-08-2024,02-08-2024 11:00,Meat,"Boeuf bourguignon with carrots, mushrooms and mashed potatoes topped with fresh herbs","Boeuf bourguignon med gulerødder, svampe hertil kartoffelmos toppet med friske urter"
02-08-2024,05-08-2024 11:00,Vegetarian,"Vegetarian Tom Kah Gai with Ingrid peas, bamboo shoots, coconut, lemongrass, lime leaves, peppers and chilli","Vegetarisk Tom Kah Gai med Ingrid ærter, bambusskud, kokos, citrongræs, limeblade peberfrugter og chili"
...
```

Note, the date and time is in CET timezone and in the format dd-MM-yyyy hh:mm~~~~.

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
* Add some tests based on the json files in the testdata folder

## W.T.F.
The Kanpla website is clearly built for schools and not for companies, so a canteen is identified by a School ID and not a Canteen Id. 