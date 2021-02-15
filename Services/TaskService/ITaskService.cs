using Models;
using System;
using System.Threading.Tasks;

namespace Services.TaskService
{
    public interface ITaskService
    {
        public Task<ServiceTask> CreateTaskAsync();
        Task<ServiceTask> GetTaskAsync(Guid taskId);
        Task UpdateTaskStatusAsync(Guid taskId, ServiceTaskStatus newStatus, Guid? resourceId = null);
    }
}
