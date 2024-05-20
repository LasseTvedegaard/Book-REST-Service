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

namespace Book_REST_Service {
    public class Program {
        public static void Main(string[] args) {
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

            // Database connection service
            builder.Services.AddTransient(provider => new LibraryConnection(connectionString));

            // Register LibraryConnection
            builder.Services.AddTransient<LibraryConnection>(provider => new LibraryConnection(connectionString));

            // Register the generic connection with LibraryConnection as the type parameter
            builder.Services.AddTransient(typeof(IGenericConnection<LibraryConnection>), typeof(GenericConnection<LibraryConnection>));

            // MVC Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var provider = builder.Services.BuildServiceProvider();
            //var configuration = provider.GetService<IConfiguration>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.WithOrigins(configuration.GetValue<string>("frontend_url"))
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Include this only if your application needs to handle credentials like cookies or authentication headers
                });
            });
            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline

            if (app.Environment.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            } else {
                app.UseHttpsRedirection();
            }

            app.UseCors("DefaultPolicy");

            app.UseSerilogRequestLogging(); // Enable Serilog request logging

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
