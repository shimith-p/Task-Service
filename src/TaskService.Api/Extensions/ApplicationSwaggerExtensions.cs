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
            });
        }
    }
}
