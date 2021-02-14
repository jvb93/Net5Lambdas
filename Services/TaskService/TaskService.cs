using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;

namespace Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly IAmazonDynamoDB _client;

        private readonly string _tableName;

        public TaskService(IAmazonDynamoDB client, IOptions<TaskStatusServiceOptions> options)
        {
            _client = client;
            _tableName = options.Value.TableName;
        }

        public async Task<Guid?> CreateTaskAsync()
        {
            var now = DateTimeOffset.Now.ToString();
            var taskId = Guid.NewGuid();
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "TaskId", new AttributeValue { S = taskId.ToString() }},
                    { "Status", new AttributeValue { S = "Pending" }},
                    { "Created", new AttributeValue { S = now }},
                    { "LastUpdated", new AttributeValue { S = now }},
                }
            };

            var response = await _client.PutItemAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return taskId;
            }

            return null;
        }
    }
}