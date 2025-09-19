using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace Todo.API.Configuration
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                // Configure API versioning for Swagger
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, configuration));
                }

                // Include XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Add security definitions
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Add custom operation filters
                options.OperationFilter<SwaggerExamplesFilter>();
                options.DocumentFilter<SwaggerVersionDocumentFilter>();
            });

            return services;
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, IConfiguration configuration)
        {
            var info = new OpenApiInfo
            {
                Title = "Todo API",
                Version = description.ApiVersion.ToString(),
                Description = GetApiVersionDescription(description.ApiVersion),
                Contact = new OpenApiContact
                {
                    Name = "Todo API Team",
                    Email = "support@todoapi.com",
                    Url = new Uri("https://github.com/todo-api")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                },
                TermsOfService = new Uri("https://todoapi.com/terms")
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private static string GetApiVersionDescription(ApiVersion version)
        {
            return version.ToString() switch
            {
                "1.0" => "Initial version of the Todo API with basic CRUD operations. Provides simple JSON responses for all todo operations.",
                "2.0" => "Enhanced version of the Todo API with improved responses including metadata, enhanced validation, and better error handling.",
                _ => $"Version {version} of the Todo API."
            };
        }
    }


    public class SwaggerExamplesFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Add examples for common operations
            if (operation.RequestBody?.Content?.ContainsKey("application/json") == true)
            {
                var requestContent = operation.RequestBody.Content["application/json"];

                if (context.MethodInfo.Name.Contains("Create") || context.MethodInfo.Name.Contains("Update"))
                {
                    requestContent.Example = OpenApiAnyFactory.CreateFromJson("""
                    {
                        "title": "Complete the project documentation",
                        "isCompleted": false
                    }
                    """);
                    // In class SwaggerExamplesFilter, method Apply, change the declaration of 'response' to be nullable.
                    OpenApiResponse? response = null;
                    if (operation.Responses.TryGetValue("200", out var resp200))
                        response = resp200;
                    else if (operation.Responses.TryGetValue("201", out var resp201))
                        response = resp201;

                    if (response?.Content?.ContainsKey("application/json") == true)
                    {
                        var responseContent = response.Content["application/json"];

                        if (context.MethodInfo.Name.Contains("GetTodos"))
                        {
                            responseContent.Example = OpenApiAnyFactory.CreateFromJson("""
                            [
                                {
                                    "id": 1,
                                    "title": "Build a REST API",
                                    "isCompleted": false
                                },
                                {
                                    "id": 2,
                                    "title": "Write unit tests",
                                    "isCompleted": true
                                }
                            ]
                            """);
                        }
                        else if (context.MethodInfo.Name.Contains("GetTodo"))
                        {
                            responseContent.Example = OpenApiAnyFactory.CreateFromJson("""
                            {
                                "id": 1,
                                "title": "Build a REST API",
                                "isCompleted": false
                            }
                            """);
                        }
                    }
                }       
            }

            // Add response examples
            if (operation.Responses.ContainsKey("200") || operation.Responses.ContainsKey("201"))
            {
                OpenApiResponse? response = null;
                if (operation.Responses.TryGetValue("200", out var resp200))
                    response = resp200;
                else if (operation.Responses.TryGetValue("201", out var resp201))
                    response = resp201;

                if (response?.Content?.ContainsKey("application/json") == true)
                {
                    var responseContent = response.Content["application/json"];

                    if (context.MethodInfo.Name.Contains("GetTodos"))
                    {
                        responseContent.Example = OpenApiAnyFactory.CreateFromJson("""
                        [
                            {
                                "id": 1,
                                "title": "Build a REST API",
                                "isCompleted": false
                            },
                            {
                                "id": 2,
                                "title": "Write unit tests",
                                "isCompleted": true
                            }
                        ]
                        """);
                    }
                    else if (context.MethodInfo.Name.Contains("GetTodo"))
                    {
                        responseContent.Example = OpenApiAnyFactory.CreateFromJson("""
                        {
                            "id": 1,
                            "title": "Build a REST API",
                            "isCompleted": false
                        }
                        """);
                    }
                }
            }
        }
    }

    public class SwaggerVersionDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Add version-specific information
            swaggerDoc.Info.Version = context.DocumentName;
            
            // Add server information
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5279",
                    Description = "Development Server (HTTP)"
                },
                new OpenApiServer
                {
                    Url = "https://localhost:5279",
                    Description = "Development Server (HTTPS)"
                }
            };

            // Add global tags
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag
                {
                    Name = "Todos",
                    Description = "Operations related to todo items",
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Description = "Find out more about todos",
                        Url = new Uri("https://docs.todoapi.com/todos")
                    }
                }
            };
        }
    }
}
