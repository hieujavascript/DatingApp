
using API.Data;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Helpers;
namespace API.extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services , IConfiguration config ) 
            {
            // đăng ký trong này để để có thể Inject vào Controller vào thời điểm Runtime
            // services.Configure đọc dữ liệu trong appsettings.json 
             services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
             services.AddScoped<ITokenService , TokenService>();
             services.AddScoped<IPhotoService , PhotoService>();
             services.AddScoped<IUserRepository , UserRepository>();
             services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
             services.AddDbContext<DataContext>( options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            } );
            return services;
        }
    }
}