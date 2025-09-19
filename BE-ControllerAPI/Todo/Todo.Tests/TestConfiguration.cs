using Microsoft.Extensions.DependencyInjection;
using Todo.Business.Features.Todos.Queries;
using Todo.Contracts.Interfaces;
using Todo.Data.Repositories;

namespace Todo.Tests
{
    public static class TestConfiguration
    {
        public static IServiceCollection ConfigureTestServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosQuery).Assembly));
            services.AddSingleton<ITodoRepository, TodoRepository>();
            return services;
        }

        public static IServiceProvider CreateTestServiceProvider()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            return services.BuildServiceProvider();
        }
    }
}
