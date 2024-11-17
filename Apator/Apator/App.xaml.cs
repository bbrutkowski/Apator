using Apator.Services;
using Apator.Services.Interface;
using Apator.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Apator
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var dbContext = ServiceProvider.GetRequiredService<DataContext>();
            dbContext.EnsureSeedData();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer("Data Source=.;Initial Catalog=ApatorDB;Integrated Security=True;TrustServerCertificate=True;"));

            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddCardWindow>();
            services.AddTransient<EditCardWindow>();
            services.AddScoped<ICardService, CardService>();

            // Add additional services if needed
        }
    }
}
