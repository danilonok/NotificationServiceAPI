using MassTransit.Initializers.Variables;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using NotificationServiceAPI.Data;
using NotificationServiceAPI.Models;
using System;

namespace NotificationServiceAPI.Helpers
{
    public class NotificationChecker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailService _emailService;


        public NotificationChecker(IServiceProvider serviceProvider,IEmailService emailService)
        {
            _serviceProvider = serviceProvider;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Checking for notifications...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                    var now = DateTime.UtcNow;

                    var pendingTasks = dbContext.NotificationTasks
                        .Where(task => !task.NotificationSent)
                        .ToList();
                    Console.WriteLine($"Found {pendingTasks.Count} notifications");
                    foreach (var task in pendingTasks)
                    {

                        var settings = GetUserSettings(dbContext, task.UserId);
                        var delta = now.AddHours(settings.NotifyBeforeHours);

                        if (task.DeadLine >= delta)
                        {
                            continue;
                        }

                        var userEmail = settings.Email;


                        
                        await _emailService.SendEmailAsync(
                                userEmail,
                               $"Reminder: {task.Title} is due soon!",
                               $"Your task '{task.Title}' is due at {task.DeadLine}.\n\nPlease complete it on time."
                        );

                        
                        task.NotificationSent = true;
                        dbContext.NotificationTasks.Update(task);
                        Console.WriteLine($"Notification sent for Task ID: {task.TaskId}");
                    }

                    await dbContext.SaveChangesAsync();

                    
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        private UserNotificationSettings GetUserSettings(AppDBContext _context, int userId)
        {
            return _context.NotificationSettings.Where(s => s.UserId == userId)
                                                .FirstOrDefault();
        }
    }


   
}
