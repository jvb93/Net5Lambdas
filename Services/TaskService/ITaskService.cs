using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Services.TaskService
{
    public interface ITaskService
    {
        public Task<Guid?> CreateTaskAsync();
    }
}
