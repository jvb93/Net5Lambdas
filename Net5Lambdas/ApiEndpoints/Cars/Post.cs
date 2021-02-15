using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Services.LambdaHostBuilder;
using Services.TaskService;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Models;
using Services.WorkerQueueService;

namespace Net5Lambdas.ApiEndpoints.Cars
{
    public class Post
    {
        private readonly ILambdaHostBuilder _hostBuilder;

        public Post() : this(LambdaHostBuilder.Host)
        {
        }

        public Post(ILambdaHostBuilder lambdaHostBuilder)
        {
            _hostBuilder = lambdaHostBuilder;
        }


        /// <summary>
        ///     A Lambda function to respond to HTTP Post methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> CreateCar(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("post Request\n");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.OK,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };

            if (_hostBuilder.TryBuild(context, () => new LambdaStartup(), out var serviceProvider))
            {
                using var container = serviceProvider.CreateScope();

                var taskService = container.ServiceProvider.GetRequiredService<ITaskService>();

                var serviceTask = await taskService.CreateTaskAsync();

                var workerQueueService = container.ServiceProvider.GetRequiredService<IWorkerQueueService>();

                await workerQueueService.SendMessageAsync(new SqsMessageWrapper()
                {
                    Payload = "",
                    TaskId = serviceTask.TaskId
                });


                response.Body = JsonSerializer.Serialize(serviceTask);
            }

            else
            {
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}