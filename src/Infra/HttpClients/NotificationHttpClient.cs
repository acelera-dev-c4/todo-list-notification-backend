using Domain;
using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Infra.HttpClients;
public interface INotificationHttpClient
{
    Task<string> GetJWTAsync();
    Task<List<MainTask>> GetMainTaskByUserId(int userId);
    //Task <string>GetMainTaskByUserId(int subscriptionId);


}

public class NotificationHttpClient : INotificationHttpClient
{
    private readonly HttpClient _httpClient;

    public NotificationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7142/");
    }
    public async Task<string> GetJWTAsync()
    {
        var authPath = "https://localhost:7142/Auth";
        var requestBody = new { Email = "igor@mail.com", Password = "123456" };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(authPath, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseContent) ?? throw new Exception("Invalid response");

        return responseModel.Token.Token;
    }

    public async Task<List<MainTask>> GetMainTaskByUserId(int userId)
    {
        var jwt = await GetJWTAsync(); // Supondo que você tenha um método para obter o JWT
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var mainTaskPath = $"https://localhost:7142/MainTask/{userId}";
        var response = await _httpClient.GetAsync(mainTaskPath);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        List<MainTask> mainTasks = JsonConvert.DeserializeObject<List<MainTask>>(responseContent) ?? new List<MainTask>();

        return mainTasks;
    }
}

    /*public async Task<string> GetMainTaskByUserId(int userId)
    {
        var jwt = await GetJWTAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var mainTaskPath = $"https://localhost:7142/MainTask/{userId}";
        
        var tasks= await _httpClient.GetAsync(mainTaskPath);
        List<MainTask> mainTasks = JsonConvert.DeserializeObject<List<MainTask>>(jsonContent);
        return mainTasks;
        //var response = await _httpClient.GetAsync(mainTaskPath);
        tasks.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var subscription = JsonConvert.DeserializeObject<List<MainTask>>(responseContent) ?? throw new Exception("MainTask not found");

        ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseContent) ?? throw new Exception("Invalid response");
        return responseModel.UserData.Name;
      */















    //var mainTaskPath = $"https://localhost:7142/MainTask{subscription.MainTaskIdTopic}";
    //response = await _httpClient.GetAsync(mainTaskPath);
    // response.EnsureSuccessStatusCode();

    // responseContent = await response.Content.ReadAsStringAsync();
    //var mainTask = JsonConvert.DeserializeObject<MainTask>(responseContent) ?? throw new Exception("MainTask not found");


    /*public async Task<int?> GetUserIdBySubscriptionIdAsync(int subscriptionsId)
    {
        var jwt = await GetJWTAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var subscriptionPath = $"https://localhost:7056/Subscription?subscriptionId/{subscriptionsId}";
        var response = await _httpClient.GetAsync(subscriptionPath);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var subscription = JsonConvert.DeserializeObject<Subscriptions>(responseContent) ?? throw new Exception("subscription not found");

        var mainTaskPath = $"https://localhost:7142/MainTask{subscription.MainTaskIdTopic}";
        response = await _httpClient.GetAsync(mainTaskPath);
        response.EnsureSuccessStatusCode();

        responseContent = await response.Content.ReadAsStringAsync();
        var mainTask = JsonConvert.DeserializeObject<MainTask>(responseContent) ?? throw new Exception("MainTask not found");
        return mainTask.UserId;
    }*/



