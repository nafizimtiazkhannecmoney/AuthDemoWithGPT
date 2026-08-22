
using System.Text;
using Authdemo.Data;
using Authdemo.Entities;
using Authdemo.Models;
using Authdemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;

namespace Authdemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            //builder.Services.AddSwaggerGen();
            //Configure Swagger to support JWT Authentication
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

                // Add JWT Authentication support in Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...\""
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });



            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<PasswordHasher<User>>();  // 
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, MailtrapEmailService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Add Authentication & Authorization
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("FinanceOnly", policy =>
                {
                    policy.RequireClaim("Department", "Finance");
                });
                // 
                options.AddPolicy("FinanceManager", policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("Department", "Finance");
                });
            });

            var app = builder.Build();
    

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Order matters: Authentication first, then Authorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}


// Nugets
//Microsoft.EntityFrameworkCore.SqlServer
//Microsoft.EntityFrameworkCore.Tools
//Microsoft.AspNetCore.Authentication.JwtBearer


// Migrations
// Add-Migration InitialCreate
// Update-Database
// Add-Migration AddDepartmentToUser
// Update-Database
// Add-Migration AddRefreshTokens
// Update-Database
// Add-Migration AddPasswordResetTokens
// Update-Database

// Integrate Mailtrap for email sending
// Install-Package MailKit

 //SHOULDER
 //Lateral Raise: 2.5, 1.25 - 1.25, 2.5 = 7.5
 //Rear Delt Fly: 2.5, 1.25 - 1.25, 2.5 = 7.5
 //Arnold Press:  2.5, 1.25 - 1.25, 2.5 = 7.5

 //TRICEPS
 //Overhead Extension: 1.25, 5.00 - 5.00, 1.25 = 12.5
 //Skull Crushers:     1.25, 5.00 - 5.00, 1.25 = 12.5
 //Chest Press:        1.25, 5.00 - 5.00, 1.25 = 12.5
 //Kick Backs:         1.25, 1.25 - 1.25, 1.25 = 5.0
