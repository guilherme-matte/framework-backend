using framework_backend.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasNewsLetterDTO = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(NewsLetterDTO));

        if (!hasNewsLetterDTO) return;

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
                                Description = "JSON serializado do NewsLetterModel"
                            },
                            ["Files[0].File"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary",
                                Description = "Imagem da notícia"
                            },
                            ["Files[0].First"] = new OpenApiSchema
                            {
                                Type = "boolean",
                                Description = "Flag que indica se é a imagem principal"
                            },
                        },
                        Required = new HashSet<string> { "Data", "Files[0].File" }
                    }
                }
            }
        };
    }
}
