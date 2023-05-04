using JATrackrAPI.Models;
using JATrackrAPI.Services;
using JATrackrAPI.Utility;

public class Program
{
    public static async Task Main(string[] args)
    {   
        // Set up env vars; <b>Assumption</b>: `.env` file located in current working directory (root of project),
        // Filename will be "cwd + .env"; passed as argument to be loaded using the Load() method in the DotEnvLoader helper class
        // If .env file is not found, no secrets values will be loaded into the environment.
        var projectRoot = Directory.GetCurrentDirectory(); 
        var dotenvFile = Path.Combine(projectRoot, ".env");
        DotEnvLoader.Load(dotenvFile);

        // Define 'WebApplicationBuilder' for adding configuration, services, logging, etc. EXCEPT middleware pipeline
        // https://andrewlock.net/exploring-dotnet-6-part-2-comparing-webapplicationbuilder-to-the-generic-host/
        // Autogenerated by creating `new` web app through .NET CLI (or through Visual Studio template)
        var builder = WebApplication.CreateBuilder(args);

        // Configuration providers
        builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
        //.EnableSubstitutions("${","}");
        
        var services = builder.Services;

        // Configure logging (TODO)

        // Builder configuration, register services support via respective Dependency Injection (DI) containers.

        // Controller support
        services.AddControllers();

        // Swagger/OpenAPI
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configure DatabaseSettings options
        // Commented out due to issues with reading placeholder values from appsettings files ("key":"${value_from_env}")
        // builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseConfig"));
        
        // Register singleton instances with DI container ( 1 instance for app lifetime; all requests resolved through this instance)
       
        services.AddSingleton(DatabaseSettingsConfig());
        services.AddSingleton<UserService>();
        services.AddSingleton<JobDataService>();

        // Create new instance of 'WebApplication' using builder config
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsProduction())
        {
            // If production env, force ALL requests to be sent over HTTPS by adding HSTS middleware to request pipeline
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Middleware... order matters...

        //app.UseAuthorization();
        // app.MapControllers();

        // Redirect HTTP requests --> HTTPS
        app.UseHttpsRedirection();
        // Add support for static files like .js files, images, etc.
        app.UseStaticFiles();
        // Add support for routing requests to controller
        app.UseRouting();

        // Define custom routes for controllers by mapping requests to relevant controller action methods based on URL path
        // https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-7.0
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}"
        );

        // If requests do not match to any defined controller or action or static file, map to index.html
        app.MapFallbackToFile("index.html");

        // Run application and listen for incoming requests
        app.Run();
    }

    // Read database configuration settings from environment variables and create new instance using those values 
    private static DatabaseSettings DatabaseSettingsConfig()
    {
        return new DatabaseSettings
        {
            DBConnectionString = Environment.GetEnvironmentVariable("MONGODB_CS"),
            DBName = Environment.GetEnvironmentVariable("MONGODB_DB_NAME"),
            UsersCollectionName = Environment.GetEnvironmentVariable("MONGODB_USER_COLLECTION"),
            JobDataCollectionName = Environment.GetEnvironmentVariable("MONGODB_JOBDATA_COLLECTION")
        };
    }
}