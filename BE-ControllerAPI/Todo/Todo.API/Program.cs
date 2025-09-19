using Asp.Versioning.ApiExplorer;
using Serilog;
using Todo.API.Configuration;
using Todo.Contracts.Interfaces;
using Todo.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Todo.API")
    .WriteTo.Console()
    .WriteTo.File("logs/todo-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add Serilog to the logging pipeline
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add API Versioning (configurable)
builder.Services.AddApiVersioningConfiguration(builder.Configuration);

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Todo.Business.Features.Todos.Queries.GetTodosQuery).Assembly));

// Add repositories
builder.Services.AddSingleton<ITodoRepository, TodoRepository>();

// Add Swagger/OpenAPI with versioning support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration(builder.Configuration);

// Add CORS (intentional BUG: Allow CORS for all origins, headers, and methods. In production, this should be restricted.)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true); // Allow all origins for development
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(x => x.ApiVersion))
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Todo API {description.GroupName.ToUpperInvariant()}");
        }
        
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Todo API Documentation";
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
