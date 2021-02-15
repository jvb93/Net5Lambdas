using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.TaskService;

namespace Services.WorkerQueueService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkerQueueService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IWorkerQueueService, WorkerQueueService>();
            serviceCollection.AddAWSService<IAmazonSQS>();

            serviceCollection.AddOptions()
                .Configure<WorkerQueueServiceOptions>(o =>
                {
                    o.QueueUrl = configuration.GetSection(WorkerQueueServiceOptions.OptionsQueueUrl).Value;
                });

            serviceCollection.AddTransient(provider => provider.GetRequiredService<IOptionsMonitor<WorkerQueueServiceOptions>>().CurrentValue);

            return serviceCollection;
        }
    }
}
