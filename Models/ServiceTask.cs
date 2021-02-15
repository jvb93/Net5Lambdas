using System;

namespace Models
{
    public class ServiceTask
    {
        public Guid TaskId { get; set; }
        public Guid? ResourceId { get; set; }
        public ServiceTaskStatus Status { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    public enum ServiceTaskStatus
    {
        Pending,
        InProgress,
        Complete,
        Failed
    }
}
