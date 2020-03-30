using JetBrains.Annotations;

namespace OrderBooks.Configuration.Service.RabbitMq.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SubscriberSettings
    {
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QueueSuffix { get; set; }
    }
}
