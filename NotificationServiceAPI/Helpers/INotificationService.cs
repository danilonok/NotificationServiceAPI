namespace NotificationServiceAPI.Helpers
{
    public interface INotificationService
    {
        Task ProcessTaskAsync(TaskMessage task);
    }
}
