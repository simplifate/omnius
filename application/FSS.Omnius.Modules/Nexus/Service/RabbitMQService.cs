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
        private Entitron.Entity.Nexus.RabbitMQ mq;

        public RabbitMQService(Entitron.Entity.Nexus.RabbitMQ mq)
        {
            this.mq = mq;

            var factory = new ConnectionFactory() { HostName = mq.HostName, Port = mq.Port };
            if(!string.IsNullOrEmpty(mq.UserName) && !string.IsNullOrEmpty(mq.Password)) {
                factory.UserName = mq.UserName;
                factory.Password = mq.Password;
            }

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Close()
        {
            connection.Close(0);
            connection.Dispose();
        }

        public void Send(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish("", mq.QueueName, properties, body);
        }
    }
}
