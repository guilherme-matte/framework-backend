using framework_backend.Data;
using framework_backend.Filter;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("A variável de ambiente DEFAULT_CONNECTION não está definida.");
}
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<NewsLetterFormOperationFilter>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Framework API", Version = "v1" });
    c.OperationFilter<ArchitectFormOperationFilter>();
    c.OperationFilter<NewsLetterFormOperationFilter>();



});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://framework-frontend-pearl.vercel.app", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddScoped<ImageService>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Framework API V1");
});
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "img");

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "img")),
    RequestPath = "/img"
});
app.Use(async (context, next) =>
{
    Console.WriteLine($"Origin: {context.Request.Headers["Origin"]}");
    await next();
});



app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
