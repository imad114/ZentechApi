using Zentech.Models;
using Zentech.Repositories;
using Zentech.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using ZentechAPI.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZentechAPI.Services;
using ZentechAPI.Models;

var builder = WebApplication.CreateBuilder(args);
// Configurer les logs (niveau 'Information' ou 'Debug' pour voir les d�tails)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();  // Utilisez le d�bogage pour afficher dans la console de d�bogage

//  services
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
    // Configure Swagger to handle file uploads via IFormFile
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary",
        Description = "Upload file"
    });
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "Zentech.xml");
    options.IncludeXmlComments(xmlFile);

    // Security configuration for Swagger to accept JWT

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer in front of the token"
    });

    // Apply security to all operations

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

// Add JWT authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; //  (true in production)
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Issuer to be defined in appsettings.json

            ValidAudience = builder.Configuration["Jwt:Audience"], // Audience to be defined in appsettings.json
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  // Secret key to be defined in appsettings.json
        };
    });

//builder.Services.AddScoped<DatabaseContext>();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<DatabaseContext>(provider =>
    new DatabaseContext(connectionString));//builder.Services.AddScoped<DatabaseContext>();

// Register services and repositories


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
//Solutions 
builder.Services.AddScoped<SolutionRepository>();
builder.Services.AddScoped<SolutionService>();
//CompanyInformation 
builder.Services.AddScoped<CompanyInformationRepository>();
builder.Services.AddScoped<CompanyInformationService>();
//Page
builder.Services.AddScoped<PageRepository>();
builder.Services.AddScoped<PageService>();

builder.Services.AddScoped<SlidesRepository>();
builder.Services.AddScoped<SlidesService>();

builder.Services.AddScoped<TechincalDocRepository>();
builder.Services.AddScoped<TechnicalDocService>();


builder.Services.AddTransient<OtherCategoriesRepository>();

builder.Services.AddTransient<OtherCategoriesRepository>();

builder.Services.AddScoped<ProductModelRepository>();
builder.Services.AddScoped<ProductModelService>();

builder.Services.AddTransient<EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));



//......................................................................
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 10 * 1024 * 1024; // Par exemple, pour autoriser les fichiers de 10 Mo
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5033);  // Modifiez ici le port
});

//..................................................


var app = builder.Build();

app.UseCors("AllowAll");




// Configure the HTTP pipeline

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            try
            {
                // Swagger generation logic
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Swagger generation: {ex.Message}");
            }
        });
    });

    app.UseSwaggerUI(c =>
    {
        // Swagger UI will be accessible at /swagger

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zentech API V1");
        c.RoutePrefix = string.Empty; // To access Swagger UI via    http://localhost:5033/

    });
}

// Serve static files from the 'wwwroot/uploads' folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/uploads"
});


app.UseAuthentication();
// For HTTP to HTTPS redirection
//app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


