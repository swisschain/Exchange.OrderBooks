using JetBrains.Annotations;
using OrderBooks.Configuration.Service.RabbitMq.Subscribers;

namespace OrderBooks.Configuration.Service.RabbitMq
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSettings
    {
        public RabbitSubscribers Subscribers { get; set; }
    }
}
