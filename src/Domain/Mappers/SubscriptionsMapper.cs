
using Domain.Models;
using Domain.Requests;

namespace Domain.Mappers;

public class SubscriptionsMapper
{
    public static Subscriptions ToClass(SubscriptionsRequest subscriptionsRequest) => new Subscriptions
    {
        Id = null,
        SubTaskIdSubscriber = subscriptionsRequest.SubTaskIdSubscriber,
        MainTaskIdTopic = subscriptionsRequest.MainTaskIdTopic
    };
}