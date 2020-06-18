using Microsoft.Extensions.Configuration;
using System;

namespace Chatbot.Bell.API.RabbitMQManager
{
    public class RabbitOptions
    {
        static IConfiguration apConfig = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", true, true)
       .Build();
        public string UserName { get { return apConfig["ElasticSearchSettings:RabbitMQ:UserName"]; } }
        public string Password { get { return apConfig["ElasticSearchSettings:RabbitMQ:Password"]; } }
        public string HostName { get { return apConfig["ElasticSearchSettings:RabbitMQ:HostName"]; } } 
        public string Port { get  { return apConfig["ElasticSearchSettings:RabbitMQ:Port"]; } }

        public string VHost { get { return apConfig["ElasticSearchSettings:RabbitMQ:VHost"]; } }
    }
}