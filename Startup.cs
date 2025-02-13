using Microsoft.Extensions.DependencyInjection;
using Zentech.Repositories;
using Zentech.Services;

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
        services.AddSwaggerGen();
        services.AddScoped<ProductService>();
        services.AddScoped<ProductRepository>();

        services.AddScoped<ContactRepository>();
        services.AddScoped<ContactService>();

        services.AddScoped<UserRepository>();
        services.AddScoped<UserService>();

        services.AddScoped<SlidesRepository>();
        services.AddScoped<SlidesService>();


    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }


      //  app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

   
}
