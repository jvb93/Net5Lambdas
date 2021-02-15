using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.WorkerQueueService
{
    public class WorkerQueueService : IWorkerQueueService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<WorkerQueueServiceOptions> _options;
        public WorkerQueueService(IAmazonSQS sqs, IOptions<WorkerQueueServiceOptions> options)
        {
            _sqs = sqs;
            _options = options;
        }

        public async Task SendMessageAsync(SqsMessageWrapper message)
        {
            var sendMessageRequest = new SendMessageRequest()
            {
                MessageBody = JsonSerializer.Serialize(message),
                QueueUrl = _options.Value.QueueUrl
            };

            await _sqs.SendMessageAsync(sendMessageRequest);
        }
    }
}