namespace NotificationServiceAPI.Models
{
    public class NotificationTask
    {
        public int Id { get; set; }
        public int TaskId { get; set; }  
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime DeadLine { get; set; }
        public bool NotificationSent { get; set; } = false;
    }
}
