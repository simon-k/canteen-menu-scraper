using System.Text.RegularExpressions;
using CanteenParser.Domain;
using HtmlAgilityPack;

namespace CanteenParser.Readers;

public class HubNordicWebsiteReader
{
    public HubNordicWebsiteContent ReadWebsiteContentAsync()
    {
        var htmlDoc = GetHtmlDocument("https://madkastel.dk/hubnordic/");

        var kaysHtml = GetKaysHtml(htmlDoc);
        var kaysWeekMenu = GetKaysWeekMenu(kaysHtml);

        var worldHtml = GetWorldHtml(htmlDoc);
        var worldWeekMenu = GetKWorldWeekMenu(worldHtml);
        
        return new HubNordicWebsiteContent
        {
            KaysWeekMenu = kaysWeekMenu,
            WorldWeekMenu = worldWeekMenu
        };
    }

    private Dictionary<string, string> GetKWorldWeekMenu(string kaysHtml)
    {
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
            var match = Regex.Match(kaysHtml, pattern);
            if (match.Success)
            {
                hub1WorldWeekMenu[day] = match.Groups.Count >= 1 ? match.Groups[1].Value : string.Empty;
            }
        }

        return hub1WorldWeekMenu;
    }

    private Dictionary<string, string> GetKaysWeekMenu(string kaysHtml)
    {
        var kaysWeekMenu = new Dictionary<string, string>()
        {
            { "Mandag", string.Empty },
            { "Tirsdag", string.Empty },
            { "Onsdag", string.Empty },
            { "Torsdag", string.Empty },
            { "Fredag", string.Empty }
        };
        
        foreach (var (day, _) in kaysWeekMenu)
        {
            var pattern = $@"<p><strong><u>{day}<\/u><\/strong><\/p>[\s]*?<p>([\s\S]*?)<\/p>";
            var match = Regex.Match(kaysHtml, pattern);
            if (match.Success)
            {
                kaysWeekMenu[day] = match.Groups.Count >= 1 ? match.Groups[1].Value : string.Empty;
            }
        }

        return kaysWeekMenu;
    }

    private string GetWorldHtml(HtmlDocument htmlDoc)
    {
        var hub1WorldDiv = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"post-37\"]/div/div/div/div[2]/div[2]/div[2]/div/div");
        var hub1WorldRawHtml = hub1WorldDiv.InnerHtml;
        return hub1WorldRawHtml;
    }    
    
    private string GetKaysHtml(HtmlDocument htmlDoc)
    {        
        var hub1KaysDiv = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"post-37\"]/div/div/div/div[2]/div[2]/div[1]/div/div");
        var hub1KaysRawHtml = hub1KaysDiv.InnerHtml;
        return hub1KaysRawHtml;
    }

    private HtmlDocument GetHtmlDocument(string url)
    {
        var web = new HtmlWeb();
        return web.Load(url);
    }
}