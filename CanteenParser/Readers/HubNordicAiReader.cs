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
        
        var prompt = "Given the canteen website https://madkastel.dk/hubnordic/ what is the menu? ALWAYS get the latest content from the website. Do not translate the menu to english, keep the menu in danish";
        var result = await kernel.InvokePromptAsync(prompt, new(settings));
        var menu = JsonSerializer.Deserialize<Hub1Menu>(result.ToString()) ?? throw new Exception($"Could not deserialize response from model. It seems like the structured output does not follow the requested schema. Response: {result}");

        return menu;
    }

    private Kernel GetKernel(string openAiApiKey)
    {
        var handler = new HttpClientHandler();
        handler.CheckCertificateRevocationList = false;
        var client = new HttpClient(handler);
        
        var kernelBuilder = Kernel.CreateBuilder();
        
        var kernel = kernelBuilder
            .AddOpenAIChatCompletion("gpt-4o-mini-search-preview", openAiApiKey, httpClient: client) 
            .Build();

        return kernel;
    }
}
