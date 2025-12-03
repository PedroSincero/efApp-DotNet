using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EFEnergiaAPI.Data;
using EFEnergiaAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=EFEnergiaDB;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationThatMustBeAtLeast32Characters";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EFEnergiaAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EFEnergiaAPI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Apply any pending migrations
        Console.WriteLine("Applying database migrations...");
        context.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        
        // Wait a bit and retry once (in case SQL Server is still starting)
        Console.WriteLine("Retrying migration in 5 seconds...");
        System.Threading.Thread.Sleep(5000);
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully on retry!");
        }
        catch (Exception retryEx)
        {
            logger.LogError(retryEx, "Failed to apply migrations after retry.");
            throw;
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
