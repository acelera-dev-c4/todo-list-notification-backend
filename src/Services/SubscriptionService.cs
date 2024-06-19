using System.Net;
using System.Net.Http.Json;
using Azure.Core;
using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface ISubscriptionService
{
    Task<IEnumerable<Subscriptions>> GetSubscriptions();
    Task<Subscriptions> Create(SubscriptionsRequest subscription);

    Task<MainTask> GetMainTask(MainTask mainTask);

}

public class SubscriptionService : ISubscriptionService
{
    private readonly MyDBContext _context;
    private readonly HttpClient _httpClient; 


    public SubscriptionService(MyDBContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<Subscriptions> Create(SubscriptionsRequest subscription)
    {
        var newSubscription = new Subscriptions
        {
            SubTaskIdSubscriber = subscription.SubTaskIdSubscriber,
            MainTaskIdTopic = subscription.MainTaskIdTopic
        };

        _context.Subscriptions.Add(newSubscription);
        await _context.SaveChangesAsync();

        return newSubscription;
    }

    public async Task<MainTask> GetMainTask(MainTask mainTask)
    {      
        await _context.FindAsync<MainTask>(mainTask.Id);

        await NotifySubscriptionCreated();
        return mainTask;
    }
    public async Task<IEnumerable<Subscriptions>> GetSubscriptions()
    {
        var subscriptions = await _context.Subscriptions.ToListAsync();

        await NotifySubscriptionCreated();
        
        return subscriptions;

    }

    private async Task NotifySubscriptionCreated()
    {
        
        
        //var notificationPayload = new { SubscriptionId = subscription.Id, Message = "Nova inscrição criada" };

        
        var response = await _httpClient.GetFromJsonAsync<HttpResponseMessage>("hook");
        // pergar messagens de status code para enviar de volta ao servidor
        var statusMessage = response!.StatusCode;

        if (statusMessage == HttpStatusCode.BadRequest)
        {
            throw new Exception("Falha ao notificar nova inscrição");
        }
        {
            throw new Exception("Falha ao notificar nova inscrição");
        }

    }



}
 