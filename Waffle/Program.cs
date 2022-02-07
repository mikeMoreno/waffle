using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows.Forms;
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
            CliArgs = args;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new Main(new WaffleLib()));

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
    }
}