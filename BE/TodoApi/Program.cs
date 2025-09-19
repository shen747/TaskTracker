using MediatR;
using TodoApi.Data;
using TodoApi.Features.Todos.Commands;
using static TodoApi.Features.Todos.Commands.AddTodo;
using static TodoApi.Features.Todos.Queries.GetTodos;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<InMemoryDataStore>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**  Intentional BUG :  Allow CORS for all origins, headers, and methods. In production, this should be restricted.*/
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod(); 
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

var api = app.MapGroup("/api/todos");

api.MapGet("/", async (IMediator mediator) =>
{
    var todos = await mediator.Send(new Query());
    return Results.Ok(todos);
});

api.MapPost("/", async (IMediator mediator, Command command) =>
{
    if(string.IsNullOrWhiteSpace(command.Title))
    {
        return Results.BadRequest(new { Error = "Title is required." });
    }
    var newTodo = await mediator.Send(command);
    return Results.Created($"/api/todos/{newTodo.Id}", newTodo);
});

api.MapPut("/{id:int}", async (IMediator mediator, int id, UpdateTodo.Command updateCommand) =>
{
    if (string.IsNullOrWhiteSpace(updateCommand.Title))
    {
        return Results.BadRequest(new { Error = "Title is required." });
    }

    updateCommand.Id = id; // Ensure the ID from the route is used
    var updatedTodo = await mediator.Send(updateCommand);
    
    if (updatedTodo == null)
    {
        return Results.NotFound(new { Error = "Todo item not found." });
    }
    
    return Results.Ok(updatedTodo);
});

api.MapDelete("/{id:int}", async (IMediator mediator,int id) =>
{
   var result = await mediator.Send(new DeleteTodo.Command { Id = id });
    return result ? Results.NoContent() : Results.NotFound(new { Error = "Todo item not found." });
});

app.Run();

