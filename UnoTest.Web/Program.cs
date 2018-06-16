namespace UnoTest.Web
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using UnoWeb.Test;

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(SetupConfiguration)
            .UseStartup<Startup>()
            .Build();

        private static void SetupConfiguration(
            WebHostBuilderContext ctx,
            IConfigurationBuilder builder)
        {
            // Removing the default configuration options
            builder.Sources.Clear();
            builder // Order is important, last wins
                .AddJsonFile("config.json", false, true)
                //.AddXmlFile("config.xml", true)
                .AddEnvironmentVariables();
        }
    }
}
