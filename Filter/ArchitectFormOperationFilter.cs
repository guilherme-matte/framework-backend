using framework_backend.DTOs;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

public class ArchitectFormOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        try
        {
            var hasArchitectForm = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(ArchitectUpdateForm));

            if (!hasArchitectForm) return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["Data"] = new OpenApiSchema
                            {
                                Type = "string",
                                Description = "JSON serializado do ArchitectDTO",
                                Example = new OpenApiString(@"{ 
                                    ""Name"": ""NOME DO ARQUITETO"",
                                    ""Nationality"": ""NACIONALIDADE"",
                                    ""Subtitle"": ""SUBTITULO"",
                                    ""BirthDate"": ""1900-01-01"",
                                    ""Biography"": ""BIOGRAFIA"",
                                    ""Speciality"": [""ESPECIALIDADE 1"", ""ESPECIALIDADE 2"",""ESPECIALIDADE 3""],
                                    ""Training"": {
                                        ""Name"": ""INSTUTUIÇÃO"",
                                        ""Year"": 1900
                                    },
                                    ""SocialMedia"": {
                                        ""Instagram"": ""INSTAGRAM"",
                                        ""Linkedin"": ""LINKEDIN"",
                                        ""Portfolio"": ""PORTIFOLIO""
                                    },
                                    ""Location"": {
                                        ""City"": ""CIDADE"",
                                        ""State"": ""ESTADO"",
                                        ""Country"": ""PAIS""
                                        
                                    }
                                }")


                            },
                            ["File"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary",
                                Description = "Imagem de perfil do arquiteto"
                            }
                        },
                        Required = new HashSet<string> { "Data", "File" }
                    }
                }
            }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no Swagger OperationFilter: {ex.Message}");
        }
    }

}
