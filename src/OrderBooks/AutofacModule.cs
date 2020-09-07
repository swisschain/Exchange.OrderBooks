using Autofac;
using MatchingEngine.Client;
using MatchingEngine.Client.Extensions;
using MyNoSqlServer.Abstractions;
using OrderBooks.Configuration;
using OrderBooks.Managers;
using OrderBooks.MyNoSql;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.MyNoSql.PriceData;
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


            builder.Register(ctx =>
                {
                    return new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<OrderBookEntity>(() => _config.OrderBooksService.MyNoSqlServer.WriterServiceUrl,
                        OrderBookEntity.OrderBookTableName);
                })
                .As<IMyNoSqlServerDataWriter<OrderBookEntity>>()
                .SingleInstance();

            builder.Register(ctx =>
                {
                    return new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<PriceEntity>(() => _config.OrderBooksService.MyNoSqlServer.WriterServiceUrl,
                        PriceEntity.PriceTableName);
                })
                .As<IMyNoSqlServerDataWriter<PriceEntity>>()
                .SingleInstance();
        }
    }
}
