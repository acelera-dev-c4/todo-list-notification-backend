using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Infra;

namespace Services;

public interface ISubscriptionService
{
    Task<Subscriptions> Create(SubscriptionsRequest subscription);
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

}
 