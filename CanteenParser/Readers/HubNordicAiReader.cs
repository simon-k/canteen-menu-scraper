using System.Text.Json;
using CanteenParser.Domain;
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
                     Given the html below, what is the menu? The website is in Danish.
                     
                     The Kays menu is in the HTML table column with the headline "HUB1 – Kays".
                     The Globetrotter, Homebound and Sprout menus are in the HTML table column named "HUB1 – Kays Verdenskøkken".
                     
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
        return html;
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
}
