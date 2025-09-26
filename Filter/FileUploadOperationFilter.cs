using framework_backend.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFormFile = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(ArchitectUpdateForm));

        if (!hasFormFile) return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            ["data"] = new OpenApiSchema { Type = "string", Description = "JSON do objeto" },
                            ["Image"] = new OpenApiSchema { Type = "string", Format = "binary" }
                        },
                        Required = new HashSet<string> { "data" }
                    }
                }
            }
        };
    }
}
