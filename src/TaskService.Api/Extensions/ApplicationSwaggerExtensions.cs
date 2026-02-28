using Microsoft.OpenApi; 

namespace TaskService.Api.Extensions
{
    public static class ApplicationSwaggerExtensions
    {
        public static void RegisterSwaggerExtensions(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Task Service API", Version = "v1" });
                c.UseInlineDefinitionsForEnums();
                c.EnableAnnotations();

                // Add "Authorize" button to Swagger UI — paste your JWT to test protected endpoints
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token. Example: eyJhbGci..."
                }); 
            });
        }
    }
}
