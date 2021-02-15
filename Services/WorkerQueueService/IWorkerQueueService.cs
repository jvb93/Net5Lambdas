using System.Threading.Tasks;
using Models;

namespace Services.WorkerQueueService
{
    public interface IWorkerQueueService
    {
        Task SendMessageAsync(SqsMessageWrapper message);
    }
}
