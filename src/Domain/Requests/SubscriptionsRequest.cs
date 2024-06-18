namespace Domain.Requests;

    public class SubscriptionsRequest
    {
        public int SubTaskIdSubscriber { get; set; }
        public int MainTaskIdTopic { get; set; }
    }
