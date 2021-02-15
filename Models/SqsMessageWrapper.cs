using System;

namespace Models
{
    public class SqsMessageWrapper
    {
        public Guid TaskId { get; set; }
        public string Payload { get; set; }
    }
}
