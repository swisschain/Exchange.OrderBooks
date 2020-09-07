using OrderBooks.Configuration.Service.RabbitMq;

namespace OrderBooks.Configuration.Service
{
    public class OrderBooksServiceSettings
    {
        public string OrderBooksServiceAddress { get; set; }

        public RabbitSettings Rabbit { get; set; }
    }
}
