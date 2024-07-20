using System.Net.Http.Json;
using System.Text;
using CanteenParser.Domain;

namespace CanteenParser;

public class LogPoster
{
    private readonly string _tenantId;
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _scope;
    private string _token = string.Empty;

    public LogPoster(string tenantId, string clientId, string clientSecret, string scope)
    {
        _tenantId = tenantId;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _scope = scope;
        _httpClient = new HttpClient();
    }
    
    public async Task Execute(Dish regular, Dish vegetarian)
    {
        if (_token == string.Empty)
            _token = await GetAuthIdAsync(_tenantId, _clientId, _clientSecret, _scope);
            
        if (regular.Name != string.Empty)
        {
            var kind = GetKind(regular.Name);
            var jsonRegularMenuString = GenerateJsonString(regular.Name, regular.Date, kind);
            await PostMenuAsync(jsonRegularMenuString, _token);    
        }
        
        if (vegetarian.Name != string.Empty)
        {
            var jsonVegetarianMenuString = GenerateJsonString(vegetarian.Name, vegetarian.Date, "Vegetarian");
            await PostMenuAsync(jsonVegetarianMenuString, _token);
        }
    }

    private static string GetKind(string name)
    {
        if (name.ToLower().Contains("fish"))
        {
            return "Fish";
        }

        if (name.ToLower().Contains("pork"))
        {
            return "Pork";
        }

        if (name.ToLower().Contains("chicken"))
        {
            return "Chicken";
        }
       
        return "Meat";
    }

    private async Task PostMenuAsync(string jsonMenuString, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var content = new StringContent(jsonMenuString, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.bifrost.heimdall.novonordisk.cloud/otlp/http/v1/logs", content);
        Console.WriteLine(response.StatusCode);
    }

    private static string GenerateJsonString(string menu, DateTimeOffset date, string kind)
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1_000_000;
        return $$"""
                       {
                           "resourceLogs": [
                               {
                                   "resource": {
                                       "attributes": [
                                           {
                                               "key": "service.name",
                                               "value": {
                                                   "stringValue": "Canteen Reader"
                                               }
                                           }
                                       ]
                                   },
                                   "scopeLogs": [
                                       {
                                           "logRecords": [
                                               {
                                                   "timeUnixNano": {{timestamp}},
                                                   "observedTimeUnixNano": {{timestamp}},
                                                   "severityText": "Information",
                                                   "body": {
                                                       "stringValue": "Canteen menu: {date} {kind} - {menu}"
                                                   },
                                                   "attributes": [
                                                       {
                                                           "key": "menu",
                                                           "value": {
                                                               "stringValue": "{{menu}}"
                                                           }
                                                       },
                                                       {
                                                           "key": "date",
                                                           "value": {
                                                               "stringValue": "{{date.ToString("dd-MM-yyyy")}}"
                                                           }
                                                       },
                                                       {
                                                           "key": "when",
                                                           "value": {
                                                               "stringValue": "{{date.AddHours(11).ToString("dd-MM-yyyy HH:mm")}}"
                                                           }
                                                       },
                                                       {
                                                           "key": "kind",
                                                           "value": {
                                                               "stringValue": "{{kind}}"
                                                           }
                                                       }
                                                   ]
                                               }
                                           ]
                                       }
                                   ]
                               }
                           ]
                       }
               """;
    }
    
    private async Task<string> GetAuthIdAsync(string tenantId, string bifrostClientId, string bifrostClientSecret, string bifrostDfProdClientId)
    {
        var content = new Dictionary<string, string>();
        content.Add("grant_type", "client_credentials");
        content.Add("client_id", bifrostClientId);
        content.Add("client_secret", bifrostClientSecret);
        content.Add("scope", $"{bifrostDfProdClientId}/.default");
        
        //using var client = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token") { Content = new FormUrlEncodedContent(content) };
        using var res = await _httpClient.SendAsync(req);

        var authResponse = await res.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse.Access_Token;
    }
    
    private class AuthResponse
    {
        public string Token_Type { get; set; } = string.Empty;
        public int Expires_In { get; set; }
        public string Access_Token { get; set; } = string.Empty;
    }
}