using Domain.Models;
using Domain.Requests;
using Infra;
using Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface ISubscriptionService
{
    Task<Subscriptions> Create(SubscriptionsRequest subscription);
    Task<Subscriptions?> GetSubscriptionBySubTaskId(int subtaskId);
    Task<List<Subscriptions?>> GetSubscriptionByMainTaskId(int maintaskId);

}

public class SubscriptionService : ISubscriptionService
{
    private readonly MyDBContext _myDBContext;
    private readonly ToDoListHttpClient _todoListHttpClient;

    public SubscriptionService(MyDBContext context, ToDoListHttpClient toDoListHttpClient)
    {
        _myDBContext = context;
        _todoListHttpClient = toDoListHttpClient;
    }

    public async Task<Subscriptions> Create(SubscriptionsRequest subscription)
    {
        var newSubscription = new Subscriptions
        {
            SubTaskIdSubscriber = subscription.SubTaskIdSubscriber,
            MainTaskIdTopic = subscription.MainTaskIdTopic
        };

        await _todoListHttpClient.SetUrlWebhook(subscription.MainTaskIdTopic);

        _myDBContext.Subscriptions.Add(newSubscription);
        await _myDBContext.SaveChangesAsync();

        return newSubscription;
    }
    public async Task<Subscriptions?> GetSubscriptionBySubTaskId(int subtaskId)
    {
        return await _myDBContext.Subscriptions.Where(s => s.SubTaskIdSubscriber == subtaskId).FirstOrDefaultAsync();
    }

    public async Task<List<Subscriptions>?> GetSubscriptionByMainTaskId(int maintaskId)
    {
        return await _myDBContext.Subscriptions.Where(s => s.MainTaskIdTopic == maintaskId).ToListAsync();
    }

}
