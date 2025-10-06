using framework_backend.DTOs;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace framework_backend.Filter
{
    public class ProjectFormOperationFilter:IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            try
            {
                var hasProjectForm = context.MethodInfo.GetParameters()
                    .Any(p => p.ParameterType == typeof(CreateProjectDTO));

                if (!hasProjectForm) return;

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
                            ["data"] = new OpenApiSchema
                            {
                                Type = "string",
                                Description = "JSON do ProjectDTO",
                                Example = new OpenApiString(@"{ 
                                    ""title"": ""TITULO"",
                                    ""shortDescription"": ""DESCRIÇÃO CURTA"",
                                    ""longDescription"": ""DESCRIÇÃO LONGA"",
                                    ""area"": ""ÁREA"",
                                    ""location"": {
                                        ""city"": ""CIDADE"",
                                        ""state"": ""ESTADO"",
                                        ""country"": ""PAÍS"",
                                        ""address"": ""ENDEREÇO"",
                                        ""coordinates"": {
                                            ""latitude"": -99.1234,
                                            ""longitude"": 99.1234
                                        }
                                    },
                                    
                                    ""esg"": true,
                                    ""architects"": [
                                        {
                                            ""architectId"": 1,
                                            ""role"": ""PAPEL DO ARQUITETO""
                                        },
                                        {
                                            ""architectId"": 2,
                                            ""role"": ""PAPEL DO ARQUITETO""
                                        }
                                    ],
                                    ""startDate"": ""1901-01-01"",
                                    ""endDate"": ""1901-01-01"",
                                    ""ongoing"": false
                                }")


                            },
                            ["files"] = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                },
                                Description = "Lista de imagens do projeto"
                            }
                        },
                        Required = new HashSet<string> { "data", "files" }
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
}
