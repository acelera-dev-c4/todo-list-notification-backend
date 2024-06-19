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
    ToDoListHttpClient _todoListHttpClient = new();

    public SubscriptionService(MyDBContext context)
    {
        _myDBContext = context;
    }

    public async Task<Subscriptions> Create(SubscriptionsRequest subscription)
    {
        var newSubscription = new Subscriptions
        {
            SubTaskIdSubscriber = subscription.SubTaskIdSubscriber,
            MainTaskIdTopic = subscription.MainTaskIdTopic
        };
        
        //_todoListHttpClient.AdviseToDoOfSubscription(subscription.MainTaskIdTopic);
        
        _myDBContext.Subscriptions.Add(newSubscription);
        await _myDBContext.SaveChangesAsync();

        return newSubscription;
    }

}
 