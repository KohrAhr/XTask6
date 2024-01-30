using Lib.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace Lib.RabbitMQ
{
    public class RabbitMQHelper : IRabbitMQHelper
    {
        private ILogger? _logger = null;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        public RabbitMQHelper() 
        { 
        }

        public void SetLogger(ILogger aLogger) 
        { 
            _logger = aLogger;
        }

        /// <summary>
        ///     Init RabbitMQ pipeline
        /// </summary>
        /// <param name="aHostname"></param>
        /// <param name="aChannelName"></param>
        /// <param name="connectionFactory"></param>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        public void InitRabbitMQ(string aHostname, string aChannelName, out ConnectionFactory? connectionFactory, out IConnection connection, out IModel? model)
        {

            connectionFactory = new ConnectionFactory { HostName = aHostname };
            try
            {
                connection = connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                _logger?.LogCritical("Broker is unreachable. Please verify that RabbitMQ service is up and running on {connectionFactory.HostName}", connectionFactory.HostName);
                throw new Exception(ex.Message, ex);
            }
            model = connection.CreateModel();

            model.QueueDeclare(aChannelName, durable: true, false, false, null);
        }

        public void SendMessage(IModel? model, string aChannelName, string aMessage)
        {
            byte[] body = Encoding.UTF8.GetBytes(aMessage);

            model?.BasicPublish(string.Empty, aChannelName, null, body);
        }

        public string GetMessage(BasicDeliverEventArgs aArguments)
        {
            byte[] body = aArguments.Body.ToArray();
            string result = Encoding.UTF8.GetString(body);

            return result;
        }
    }
}