using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Waffle.History;
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

            SetupApplicationFolders();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => services
                            .AddSingleton<HistoryService>()
                            .AddSingleton<WaffleLib>()
                            .AddSingleton<Navigator>())
                            .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                Application.Run(services.GetRequiredService<Navigator>());
            }
        }

        private static void SetupApplicationFolders()
        {
            if (!Directory.Exists(Globals.ApplicationFolder))
            {
                Directory.CreateDirectory(Globals.ApplicationFolder);
            }

            if (!Directory.Exists(Globals.HistoryFolder))
            {
                Directory.CreateDirectory(Globals.HistoryFolder);
            }
        }
    }
}

// TODO: all classes that can be: make internal