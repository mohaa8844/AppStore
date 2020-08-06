using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Controllers;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Handlers;
using AppStoreApiWithIdentityServer4.Models;
using ClickHouse.Ado;
using ClickHouse.Net;
using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace AppStoreApiWithIdentityServer4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private IConfiguration config;
        private IHostEnvironment env;
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath);
            builder.AddJsonFile("appsettings.json");
            config = builder.Build();
            this.env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });


            services.AddSignalR();
            services.AddSingleton<NotificationsHub>();
            services.AddClickHouse();
            services.AddTransient(p => new ClickHouseConnectionSettings("Host =192.168.84.128; Port=9000; User = default; Password =osboxes.org; Database = AppStore;Compress=True;CheckCompressedHash=False;SocketTimeout=600;Compressor=lz4"));
            services.AddSingleton<ClickHouseDatabase>();


            services.AddControllersWithViews();
            services.AddAuthorization();
            services.AddDbContext<AppStoreContext>(options =>options.UseSqlServer(config.GetConnectionString("AppStoreConnection")));
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppStoreContext>()
                .AddDefaultTokenProviders();
            services.AddSingleton<IHostEnvironment>(env);

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<GraphSchema>();
            services.AddGraphQL().AddGraphTypes(ServiceLifetime.Scoped);

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseGraphQL<GraphSchema>("/queries");
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationsHub>("/notificationsHub");
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
