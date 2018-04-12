using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace FSS.Omnius.Modules.Nexus.Service
{
    class RabbitMQService
    {
        private IConnection connection;
        private IModel channel;
        private string queueName;

        public RabbitMQService(string hostName, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            
            this.queueName = queueName;
        }

        ~RabbitMQService()
        {
            channel.Close();
            channel.Dispose();
            connection.Close();
            connection.Dispose();
        }

        public void Send(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish("amq.direct", queueName, properties, body);
        }
    }
}
