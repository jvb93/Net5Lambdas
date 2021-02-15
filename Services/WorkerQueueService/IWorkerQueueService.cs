using System.Threading.Tasks;

namespace Services.WorkerQueueService
{
    public interface IWorkerQueueService
    {
        Task SendMessageAsync(string messageBody);
    }
}
