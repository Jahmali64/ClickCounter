using System.Text;
using ClickCounter.Application;
using ClickCounter.Infrastructure;
using ClickCounter.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try {
    Log.Information("Starting Click Counter API");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Configuration.SetBasePath(Directory.GetParent(builder.Environment.ContentRootPath)!.FullName)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    JwtSettings jwtSettings = new();
    builder.Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped(typeof(CancellationToken),
        serviceProvider => {
            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        });
    builder.Services.AddCors(options => {
        options.AddPolicy(name: "Development", configurePolicy: policy => policy.AllowAnyOrigin());
        options.AddPolicy(name: "Production", configurePolicy: policy => policy.WithOrigins("https://clickcounter.azurewebsites.net"));
    });

    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true
        };
    });
    builder.Services.AddAuthorization();
    
    WebApplication app = builder.Build();
    if (app.Environment.IsDevelopment()) {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseCors("Development");
    } else {
        app.UseCors("Production");
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Unhandled exception");
} finally {
    Log.CloseAndFlush();
}