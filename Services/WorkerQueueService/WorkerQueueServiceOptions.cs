namespace Services.WorkerQueueService
{
    public class WorkerQueueServiceOptions
    {
        public string QueueUrl { get; set; }
        public static string OptionsQueueUrl => "WORKER_QUEUE_URL";
    }
}
