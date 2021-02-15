using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Services.LambdaHostBuilder;
using Services.TaskService;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Net5Lambdas.Workers.Cars
{
    public class Create
    {
        private readonly ILambdaHostBuilder _hostBuilder;

        public Create() : this(LambdaHostBuilder.Host)
        {
        }

        public Create(ILambdaHostBuilder lambdaHostBuilder)
        {
            _hostBuilder = lambdaHostBuilder;
        }


        /// <summary>
        ///     A Lambda function to respond to SQS messages
        /// </summary>
        /// <param name="request"></param>
        public async Task CreateCar(SQSEvent request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine("creating car\n");

                if (_hostBuilder.TryBuild(context, () => new LambdaStartup(), out var serviceProvider))
                {
                    var queueMessage = JsonSerializer.Deserialize<SqsMessageWrapper>(request.Records.First().Body);
                    using var container = serviceProvider.CreateScope();
                    var taskService = container.ServiceProvider.GetRequiredService<ITaskService>();

                    await taskService.UpdateTaskStatusAsync(queueMessage.TaskId, ServiceTaskStatus.InProgress);
                    //simulate 5 second database latency
                    await Task.Delay(5000);

                    await taskService.UpdateTaskStatusAsync(queueMessage.TaskId, ServiceTaskStatus.Complete,
                        Guid.NewGuid());
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.Message);
            }
          
        }
    }
}