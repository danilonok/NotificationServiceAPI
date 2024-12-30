using NotificationServiceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationPlanServiceAPI.Helpers
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            try
            {
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex}");
            }
        }

    }
}
