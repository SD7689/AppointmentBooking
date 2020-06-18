using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Chatbot.Bell.API.RabbitMQManager
{
    public class RabbitManager : IRabbitManager
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string queueName)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                var MessageSerialize = JsonSerializer.Serialize(message);
                var sendBytes = Encoding.UTF8.GetBytes(MessageSerialize);
                channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
                IBasicProperties basicProperties = channel.CreateBasicProperties();
                channel.BasicPublish(exchange:"", routingKey: queueName, basicProperties: basicProperties, body: sendBytes);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
    }
}