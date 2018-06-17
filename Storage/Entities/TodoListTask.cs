using System;

namespace TodoListApi.Storage.Entities
{
    public class TodoListTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }

        public TodoList TodoList { get; set; }
    }
}