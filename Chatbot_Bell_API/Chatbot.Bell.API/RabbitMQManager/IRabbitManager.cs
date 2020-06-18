namespace Chatbot.Bell.API.RabbitMQManager
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string queueName)
            where T : class;
    }
}