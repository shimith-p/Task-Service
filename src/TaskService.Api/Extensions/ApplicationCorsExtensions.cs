namespace TaskService.Api.Extensions
{
    public static class ApplicationCorsExtensions
    {
        public static void RegisterCorsExtensions(this IServiceCollection services)
        {
            services.AddCors(opts =>
                opts.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }
    }
}  
