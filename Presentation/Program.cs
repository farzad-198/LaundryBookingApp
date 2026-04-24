using Infrastructure.Data;
using Infrastructure.DependencyInjection;
using Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Startup;

namespace Presentation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

            ServiceCollection services = new ServiceCollection();

            services.RegisterServices(connectionString);
            services.AddScoped<AppRunner>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            using IServiceScope scope = serviceProvider.CreateScope();

            AppRunner appRunner = scope.ServiceProvider.GetRequiredService<AppRunner>();
            await appRunner.RunAsync();
        }
    }
}
