using Zentech.Models;
using Zentech.Repositories;
using Zentech.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

//  des services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Zentech API",
        Version = "v1",
        Description = "API documentation for Zentech application",
    });
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "Zentech.xml");
    options.IncludeXmlComments(xmlFile);

    // Configuration de la sécurité pour Swagger pour accepter JWT 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer in front of the token"
    });

    // Appliquer la sécurité à toutes les opérations
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
});

// Ajouter l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; //  (true en production)
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Issuer à définir dans appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"],  // Audience à définir dans appsettings.json
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  // Secret key à définir dans appsettings.json
        };
    });

// Enregistrer DatabaseContext comme service
builder.Services.AddScoped<DatabaseContext>();

// Enregistrer les services et repositories

// product
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductRepository>();
// contact
builder.Services.AddScoped<ContactRepository>();
builder.Services.AddScoped<ContactService>();
// user
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
//news 
builder.Services.AddScoped<NewsRepository>();
builder.Services.AddScoped<NewsService>();
//Category 
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CategoryService>();






var app = builder.Build();




// Configure le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Swagger UI sera accessible sur /swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zentech API V1");
        c.RoutePrefix = string.Empty; // Pour accéder à Swagger UI via http://localhost:5033/
    });
}


app.UseAuthentication();  
// pour redirection http ver https
//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
