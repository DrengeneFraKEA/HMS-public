using RabbitMQ.Client;
using System.Text;

namespace HMS.Data
{
    public class RabbitMQPublisher
    {
        private readonly string _hostName;
        private readonly string _queueName;

        public RabbitMQPublisher(string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
        }

        public void PublishMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare the queue if it doesn't exist
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Convert the message to bytes
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message to the queue
                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
