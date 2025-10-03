using framework_backend.Data;
using framework_backend.Filter;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("A variável de ambiente DEFAULT_CONNECTION não está definida.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<NewsLetterFormOperationFilter>();
builder.Services.AddScoped<ImageService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Framework API", Version = "v1" });
    c.OperationFilter<ArchitectFormOperationFilter>();
    c.OperationFilter<NewsLetterFormOperationFilter>();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://framework-frontend-pearl.vercel.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// If running behind proxy (Cloudflare, Render) to get real IP
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // optionally add known proxies/ networks:
    // options.KnownProxies.Add(IPAddress.Parse("..."));
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Exception handler middleware - put it as early as possible
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Unhandled exception: {ex}");
        if (!context.Response.HasStarted)
        {
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var payload = System.Text.Json.JsonSerializer.Serialize(new { message = "Erro interno no servidor" });
            await context.Response.WriteAsync(payload);
        }
        else
        {
            Console.Error.WriteLine("Response already started, cannot set status code.");
        }
    }
});

// Forwarded headers (if behind proxy)
app.UseForwardedHeaders();

// Request logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"{DateTime.UtcNow:o} Remote:{context.Connection.RemoteIpAddress} Method:{context.Request.Method} Path:{context.Request.Path} Origin:{context.Request.Headers["Origin"]} Referer:{context.Request.Headers["Referer"]} UA:{context.Request.Headers["User-Agent"]}");
    await next();
});

// Serve a minimal root/health endpoint before other handlers
app.MapMethods("/", new[] { "HEAD", "GET" }, () => Results.Ok(new { status = "ok", now = DateTime.UtcNow }));

// Static files for images
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "img");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/img"
});

// HTTPS redirect
app.UseHttpsRedirection();

// CORS must be before auth/controllers
app.UseCors("AllowFrontend");

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Swagger (optionally restrict to Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Framework API V1");
    });
}

app.MapControllers();

app.Run();
