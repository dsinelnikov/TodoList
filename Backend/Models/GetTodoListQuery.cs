namespace TodoList.Backend.Models
{
    public class GetTodoListQuery
    {
        public int Offset { get; set; }

        public int Count { get; set; }

        public string Filter { get; set; }

        public bool IncludeTasks { get; set; }
    }
}