using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TaskService.Infrastructure.Persistence.Data;

namespace TaskService.Api.Extensions
{
    public static class ApplicationDbContextExtensions
    {
        public static IServiceCollection RegisterDbContextExtensions(this IServiceCollection services, IConfiguration configuration) {
            return services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    }));

        }
    }
}
