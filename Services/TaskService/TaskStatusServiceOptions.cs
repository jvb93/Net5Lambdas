namespace Services.TaskService
{
    public class TaskStatusServiceOptions
    {
        public string TableName { get; set; }
        public static string OptionsTableName => "TASK_TABLE_NAME";
    }
}
