
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services , IConfiguration config ) 
            {
             services.AddScoped<ITokenService , TokenService>();
             services.AddDbContext<DataContext>( options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            } );
            return services;
        }
    }
}