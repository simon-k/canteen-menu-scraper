using CanteenParser;
using CanteenParser.Domain;
using CanteenParser.Readers;
using CanteenParser.Writers;

var canteenId = args[0];
var username = args[1];     //TODO: Make user and password optional
var password = args[2];
var aiKey = args[3];
var gistToken = args[4];
var gistId = args[5];
var gistFilename = args[6];

List<Dish> dishes;

if (canteenId == "Hub1")
{
    /*var websiteReader = new HubNordicWebsiteReader();
    var websiteContent = websiteReader.ReadWebsiteContentAsync();
    dishes = HubNordicMenuParser.GetAllDishes(websiteContent);*/
    var menuReader = new HubNordicAiReader();
    var menu = await menuReader.ReadAsync(aiKey);
    dishes = HubNordicMenuParser.GetAllDishes(menu);
}
else
{
    var websiteReader = new KanplaWebsiteReader();
    var websiteContent = await websiteReader.ReadWebsiteContentAsync(username, password, canteenId);
    dishes = KanplaMenuParser.GetAllDishes(7, websiteContent);    
}

var csvWriter = new GistCsvWriter(gistToken, gistId, gistFilename);
await csvWriter.Execute(dishes);
