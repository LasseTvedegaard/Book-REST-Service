using Book_REST_Service.Controllers;
using BusinessLogic;
using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Context;
using DataAccess.Helpers;
using DataAccess.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Model;
using Serilog;
using System.Text;

namespace Book_REST_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // .env KUN lokalt
            if (builder.Environment.IsDevelopment())
            {
                Env.Load();
            }

            // Serilog
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            var configuration = builder.Configuration;

            // DB connection
            var connectionString = ConnectionStringHelper.GetConnectionString(configuration);

            // -----------------------------
            // Dependency Injection
            // -----------------------------

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

            // ✅ KORREKT: DB connection skal være Scoped (vigtigt for Dapper)
            builder.Services.AddScoped(provider => new LibraryConnection(connectionString));
            builder.Services.AddScoped(
                typeof(IGenericConnection<LibraryConnection>),
                typeof(GenericConnection<LibraryConnection>)
            );

            // 🔑 Refresh tokens (Dapper)
            builder.Services.AddScoped<RefreshTokenAccess>();

            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // -----------------------------
            // CORS (KRITISK for refresh tokens)
            // -----------------------------

            builder.Services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", p =>
                {
                    p.WithOrigins(
                        "http://localhost:3000",
                        "https://dit-frontend-domæne.azurewebsites.net"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // -----------------------------
            // JWT Authentication
            // -----------------------------

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            {
                throw new Exception("JWT Key mangler eller er for kort (min. 32 tegn).");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        )
                    };
                });

            // -----------------------------
            // Swagger
            // -----------------------------

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BookBuddy API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token (without 'Bearer ')"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // -----------------------------
            // Middleware
            // -----------------------------

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
