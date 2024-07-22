using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CanteenParser.Domain;

namespace CanteenParser.Readers;

public class WebsiteReader
{
    public async Task<WebsiteContent> ReadWebsiteContentAsync(string username, string password)
    {
        var authId = await GetAuthIdAsync(username, password);
        var frontendJson = await GetFrontendAsync(authId);
        
        var frontendContent = JsonSerializer.Deserialize<WebsiteContent>(frontendJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        return frontendContent;
    }
    
    private async Task<string> GetFrontendAsync(string authId)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authId);
        
        var response = await client.PostAsync("https://novo.foodandco.dk/api/internal/load/frontend", new StringContent(""));

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    private async Task<string> GetAuthIdAsync(string email, string password)
    {
        using var client = new HttpClient();
       
        using var response = await client.PostAsync(
            "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyDDtuovWpK6PARKIt9wUqaTQP7MjFWIWF4", 
            new StringContent($"{{\"returnSecureToken\":true,\"email\":\"{email}\",\"password\":\"{password}\"}}"));
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Could not authenticate to the canteen menu website");
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!.IdToken;
    }
}

public class AuthResponse
{
    public string Kind { get; set; } = string.Empty;
    public string LocalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public bool Registered { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public string ExpiresIn { get; set; } = string.Empty;
}

