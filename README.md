# canteen-menu-scraper

## How it works
Reads a canteen menu from the Kanpla website, parses it and writes it to a Gist as a CSV file.

# Requirements
1. A user with login to the Kanpla website. The user must have assigned the canteens to the profiles under its settings in order to read the particular canteens menu.
2. An existing Gist to store the menu

## How to run it
The console app has 6 parameters

1. The Canteen "School ID"
1. The username for the canteen user
2. The password for the canteen user
3. PAT for a GitHub user that has permission to write to the Gist 
4. Gist ID for the Gist you want to write to
5. Giat Filename for the CSV file

```bash
CanteenParser.exe <canten-school-id> <canteen-username> <canteen-password> <github-pat> <gist-id> <gist-filename>
```

The menu can then be accessed from the Gist URL. The run action of this repository generates csv files in this gist

https://gist.github.com/simon-k/6ff5c22ee7d43540c0e2767974f27f9e

And looks like this

```csv
Date,When,Kind,Name,Description
02-08-2024,02-08-2024 11:00,Vegetarian,"Mexican bean casule with rice sour cream, jalapenos, vegan tortilla chips","Mexicansk bønnecasule med ris creme fraiche, jalapenos, vegansk tortillia chips"
02-08-2024,02-08-2024 11:00,Meat,"Boeuf bourguignon with carrots, mushrooms and mashed potatoes topped with fresh herbs","Boeuf bourguignon med gulerødder, svampe hertil kartoffelmos toppet med friske urter"
02-08-2024,05-08-2024 11:00,Vegetarian,"Vegetarian Tom Kah Gai with Ingrid peas, bamboo shoots, coconut, lemongrass, lime leaves, peppers and chilli","Vegetarisk Tom Kah Gai med Ingrid ærter, bambusskud, kokos, citrongræs, limeblade peberfrugter og chili"
...
```

Note, the date and time is in CET timezone and in the format dd-MM-yyyy hh:mm.

## TODO
* Make canteen website url an optional parameter
* Make number of days a parameter
* Dependency Injection. Fx the HttpClient should only be created once with a HttpClientFactory https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0
* Add some tests based on the json files in the testdata folder

## W.T.F.
* The Kanpla website is clearly built for schools and not for companies, so a canteen is identified by a School ID and not a Canteen Id. 
* The json that is used to scrape the menu from is highly coupled with the frontend, so it might very well break in the future when Kanpla updates the website. There doesn't seem to be a pretty REST API for getting the menu.
