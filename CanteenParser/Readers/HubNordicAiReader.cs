using System.Text.Json;
using CanteenParser.Domain;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace CanteenParser.Readers;

#pragma warning disable SKEXP0010
public class HubNordicAiReader
{
    public Task<Hub1Menu> ReadAsync(string openAiApiKey)
    {
        var kernel = GetKernel(openAiApiKey);
        var menu = PromptForMenu(kernel);
        return menu;
    }

    private async Task<Hub1Menu> PromptForMenu(Kernel kernel)
    {
        var settings = new OpenAIPromptExecutionSettings()
        {
            ResponseFormat = typeof(Hub1Menu)
        };
        
        var html = await GetHtmlContentAsync("https://madkastel.dk/hubnordic/");
        var prompt = $"""
                     Find ugens menu som findes i nedenstående HTML. Menuen er på dansk.
                     
                     Kays menu er i tabellen som har titlen "HUB1 – Kays".
                     Globetrotter, Homebound og Sprout menuerne er i tabellen med titlen "HUB1 – Kays Verdenskøkken".
                     
                     Dagene i ugen er: Mandag, Tirsdag, Onsdag, Torsdag, Fredag.
                     Dage være grupperet med komma, semikolon eller skråstreg. Fx "Mandag, Tirsdag, Torsdag og Fredag" eller "Mandag/Tirsdag"
                     
                     Hvis der ikke er en menu for en dag eller restuaranten er lukket, så skriv "Lukket" for den dag.
                     
                     {html}
                     """;
        var result = await kernel.InvokePromptAsync(prompt, new(settings));
        var menu = JsonSerializer.Deserialize<Hub1Menu>(result.ToString()) ?? throw new Exception($"Could not deserialize response from model. It seems like the structured output does not follow the requested schema. Response: {result}");

        return menu;
    }

    public async Task<string> GetHtmlContentAsync(string url)
    {
        using var handler = new HttpClientHandler();
        handler.CheckCertificateRevocationList = false;
        using var client = new HttpClient(handler);
        var html = await client.GetStringAsync(url);
        
        // In the html, find the dic with the class containing "et_pb_column et_pb_column_1_4 et_pb_column_1"
        // and return the inner HTML of that div.
        
        var htmlDoc = GetHtmlDocument("https://madkastel.dk/hubnordic/");
        
        var kaysDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'et_pb_column et_pb_column_1_4 et_pb_column_1')]") ??
                      htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'et_pb_column et_pb_column_1_2 et_pb_column_1')]");

        var worldDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'et_pb_column et_pb_column_1_4 et_pb_column_2')]");

        if (kaysDiv == null)
        {
            kaysDiv = htmlDoc.CreateElement("div");
            kaysDiv.InnerHtml = """
                                <h1>HUB1 – Kays</h1>
                                <p>Mandag: Lukket</p>
                                <p>Tirsdag: Lukket</p>
                                <p>Onsdag: Lukket</p>
                                <p>Torsdag: Lukket</p>
                                <p>Fredag: Lukket</p>
                                """;
        }
        if (worldDiv == null)
        {
            worldDiv = htmlDoc.CreateElement("div");
            worldDiv.InnerHtml = """
                                 <h1>Globetrotter</h1>
                                 <p>Mandag: Lukket</p>
                                 <p>Tirsdag: Lukket</p>
                                 <p>Onsdag: Lukket</p>
                                 <p>Torsdag: Lukket</p>
                                 <p>Fredag: Lukket</p>
                                 <h1>Sprout</h1>
                                 <p>Mandag: Lukket</p>
                                 <p>Tirsdag: Lukket</p>
                                 <p>Onsdag: Lukket</p>
                                 <p>Torsdag: Lukket</p>
                                 <p>Fredag: Lukket</p>
                                 <h1>Homebound</h1>
                                 <p>Mandag: Lukket</p>
                                 <p>Tirsdag: Lukket</p>
                                 <p>Onsdag: Lukket</p>
                                 <p>Torsdag: Lukket</p>
                                 <p>Fredag: Lukket</p>
                                 """;
        }
        
        var divHtml = kaysDiv.InnerHtml + worldDiv.InnerHtml;
        
        return divHtml;
    }

    private Kernel GetKernel(string openAiApiKey)
    {
        var handler = new HttpClientHandler();
        handler.CheckCertificateRevocationList = false;
        var client = new HttpClient(handler);
        
        var kernelBuilder = Kernel.CreateBuilder();
        
        var kernel = kernelBuilder
            .AddOpenAIChatCompletion("gpt-4.1-nano", openAiApiKey, httpClient: client) 
            .Build();

        return kernel;
    }
    
    private HtmlDocument GetHtmlDocument(string url)
    {
        var web = new HtmlWeb();
        return web.Load(url);
    }
}
