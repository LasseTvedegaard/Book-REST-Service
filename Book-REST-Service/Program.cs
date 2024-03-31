using Book_REST_Service.Controllers;
using BusinessLogic;
using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Interfaces;
using Model;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;

namespace Book_REST_Service {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container

            // Employee
            builder.Services.AddTransient<ICRUD<Employee>, EmployeeControl>();
            builder.Services.AddTransient<ICRUDAccess<Employee>, EmployeeAccess>();

            // Genre
            builder.Services.AddTransient<ICRUD<Genre>, GenreControl>();
            builder.Services.AddTransient<ICRUDAccess<Genre>, GenreAccess>();

            // Location
            builder.Services.AddTransient<ICRUD<Location>, LocationControl>();
            builder.Services.AddTransient<ICRUDAccess<Location>, LocationAccess>();

            // Book
            builder.Services.AddTransient<IBookControl, BookControl>();
            builder.Services.AddTransient<IBookAccess, BookAccess>();

            // User
            builder.Services.AddTransient<IUserControl, UserControl>();
            builder.Services.AddTransient<IUserAccess, UserAccess>();

            // Log
            builder.Services.AddTransient<ILogControl, LogControl>();
            builder.Services.AddTransient<ILogAccess, LogAccess>();

            // SeriLog
            var _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.File(
                   new CompactJsonFormatter(),
                   "Log/APILog-.log",
                   rollingInterval: RollingInterval.Day
                ).WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} -//- {Exception}")
                .CreateLogger();

            builder.Logging.AddSerilog(_logger);


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}