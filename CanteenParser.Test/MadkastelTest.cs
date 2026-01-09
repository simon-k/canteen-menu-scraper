using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CanteenParser.Test;

public class MadkastelTest
{
    //[Fact]
    public async Task Test1()
    {
        var url = "https://madkastel.dk/hubnordic/";
        var web = new HtmlWeb();
        var htmlDoc = web.Load(url);
        
        //  Use Chrome Developer Tools -> Elements -> Copy xpath to get the xpath to the menu
        var hub1KaysDiv = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"post-37\"]/div/div/div/div[2]/div[2]/div[1]/div/div");
        var hub1KaysRawHtml = hub1KaysDiv.InnerHtml;

        var hub1KaysWeekMenu = new Dictionary<string, string>()
        {
            { "Mandag", string.Empty },
            { "Tirsdag", string.Empty },
            { "Onsdag", string.Empty },
            { "Torsdag", string.Empty },
            { "Fredag", string.Empty }
        };
        
        //For each element in the weekMenu
        foreach (var (day, _) in hub1KaysWeekMenu)
        {
            var pattern = $@"<p><strong><u>{day}<\/u><\/strong><\/p>[\s]*?<p>([\s\S]*?)<\/p>";
            var match = Regex.Match(hub1KaysRawHtml, pattern);
            if (match.Success)
            {
                hub1KaysWeekMenu[day] = match.Groups.Count >= 1 ? match.Groups[1].Value : string.Empty;
            }
        }
            
        
        
        //********
        
        var hub1WorldDiv = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"post-37\"]/div/div/div/div[2]/div[2]/div[2]/div/div");
        var hub1WorldRawHtml = hub1WorldDiv.InnerHtml;
        
        var hub1WorldWeekMenu = new Dictionary<string, string>()
        {
            { "SPROUT MENU", string.Empty },
            { "GLOBETROTTER MENU", string.Empty },
            { "ONSDAG I GLOBETROTTER (VEGETAR)", string.Empty },
            { "HOMEBOUND MENU", string.Empty },
            { "ONSDAG I HOMEBOUND (VEGETAR)", string.Empty }
        };
        
        foreach (var (day, _) in hub1WorldWeekMenu)
        {
            var dayEscaped = day.Replace("(", "\\(").Replace(")", "\\)");
            var pattern = $@"<p><strong>{dayEscaped}<\/strong><\/p>[\s]*?<p>([\s\S]*?)<\/p>";
            var match = Regex.Match(hub1WorldRawHtml, pattern);
            if (match.Success)
            {
                hub1WorldWeekMenu[day] = match.Groups.Count >= 1 ? match.Groups[1].Value : string.Empty;
            }
        }
        
    }
}