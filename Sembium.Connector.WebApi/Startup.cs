using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.AccessTokenValidation;

namespace Sembium.Connector.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        private string GetAppSetting(string settingName)
        {
            return Configuration.GetSection("AppSettings").GetValue<string>(settingName);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(x =>
                {
                    x.Authority = GetAppSetting("IdentityServiceUrl");
                    x.ApiName = GetAppSetting("IdentityServiceApiName");
                    x.ApiSecret = GetAppSetting("IdentityServiceApiSecret");
                    x.RequireHttpsMetadata = false;
                });

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            var builder = new ContainerBuilder();

            builder.Populate(services);

            ContainerRegistrations.RegisterFor(builder);
            builder.RegisterType<ConnectionConfigProvider>().As<IDataConnectionConfigProvider>();
            builder.RegisterType<SqlConnectionStringProvider>().As<ISqlConnectionStringProvider>();
            builder.RegisterType<ClientConnectionInfoProvider>().As<IClientConnectionInfoProvider>();            
            builder.RegisterInstance(Configuration).As<IConfiguration>();

            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddAWSProvider(this.Configuration.GetAWSLoggingConfigSection());

            app.UseMiddleware<ErrorHandlingMiddleware<UserException, AuthorizeException>>();
            app.UseAuthentication();
            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
