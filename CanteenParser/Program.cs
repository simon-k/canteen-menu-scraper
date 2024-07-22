using CanteenParser;
using CanteenParser.Readers;
using CanteenParser.Writers;

var username = args[0];
var password = args[1];
var gistToken = args[2];
var gistId = args[3];

var websiteReader = new WebsiteReader();
var websiteContent = await websiteReader.ReadWebsiteContentAsync(username, password);

var dishes = MenuParser.GetAllDishes(7, websiteContent);

var csvWriter = new GistCsvWriter(gistToken, gistId);
await csvWriter.Execute(dishes);
