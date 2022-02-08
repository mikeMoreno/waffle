using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Waffle.Lib;

namespace Waffle
{
    internal static class Program
    {
        public static string[] CliArgs { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {   
            ApplicationConfiguration.Initialize();
            
            CliArgs = args;

            SetupApplicationFolder();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => services
                            .AddSingleton<WaffleLib>()
                            .AddSingleton<Main>())
                            .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                Application.Run(services.GetRequiredService<Main>());
            }
        }

        private static void SetupApplicationFolder()
        {
            if (!Directory.Exists(Globals.ApplicationFolder))
            {
                Directory.CreateDirectory(Globals.ApplicationFolder);
            }
        }
    }
}