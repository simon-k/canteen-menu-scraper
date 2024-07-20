using System.Text.Json;
using CanteenParser;
using CanteenParser.Domain;

var username = args[0];
var password = args[1];
var tenant = args[2];
var clientId = args[3];
var clientSecret = args[4];
var scope = args[5];

var websiteReader = new WebsiteReader();
var websiteContent = await websiteReader.ReadWebsiteContentAsync(username, password);

var frontend = JsonSerializer.Deserialize<Frontend>(websiteContent, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
})!;

var logPoster = new LogPoster(tenant, clientId, clientSecret, scope);
for(int i = 0; i < 7; i++)
{
    try
    {
        var vegetarianDish = MenuReader.GetVegetarianDish(frontend, -i);
        var dish = MenuReader.GetDish(frontend, -i);
        
        Console.WriteLine($"Menu: {dish}");
        Console.WriteLine($"Vegetarian menu: {vegetarianDish}");
        
        await logPoster.Execute(dish, vegetarianDish);
    }
    catch (Exception e)
    {
        Console.WriteLine("Could not find the days menu");
    }
}
