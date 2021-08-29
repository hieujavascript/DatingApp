using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services , 
            IConfiguration config) 
            {
            
            services.AddIdentityCore<AppUser>(opt => {
                // password đơn giản
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
            // do add AddAuthentication tu Nuget Microst.aspnetcore.authentication nen add o day
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => 
                    {
                        // cho phep Client send Token kieu Query String
                        // Thêm đăng ký AllowCredentials trong Cors StartUp để cho phép send Access_Token
                        var accessToken = context.Request.Query["access_token"];                        
                        var path = context.HttpContext.Request.Path;                        
                // StartsWithSegments("/hubs") khới với endpoints.MapHub<PresenceHub>("hubs/presence");
                // trong file StartUp.cs
                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
                            context.Token = accessToken;
                        }                       

                        return Task.CompletedTask;
                    }
                };
            });
            // policy viết trong admin controller
            services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole" , policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole" , policy => policy.RequireRole("Admin" , "Moderator"));
            });

            return services;
            }
    }
}