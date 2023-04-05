using CMDEFLayer;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMDWebAPI.Services
{
    public class SenderService : ISenderService
    {
        private readonly IConfiguration _config;

        public SenderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessageAsync<T>(T model, string queueName)
        {
            var connectionString = _config.GetConnectionString("ServiceBusConn");
            var qClient = new QueueClient(connectionString, queueName);
            var msgBody = JsonSerializer.Serialize(model);
            var msg = new Message(Encoding.UTF8.GetBytes(msgBody));
            await qClient.SendAsync(msg);
        }
    }
}
