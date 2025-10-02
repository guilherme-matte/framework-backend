using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace framework_backend.Filter
{
    public class NewsLetterFormOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasNewsLetterForm = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType.Name == "NewsLetterDTO");

            if (!hasNewsLetterForm) return;

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
                                Description = "JSON serializado do NewsLetterModel",
                                Example = new OpenApiString(@"{ 
                                    ""Title"": ""Título da newsletter"",
                                    ""Date"": ""2025-10-02"",
                                    ""Excerpt"": ""Resumo da newsletter"",
                                    ""Category"": ""Categoria"",
                                    ""Tags"": [""tag1"", ""tag2""],
                                    ""BulletPoint"": [""ponto 1"", ""ponto 2""]
                                }")
                            },
                            ["Files"] = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                },
                                Description = "Arquivos de imagem da newsletter"
                            },
                            ["FirstFlags"] = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "boolean"
                                },
                                Description = "Flags booleanas para cada imagem"
                            }
                        },
                        Required = new HashSet<string> { "Data" }
                    },
                    Encoding = new Dictionary<string, OpenApiEncoding>
                    {
                        ["Data"] = new OpenApiEncoding { ContentType="text/plain",
                            Style = ParameterStyle.Form },
                        ["Files"] = new OpenApiEncoding { Style = ParameterStyle.Form },
                        ["FirstFlags"] = new OpenApiEncoding { Style = ParameterStyle.Form }
                    }
                }
            }
            };
        }
    }
}


