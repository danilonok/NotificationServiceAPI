

using NotificationServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace NotificationServiceAPI.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
        }
        public DbSet<UserNotificationSettings> NotificationSettings { get; set; }

        public DbSet<NotificationTask> NotificationTasks { get; set; }

    }
}
