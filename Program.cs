using framework_backend.Data;
using framework_backend.Models;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Framework API", Version = "v1" });

    // Exemplo de body para Architect
    c.MapType<ArchitectModel>(() => new OpenApiSchema
    {
        Type = "object",
        Example = new OpenApiObject
        {
            ["name"] = new OpenApiString("nome"),
            ["subtitle"] = new OpenApiString("subtitulo"),
            ["birthDate"] = new OpenApiString("0000-00-00"),
            ["nationality"] = new OpenApiString("nacionalidade"),
            ["biography"] = new OpenApiString("biografia"),
            ["picture"] = new OpenApiString("url da imagem"),
            ["verified"] = new OpenApiBoolean(true),
            ["trending"] = new OpenApiBoolean(true),
            ["training"] = new OpenApiObject
            {
                ["name"] = new OpenApiString("nome da instituição"),
                ["year"] = new OpenApiInteger(0000)
            },
            ["socialMedia"] = new OpenApiObject
            {
                ["linkedin"] = new OpenApiString("url"),
                ["instagram"] = new OpenApiString("url"),
                ["portfolio"] = new OpenApiString("url")
            },
            ["stats"] = new OpenApiObject
            {
                ["totalProjects"] = new OpenApiInteger(0),
                ["esgProjects"] = new OpenApiInteger(0),
                ["views"] = new OpenApiInteger(0000),
                ["likes"] = new OpenApiInteger(000),
                ["followers"] = new OpenApiInteger(000)
            },
            ["location"] = new OpenApiObject
            {
                ["city"] = new OpenApiString("cidade"),
                ["state"] = new OpenApiString("estado"),
                ["country"] = new OpenApiString("pais")
            },
            ["speciality"] = new OpenApiArray
            {
                new OpenApiString("nome da especialidade 1"),
                new OpenApiString("nome da especialidade 2")
            }
        }
    });
});

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


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
//teste de deploy
