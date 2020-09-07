using OrderBooks.Configuration.Jwt;
using OrderBooks.Configuration.Service;

namespace OrderBooks.Configuration
{
    public class AppConfig
    {
        public OrderBooksServiceSettings OrderBooksService { get; set; }

        public JwtConfig Jwt { get; set; }

        public MyNoSqlConfig MyNoSqlServer { get; set; }
    }
}
