using CMDEFLayer;
using System.Threading.Tasks;

namespace CMDWebAPI.Services
{
    public interface ISenderService
    {
        Task SendMessageAsync<T>(T item, string queueName);
    }
}