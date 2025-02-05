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
        /// <summary>
        /// Override OnStartup method. 
        /// Configure the Dependency Injection, set data context to the NavigationService and navigate to the HomeViewModel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
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


        /// <summary>
        /// Configure the service collection for Dependency Injection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Register Services
            // Note all services are Singleton since good practice is for them to not hold state (where possible)
            serviceCollection.AddSingleton<GPXParserService>();
            serviceCollection.AddSingleton<GPXProcessorService>();
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
