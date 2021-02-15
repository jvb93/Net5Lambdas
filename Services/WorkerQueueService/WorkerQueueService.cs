using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

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

        public async Task SendMessageAsync(string messageBody)
        {
            var sendMessageRequest = new SendMessageRequest()
            {
                MessageBody = messageBody,
                QueueUrl = _options.Value.QueueUrl
            };

            await _sqs.SendMessageAsync(sendMessageRequest);
        }
    }
}