using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication6.Providers;
using WebApplication6.Service;

namespace WebApplication6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped((serviceProvider) => PictureProvider.GetInstance(serviceProvider.GetRequiredService<IOptions<ObserverOptions>>().Value));
            services.AddScoped<IObserverService, ObserverService>();
            services.AddScoped<MotionDetection>();

            services.AddHostedService<ActivationService>();
            services.AddHttpClient<IIpProvider, IpProvider>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<KeyStorage>();
            services.AddSingleton<IOwnerDetector, OwnerDetector>();
            
            services.Configure<ObserverOptions>(Configuration.GetSection(nameof(ObserverOptions)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

         //   app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
