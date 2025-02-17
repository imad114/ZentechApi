using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Zentech.Repositories;
using Zentech.Services;
using ZentechAPI.Repositories;
using ZentechAPI.Services;
using ZentechAPI.Models;
using System;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Configuration Swagger avec JWT
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Zentech API",
                Version = "v1",
                Description = "API documentation for Zentech application",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by your JWT token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                }
            });

            var xmlFile = Path.Combine(AppContext.BaseDirectory, "Zentech.xml");
            if (File.Exists(xmlFile))
            {
                options.IncludeXmlComments(xmlFile);
            }
        });

        // Configuration de l'authentification JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                };
            });

        // Connexion à la base de données
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("DefaultConnection");
        services.AddScoped<DatabaseContext>(provider => new DatabaseContext(connectionString));

        // Ajout des services et repositories
        services.AddScoped<ProductService>();
        services.AddScoped<ProductRepository>();
        services.AddScoped<ContactRepository>();
        services.AddScoped<ContactService>();
        services.AddScoped<UserRepository>();
        services.AddScoped<UserService>();
        services.AddScoped<SlidesRepository>();
        services.AddScoped<SlidesService>();
        services.AddScoped<NewsRepository>();
        services.AddScoped<NewsService>();
        services.AddScoped<CategoryRepository>();
        services.AddScoped<CategoryService>();
        services.AddScoped<SolutionRepository>();
        services.AddScoped<SolutionService>();
        services.AddScoped<CompanyInformationRepository>();
        services.AddScoped<CompanyInformationService>();
        services.AddScoped<PageRepository>();
        services.AddScoped<PageService>();
        services.AddTransient<EmailService>();
        services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

        // Configuration CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Configuration pour limiter la taille des fichiers uploadés
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = 10 * 1024 * 1024; // 10 Mo
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Gestion des erreurs
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zentech API V1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Redirection HTTPS uniquement en production
        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // Configuration des fichiers statiques
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
            RequestPath = "/uploads"
        });

        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}
