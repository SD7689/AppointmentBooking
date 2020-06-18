using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace Chatbot.Bell.API.RabbitMQManager
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitOptions _options;

        private readonly IConnection _connection;
        private readonly ILogger<RabbitModelPooledObjectPolicy> _logger;

        public RabbitModelPooledObjectPolicy(ILogger<RabbitModelPooledObjectPolicy> logger, IOptions<RabbitOptions> optionsAccs)
        {
            _logger = logger;
            _options = optionsAccs.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = Convert.ToInt32( _options.Port),
                VirtualHost = _options.VHost,
            };
            _logger.LogInformation($"usign rabbitmq HostName: {_options.HostName}");
            _logger.LogInformation($"usign rabbitmq UserName: {_options.UserName}");
            _logger.LogInformation($"usign rabbitmq Password: {_options.Password}");
            _logger.LogInformation($"usign rabbitmq Port: {Convert.ToInt32(_options.Port)}");
            _logger.LogInformation($"usign rabbitmq VHost: {_options.VHost}");
            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}