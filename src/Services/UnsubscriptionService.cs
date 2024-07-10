using Domain.Exceptions;
using Domain.Models;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface IUnsubscriptionService
{
    Task<Subscriptions?> GetById(int subscriptionId);
    Task Delete(int subscriptionId);
}

public class UnsubscriptionService : IUnsubscriptionService
{
    private readonly MyDBContext _myDBContext;
    private readonly ToDoListHttpClient _client;

    public UnsubscriptionService(MyDBContext myDBContext, ToDoListHttpClient toDoListHttpClient)
    {
        _myDBContext = myDBContext;
        _client = toDoListHttpClient;
    }

    public async Task<Subscriptions?> GetById(int subscriptionId)
    {
        return await _myDBContext.Subscriptions.FindAsync(subscriptionId);
    }

    public async Task Delete(int subscriptionId)
    {
        var subscription = await GetById(subscriptionId);

        if (subscription is null) throw new NotFoundException("Subscription not found");

        await _client.DeleteUrlWebhook(subscription.MainTaskIdTopic);
        await _myDBContext.Subscriptions.Where(x => x.Id == subscriptionId).ExecuteDeleteAsync();
    }
}
