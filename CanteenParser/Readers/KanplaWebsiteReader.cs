using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CanteenParser.Domain;

namespace CanteenParser.Readers;

public class KanplaWebsiteReader
{
    public async Task<KanplaWebsiteContent> ReadWebsiteContentAsync(string username, string password, string schoolId)
    {
        var authId = await GetAuthIdAsync(username, password);
        var frontendJson = await GetFrontendAsync(authId, schoolId);
        
        var frontendContent = JsonSerializer.Deserialize<KanplaWebsiteContent>(frontendJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        return frontendContent;
    }
    
    private async Task<string> GetFrontendAsync(string authId, string schoolId)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authId);
        
        var payload = JsonContent.Create(
            inputValue: new FrontendPayload
            {
                SchoolId = schoolId
            }, 
            options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        
        var response = await client.PostAsync("https://novo.foodandco.dk/api/internal/load/frontend", payload);

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    private async Task<string> GetAuthIdAsync(string email, string password)
    {
        using var client = new HttpClient();
       
        using var response = await client.PostAsync(
            "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyDDtuovWpK6PARKIt9wUqaTQP7MjFWIWF4", 
            new StringContent($"{{\"returnSecureToken\":true,\"email\":\"{email}\",\"password\":\"{password}\"}}"));  //TODO: Use a proper JSON serializer
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Could not authenticate to the canteen menu website");
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!.IdToken;
    }
    
    private class AuthResponse
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

    private class FrontendPayload
    {
        public required string SchoolId { get; set; }
    }
    
}


