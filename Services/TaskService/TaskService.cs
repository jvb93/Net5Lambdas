using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly IAmazonDynamoDB _client;

        private readonly string _tableName;

        public TaskService(IAmazonDynamoDB client, IOptions<TaskServiceOptions> options)
        {
            _client = client;
            _tableName = options.Value.TableName;
        }

        public async Task<ServiceTask> CreateTaskAsync()
        {
            var now = DateTimeOffset.Now;
            var taskId = Guid.NewGuid();
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "TaskId", new AttributeValue { S = taskId.ToString() }},
                    { "Status", new AttributeValue { S = ServiceTaskStatus.Pending.ToString() }},
                    { "Created", new AttributeValue { S = now.ToString() }},
                    { "LastUpdated", new AttributeValue { S = now.ToString() }},
                }
            };

            var response = await _client.PutItemAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return new ServiceTask()
                {
                    Created = now,
                    LastUpdated = now,
                    ResourceId = null,
                    Status = ServiceTaskStatus.Pending,
                    TaskId = taskId
                };
            }

            return null;
        }

        public async Task<ServiceTask> GetTaskAsync(Guid taskId)
        {
            var request = GenerateQueryRequest(taskId);

            var queryResults = await _client.QueryAsync(request);

            return queryResults.Count == 0 ? null : CreateServiceTaskFromDynamoRow(queryResults.Items[0]);
        }

        private QueryRequest GenerateQueryRequest(Guid taskId)
        {
            var keyConditionExpression = "#TaskId = :TaskId";

            var expressionAttributeNames = new Dictionary<string, string>();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            expressionAttributeNames.Add("#TaskId", "TaskId");
            expressionAttributeValues.Add(":TaskId", new AttributeValue { S = taskId.ToString() });

            return new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = keyConditionExpression,
                ExpressionAttributeNames = expressionAttributeNames,
                ExpressionAttributeValues = expressionAttributeValues
            };
        }

        public async Task UpdateTaskStatusAsync(Guid taskId, ServiceTaskStatus newStatus, Guid? resourceId = null)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                ReturnValues = ReturnValue.ALL_OLD,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "TaskId", new AttributeValue { S = taskId.ToString() }}
                },
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#S", "Status"},
                    {"#LU", "LastUpdated"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":status", new AttributeValue { S = newStatus.ToString() }},
                    {":lastUpdated", new AttributeValue { S = DateTimeOffset.Now.ToString() }}
                },
                UpdateExpression = "SET #S = :status, #LU = :lastUpdated"
            };

            if (resourceId != null)
            {
                request.ExpressionAttributeNames.Add("#RID", "ResourceId");
                request.ExpressionAttributeValues.Add(":resourceId", new AttributeValue { S = resourceId.ToString() });
                request.UpdateExpression += ", #RID = :resourceId";
            }
            
            await _client.UpdateItemAsync(request);
        }


        private ServiceTask CreateServiceTaskFromDynamoRow(IDictionary<string, AttributeValue> entry)
        {
            return new ServiceTask
            {
                TaskId = Guid.Parse(entry["TaskId"].S),
                ResourceId = Guid.TryParse(entry["ResourceId"].S, out var i) ? (Guid?)i : null,
                Status = (ServiceTaskStatus)Enum.Parse(typeof(ServiceTaskStatus), entry["Status"].S),
                Created = DateTimeOffset.Parse(entry["Created"].S),
                LastUpdated = DateTimeOffset.Parse(entry["LastUpdated"].S),
            };
        }
    }
}