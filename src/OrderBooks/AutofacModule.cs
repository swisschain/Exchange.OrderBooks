using Autofac;
using MatchingEngine.Client;
using MatchingEngine.Client.Extensions;
using OrderBooks.Configuration;
using OrderBooks.Managers;
using OrderBooks.RabbitMq.Subscribers;

namespace OrderBooks
{
    public class AutofacModule : Module
    {
        private readonly AppConfig _config;

        public AutofacModule(AppConfig config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .SingleInstance();

            builder.RegisterType<OrderBooksSubscriber>()
                .WithParameter("settings", _config.OrderBooksService.Rabbit.Subscribers.OrderBooks)
                .SingleInstance();

            builder.RegisterMatchingEngineClient(new MatchingEngineClientSettings
            {
                OrderBooksServiceAddress = _config.OrderBooksService.OrderBooksServiceAddress
            });
        }
    }
}
