using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lib.RabbitMQ.Interfaces
{
    public interface IRabbitMQHelper
    {
        void InitRabbitMQ(string aHostname, string aChannelName, out ConnectionFactory? connectionFactory, out IConnection connection, out IModel? model);

        void SendMessage(IModel? model, string aChannelName, string aMessage);

        string GetMessage(BasicDeliverEventArgs aArguments);
    }
}
