using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.TaskService;
using Services.WorkerQueueService;

namespace Services.LambdaHostBuilder
{
    public class LambdaStartup : ILambdaStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTaskService(configuration);
            services.AddWorkerQueueService(configuration);
        }
    }
}