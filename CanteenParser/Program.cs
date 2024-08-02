using CanteenParser;
using CanteenParser.Readers;
using CanteenParser.Writers;

var schoolId = args[0];
var username = args[1];
var password = args[2];
var gistToken = args[3];
var gistId = args[4];
var gistFilename = args[5];

var websiteReader = new WebsiteReader();
var websiteContent = await websiteReader.ReadWebsiteContentAsync(username, password, schoolId);

var dishes = MenuParser.GetAllDishes(7, websiteContent);

var csvWriter = new GistCsvWriter(gistToken, gistId, gistFilename);
await csvWriter.Execute(dishes);
