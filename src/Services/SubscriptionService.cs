using Domain.Models;
using Domain.Requests;
using Infra.DB;

namespace Services;

public interface ISubscriptionService
{
    Task<Subscriptions> Create(SubscriptionsRequest subscription);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly MyDBContext _myDBContext;
   
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

        _myDBContext.Subscriptions.Add(newSubscription);
        await _myDBContext.SaveChangesAsync();

        return newSubscription;
    }

}
 