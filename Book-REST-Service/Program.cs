using Book_REST_Service.Controllers;
using BusinessLogic;
using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Context;
using DataAccess.Helpers;
using DataAccess.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model;
using Serilog;
using System.Text;

namespace Book_REST_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Sørg for at log-mappen eksisterer, før Serilog starter
            Directory.CreateDirectory(@"C:\BookBuddy\backend\Logs");

            // Load environment variables from .env
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            builder.Host.UseSerilog((context, config) => {
                config.ReadFrom.Configuration(context.Configuration);
            });

            var configuration = builder.Configuration;

            // Get PostgreSQL connection string via helper
            var connectionString = ConnectionStringHelper.GetConnectionString(configuration);

            // Register services (business logic + data access)
            builder.Services.AddTransient<ICRUD<Employee>, EmployeeControl>();
            builder.Services.AddTransient<ICRUDAccess<Employee>, EmployeeAccess>();

            builder.Services.AddTransient<ICRUD<Genre>, GenreControl>();
            builder.Services.AddTransient<ICRUDAccess<Genre>, GenreAccess>();

            builder.Services.AddTransient<ICRUD<Location>, LocationControl>();
            builder.Services.AddTransient<ICRUDAccess<Location>, LocationAccess>();

            builder.Services.AddTransient<IBookControl, BookControl>();
            builder.Services.AddTransient<IBookAccess, BookAccess>();

            builder.Services.AddTransient<IUserControl, UserControl>();
            builder.Services.AddTransient<IUserAccess, UserAccess>();

            builder.Services.AddScoped<IReadingNoteControl, ReadingNoteControl>();
            builder.Services.AddScoped<IReadingNoteAccess, ReadingNoteAccess>();

            builder.Services.AddTransient<ILogControl, LogControl>();
            builder.Services.AddTransient<ILogAccess, LogAccess>();

            // Register DB connection classes
            builder.Services.AddTransient(provider => new LibraryConnection(connectionString));
            builder.Services.AddTransient<LibraryConnection>(provider => new LibraryConnection(connectionString));
            builder.Services.AddTransient(typeof(IGenericConnection<LibraryConnection>), typeof(GenericConnection<LibraryConnection>));

            // Add Controllers + Case-insensitive JSON
            builder.Services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            // CORS (for frontend adgang)
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAllOrigins", policy => {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Swagger (til API-test)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            // Build app
            var app = builder.Build();

            // Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
