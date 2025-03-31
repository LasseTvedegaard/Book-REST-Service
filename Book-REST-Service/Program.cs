using Book_REST_Service.Controllers;
using BusinessLogic;
using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Helpers;
using Model;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Context;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Book_REST_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env file
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog using the configuration from the builder's context
            builder.Host.UseSerilog((context, config) => {
                config.ReadFrom.Configuration(context.Configuration);
            });

            // Retrieve the configuration directly from the builder's Configuration property
            var configuration = builder.Configuration;

            // Use the ConnectionStringHelper to get the connection string
            var connectionString = ConnectionStringHelper.GetConnectionString(configuration);

            // Add services to the container

            // Employee services
            builder.Services.AddTransient<ICRUD<Employee>, EmployeeControl>();
            builder.Services.AddTransient<ICRUDAccess<Employee>, EmployeeAccess>();

            // Genre services
            builder.Services.AddTransient<ICRUD<Genre>, GenreControl>();
            builder.Services.AddTransient<ICRUDAccess<Genre>, GenreAccess>();

            // Location services
            builder.Services.AddTransient<ICRUD<Location>, LocationControl>();
            builder.Services.AddTransient<ICRUDAccess<Location>, LocationAccess>();

            // Book services
            builder.Services.AddTransient<IBookControl, BookControl>();
            builder.Services.AddTransient<IBookAccess, BookAccess>();

            // User services
            builder.Services.AddTransient<IUserControl, UserControl>();
            builder.Services.AddTransient<IUserAccess, UserAccess>();

            // Log services
            builder.Services.AddTransient<ILogControl, LogControl>();
            builder.Services.AddTransient<ILogAccess, LogAccess>();

            // ReadingNote services
            builder.Services.AddScoped<IReadingNoteControl, ReadingNoteControl>();
            builder.Services.AddScoped<IReadingNoteAccess, ReadingNoteAccess>();


            // Database connection service
            builder.Services.AddTransient(provider => new LibraryConnection(connectionString));

            // Register LibraryConnection
            builder.Services.AddTransient<LibraryConnection>(provider => new LibraryConnection(connectionString));

            // Register the generic connection with LibraryConnection as the type parameter
            builder.Services.AddTransient(typeof(IGenericConnection<LibraryConnection>), typeof(GenericConnection<LibraryConnection>));

            // MVC Controllers with case-insensitive JSON binding
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // CORS
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAllOrigins",
                    policy => {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            // Swagger
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

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline
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
