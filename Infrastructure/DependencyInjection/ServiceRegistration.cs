using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, string connectionString)
        {
            // db
            services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

            // repo
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ILaundryRoomRepository, LaundryRoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // app service
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ILaundryRoomService, LaundryRoomService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
