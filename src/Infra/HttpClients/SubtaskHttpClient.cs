using Domain.Exceptions;
using Domain.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;


namespace Services;

public interface ISubTaskHttpClient
{
    Task<string> GetJWTAsync();
    Task UpdateSubtaskAsync(int subtaskId, string jwt);
}

public class SubtaskHttpClient : ISubTaskHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ToDoListOptions _options;

    public SubtaskHttpClient(IHttpClientFactory httpClientFactory, IOptions<ToDoListOptions> options)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient("toDoClient");
    }

    public async Task<string> GetJWTAsync()
    {
        var authPath = _options.Auth;
        var requestBody = new { Email = _options.Email, Password = _options.Password };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(authPath, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseContent) ?? throw new Exception("Invalid response");

        return responseModel.Token.Token;
    }

    public async Task UpdateSubtaskAsync(int subtaskId, string jwt)
    {
        var subtaskPath = $"{_options.Subtask}/{subtaskId}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var requestBody = new { finished = true };
        string json = JsonConvert.SerializeObject(requestBody);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PutAsync(subtaskPath, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new NotFoundException("Failed to update subtask");
        }
    }
}