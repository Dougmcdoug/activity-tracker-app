using System.Configuration;
using System.Data;
using System.ServiceProcess;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RunningTrackingApp.Services;
using RunningTrackingApp.ViewModels;

namespace RunningTrackingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);

            // Configure Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Set data context
            var navigationService = serviceProvider.GetRequiredService<NavigationService>();
            navigationService.NavigateTo<HomeViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<NavigationViewModel>()
            };
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Register Services
            // Note I've defined MapService as Transient because it will hold 
            serviceCollection.AddSingleton<GPXParserService>();
            serviceCollection.AddSingleton<NavigationService>();
            serviceCollection.AddSingleton<MapService>();

            // Register ViewModels
            serviceCollection.AddSingleton<NavigationViewModel>();
            serviceCollection.AddTransient<GPXImportViewModel>(); 
            serviceCollection.AddTransient<HomeViewModel>();
            serviceCollection.AddTransient<GPSTraceViewModel>();
        }
    }

}
