using NotificationServiceAPI.Data;
using NotificationServiceAPI.Models;

namespace NotificationServiceAPI.Helpers
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly AppDBContext _context;


        public NotificationService(IEmailService emailService, AppDBContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        public async Task ProcessTaskAsync(TaskMessage task)
        {
            
            var userSettings = GetUserSettings(task.UserID);

            if (userSettings != null && userSettings.IsNotificationEnabled)
            {
                var notifyTime = task.DeadLine.AddHours(-userSettings.NotifyBeforeHours);

                if (DateTime.UtcNow >= notifyTime)
                {
                    await _emailService.SendEmailAsync(
                        "user-email@example.com", // Нужно заменить на реальный email пользователя
                        $"Reminder: {task.Title} is due soon!",
                        $"Your task '{task.Title}' is due at {task.DeadLine}.\n\nPlease complete it on time."
                    );

                    Console.WriteLine($"[x] Notification sent for Task ID: {task.Id}");
                }
            }
        }

        private UserNotificationSettings GetUserSettings(int userId)
        {
            return _context.NotificationSettings.Where(s => s.UserId == userId)
                                                .FirstOrDefault();
        }
    }
}
