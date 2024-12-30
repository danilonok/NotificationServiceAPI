using NotificationServiceAPI.Data;
using NotificationServiceAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
namespace NotificationServiceAPI.Helpers
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq-service",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "my-queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[x] Received message: {message}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                    var task = JsonSerializer.Deserialize<TaskMessage>(message);

                    var notificationTask = new NotificationTask
                    {
                        TaskId = task.Id,
                        UserId = task.UserID,
                        Title = task.Title,
                        DeadLine = task.DeadLine
                    };

                    dbContext.NotificationTasks.Add(notificationTask);
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine($"Task saved to DB: {task.Title}");
                }
            };

            _channel.BasicConsume(queue: "my-queue",
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

    public class TaskMessage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DeadLine { get; set; }
        public int UserID { get; set; }
    }
}
