using ASK.API.Core.Entities;
using ASK.API.Core.Interfaces;
using ASK.API.Helpers;
using ASK.API.Infrastructure.Data;
using ASK.API.Infrastructure.SqlServerImplementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Service.Models;
using UserManagement.Service.Services;

namespace ASK.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddDbContext<AskDbContext>(options =>
               options.UseSqlServer(config.GetConnectionString("AskConnectionString")));

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = config.GetConnectionString("Redis");

                opt.InstanceName = "AskService_";
            });

            services.AddAutoMapper(typeof(MappingProfile));
            var emailConfig = config.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService,TokenService>();

            services.AddIdentityCore<AuthUser>()
    .AddRoles<IdentityRole<long>>()
    .AddTokenProvider<DataProtectorTokenProvider<AuthUser>>("Ask")
    .AddEntityFrameworkStores<AskDbContext>()
    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;
            }
);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))

    });
            services.AddCors();

            return services;

        }
        public static IApplicationBuilder UseApplicationServices(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();

            return app;
        }
    }
}
