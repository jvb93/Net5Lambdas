using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Services.LambdaHostBuilder;
using Services.TaskService;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Net5Lambdas
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
        /// A Lambda function to respond to HTTP Post methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Create(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("post Request\n");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            if (_hostBuilder.TryBuild(context, () => new LambdaStartup(), out var serviceProvider))
            {
                using var container = serviceProvider.CreateScope();

               

                var taskService = container.ServiceProvider.GetRequiredService<ITaskService>();

                var taskId = await taskService.CreateTaskAsync();


                response.Body = $"Created Task: {taskId}";

            }

            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;

        }
    }
}
