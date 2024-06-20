using Azure;
using Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Infra;

public class ToDoListHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ToDoListHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7142/");
        _configuration = configuration;
    }
    // exemplo, nao esta funcionando ainda
    public async void UpdateMainTaskAsFinished(int mainTaskId)
    {
        HttpContent? placeholder = null;
        await _httpClient.PostAsync($"{_httpClient.BaseAddress}Notification/{mainTaskId}", placeholder);
    }

    public async Task<HttpResponseMessage> AdviseToDoOfSubscription(int _mainTaskId)
    {
        var login = new
        {
            email = _configuration["SystemUser:Email"],
            password = _configuration["SystemUser:Password"]
        };

        string jsonLogin = JsonConvert.SerializeObject(login);
        HttpContent loginContent = new StringContent(jsonLogin, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}Auth", loginContent);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        string tokenFromAuth;

        using (JsonDocument doc = JsonDocument.Parse(responseBody))
        {
            JsonElement root = doc.RootElement;
            JsonElement tokenElement = root.GetProperty("token");
            tokenFromAuth = tokenElement.GetProperty("token").GetString()!;
        }

        var payload = new
        {
            url = $"{_httpClient.BaseAddress}Notification",
            mainTaskId = _mainTaskId,
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromAuth);

        string jsonPayload = JsonConvert.SerializeObject(payload);
        HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var result = await _httpClient.PutAsync($"{_httpClient.BaseAddress}MainTask/SetUrlWebhook", content);
        return result;
    }

   
    public async Task<string> GetJWTAsync()
    {
        var authPath = "https://localhost:7142/Auth";
        var requestBody = new { Email = "igor@mail.com", Password ="123456" };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(authPath, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseContent) ?? throw new Exception("Invalid response");

        return responseModel.Token.Token;
    }

}