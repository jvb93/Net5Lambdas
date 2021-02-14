using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Services.TaskService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ITaskService, TaskService>();
            serviceCollection.AddAWSService<IAmazonDynamoDB>();

            serviceCollection.AddOptions()
                .Configure<TaskStatusServiceOptions>(o =>
                {
                    o.TableName = configuration.GetSection(TaskStatusServiceOptions.OptionsTableName).Value;
                });

            serviceCollection.AddTransient(provider => provider.GetRequiredService<IOptionsMonitor<TaskStatusServiceOptions>>().CurrentValue);

            return serviceCollection;
        }
    }
}
