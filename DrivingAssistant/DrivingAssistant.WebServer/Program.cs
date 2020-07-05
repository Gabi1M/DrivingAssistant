using DrivingAssistant.Core.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DrivingAssistant.WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.SetFilename("WebServer.log");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
