using JetBrains.Annotations;

namespace OrderBooks.Configuration.Service.RabbitMq.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSubscribers
    {
        public SubscriberSettings OrderBooks { get; set; }
    }
}
