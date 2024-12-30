using System.ComponentModel.DataAnnotations;

namespace NotificationServiceAPI.Models
{
    public class UserNotificationSettings
    {
        [Key]
        public int UserId { get; set; }
        public string Email { get; set; }
        public int NotifyBeforeHours { get; set; } = 1; 
        public bool IsNotificationEnabled { get; set; } = true;
    }
}
