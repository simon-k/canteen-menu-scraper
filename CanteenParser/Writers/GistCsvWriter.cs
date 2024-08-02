using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CanteenParser.Domain;
using CsvHelper;

namespace CanteenParser.Writers;

public class GistCsvWriter(string authToken, string gistId, string filename)
{
    private readonly HttpClient _httpClient = new();  //TODO: Inject this with DI

    public async Task Execute(List<Dish> dishes)
    {
        var cvs = await DishesToCsvAsync(dishes);
        
        var gistPost = new GistPost
        {
            Files = new Dictionary<string, GistFile>
            {
                {
                    filename, new GistFile
                    {
                        Content = cvs
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Patch, $"https://api.github.com/gists/{gistId}");
        request.Headers.Authorization = new("Bearer", authToken);
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Headers.Add("User-Agent", "CanteenParser");
        request.Content = JsonContent.Create(gistPost, options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Could not post to gist. {response.Content.ReadAsStringAsync()}");
        
        Console.WriteLine($"Wrote menu to gist with id {gistId} and filename {filename}");
    }
    
    private async Task<string> DishesToCsvAsync(List<Dish> dishes)
    {
        await using var stringWriter = new StringWriter();
        await using var csvWriter = new CsvWriter(stringWriter, CultureInfo.InvariantCulture, true);
        await csvWriter.WriteRecordsAsync(dishes);
        await csvWriter.FlushAsync();
        var csv = stringWriter.ToString();
        return csv;
    }
    
    private class GistPost
    {
        public required Dictionary<string, GistFile> Files { get; set; }
    }
    
    private class GistFile
    {
        public required string Content { get; set; }
    }
}

