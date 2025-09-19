using Asp.Versioning;

namespace Todo.API.Configuration
{
    public static class ApiVersioningConfiguration
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var apiVersion = configuration.GetValue<string>("ApiVersioning:DefaultVersion") ?? "1.0";
            var versionParts = apiVersion.Split('.');
            var majorVersion = int.Parse(versionParts[0]);
            var minorVersion = versionParts.Length > 1 ? int.Parse(versionParts[1]) : 0;

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(majorVersion, minorVersion);
                options.AssumeDefaultVersionWhenUnspecified = configuration.GetValue<bool>("ApiVersioning:AssumeDefaultVersionWhenUnspecified", true);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader(configuration.GetValue<string>("ApiVersioning:QueryStringParameterName") ?? "version"),
                    new HeaderApiVersionReader(configuration.GetValue<string>("ApiVersioning:HeaderName") ?? "X-Version"),
                    new MediaTypeApiVersionReader(configuration.GetValue<string>("ApiVersioning:MediaTypeParameterName") ?? "ver")
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = configuration.GetValue<string>("ApiVersioning:GroupNameFormat") ?? "'v'VVV";
                options.SubstituteApiVersionInUrl = configuration.GetValue<bool>("ApiVersioning:SubstituteApiVersionInUrl", true);
            });

            return services;
        }
    }
}
