using Autofac;
using OrderBooks.Common.Domain.Handlers;
using OrderBooks.Common.Domain.Services;

namespace OrderBooks.Common.Services
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OrderBooksService>()
                .As<IOrderBooksService>()
                .SingleInstance();

            builder.RegisterType<OrderBooksHandler>()
                .As<IOrderBooksHandler>()
                .SingleInstance();
        }
    }
}
