using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.IO;

namespace WebApplication6
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if !DEBUG
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            Log.Logger = new LoggerConfiguration().WriteTo.File("./logs/startup.log.txt", rollingInterval:RollingInterval.Day, retainedFileCountLimit:3).CreateLogger();
            try
            {
                CreateHostBuilder(args).UseSerilog((context, service, configuration) => configuration.ReadFrom.Configuration(context.Configuration)).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Can't start application");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
