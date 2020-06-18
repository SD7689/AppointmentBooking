using Chatbot.Bell.API.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Processor
{
    public class ChatBotBusinessLogic
    {
        #region Variable_Declaration
        private readonly ILogger<ChatBotBusinessLogic> _logger;
        private readonly IMemoryCache memoryCache;
        public ChatBotBusinessLogic(ILogger<ChatBotBusinessLogic> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            this.memoryCache = memoryCache;
        }
        #endregion

        #region JWT Token
        internal string Generate_DR_JSONWebToken(AuthUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.SecurityKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                new Claim("Program_Code", userInfo.Program_Code),
                new Claim("Module", "DigitalReceipt"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(AppConfig.SecurityKey, AppConfig.Audience_DR, claims, expires: DateTime.Now.AddMinutes(Convert.ToInt32(AppConfig.TokenExpiryTimeInMinuts)), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        #endregion

        #region RabbitMQ
        public bool QueueDigitalReceiptItems(string billIMaster, string ProgCode)
        {
            bool isError = false;
            try
            {
                var factory = new ConnectionFactory() { HostName = AppConfig.RabitMQHostName, UserName = AppConfig.RabbitMQUserName, Password = AppConfig.RabbitMQPassword };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Random rnd = new Random();
                    string Index = Convert.ToString(rnd.Next(1, Convert.ToInt16(AppConfig.RabbitMQQueuesLength)));
                    string currentRabbitMQQueuesName = String.Format(AppConfig.RabbitMQQueuesName, Convert.ToString(ProgCode).Trim().ToLower(), Index);
                    channel.QueueDeclare(queue: currentRabbitMQQueuesName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //var body = Encoding.UTF8.GetBytes(ObjShoppingBagInfo);
                    byte[] messagebuffer = Encoding.Default.GetBytes(billIMaster);


                    channel.BasicPublish(exchange: "",
                                                     routingKey: currentRabbitMQQueuesName,
                                                     basicProperties: null,
                                                     body: messagebuffer);

                }
            }
            catch (Exception Ex)
            {

            }
            return isError;
        }

        public bool QueueDigitalReceiptCommunicationInfo(string commInfo, string ProgCode)
        {
            bool isError = false;
            try
            {
                var factory = new ConnectionFactory() { HostName = AppConfig.RabitMQHostName, UserName = AppConfig.RabbitMQUserName, Password = AppConfig.RabbitMQPassword };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Random rnd = new Random();
                    string Index = Convert.ToString(rnd.Next(1, Convert.ToInt16(AppConfig.CommunicationMQQueuesLength)));
                    string currentRabbitMQQueuesName = String.Format(AppConfig.RabbitMQQueuesCommunicationName, Convert.ToString(ProgCode).Trim().ToLower(), Index);
                    channel.QueueDeclare(queue: currentRabbitMQQueuesName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //var body = Encoding.UTF8.GetBytes(ObjShoppingBagInfo);
                    byte[] messagebuffer = Encoding.Default.GetBytes(commInfo);


                    channel.BasicPublish(exchange: "",
                                                     routingKey: currentRabbitMQQueuesName,
                                                     basicProperties: null,
                                                     body: messagebuffer);

                }
            }
            catch (Exception Ex)
            {

            }
            return isError;
        }

        public bool FailoverQueueDigitalReceipt(string billIMaster, string ProgCode)
        {
            bool isError = false;
            try
            {
                var factory = new ConnectionFactory() { HostName = AppConfig.RabitMQHostName, UserName = AppConfig.RabbitMQUserName, Password = AppConfig.RabbitMQPassword };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Random rnd = new Random();
                    string Index = Convert.ToString(rnd.Next(1, Convert.ToInt16(AppConfig.RabbitMQQueuesLength)));
                    string currentRabbitMQQueuesName = String.Format(AppConfig.FailoverMQQueuesVendorName, Convert.ToString(ProgCode).Trim().ToLower(), Index);
                    channel.QueueDeclare(queue: currentRabbitMQQueuesName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //var body = Encoding.UTF8.GetBytes(ObjShoppingBagInfo);
                    byte[] messagebuffer = Encoding.Default.GetBytes(billIMaster);


                    channel.BasicPublish(exchange: "",
                                                     routingKey: currentRabbitMQQueuesName,
                                                     basicProperties: null,
                                                     body: messagebuffer);

                }
            }
            catch (Exception Ex)
            {

            }
            return isError;
        }

        #endregion
    }
}
