using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeGrabMonolith.Database.Contexts;

namespace ShoeGrabMonolith.Database.Management;

public static class MigrationsManager
{
    public static void ApplyMigrations(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetRequiredService<UserContext>())
            {
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database");
                    throw;
                }
            }
        }
    }
}
