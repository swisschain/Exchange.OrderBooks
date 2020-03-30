using System.Reflection;
using Autofac;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using OrderBooks.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderBooks.Managers;
using Swisschain.Sdk.Server.Common;

namespace OrderBooks
{
    public sealed class Startup : SwisschainStartup<AppConfig>
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
            AddJwtAuth(Config.Jwt.Secret, "exchange.swisschain.io");
        }

        protected override void ConfigureServicesExt(IServiceCollection services)
        {
            services
                .AddAutoMapper(typeof(AutoMapperProfile))
                .AddControllersWithViews()
                .AddFluentValidation(options =>
                {
                    ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
                    options.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
                });
        }

        protected override void ConfigureExt(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthorization();

            app.ApplicationServices.GetRequiredService<AutoMapper.IConfigurationProvider>()
                .AssertConfigurationIsValid();

            app.ApplicationServices.GetRequiredService<StartupManager>()
                .StartAsync()
                .GetAwaiter()
                .GetResult();
        }

        protected override void ConfigureContainerExt(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule(Config));
            builder.RegisterModule(new Common.Services.AutofacModule());
        }
    }
}
