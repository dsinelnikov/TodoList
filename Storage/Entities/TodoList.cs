using System;
using System.Collections.Generic;

namespace TodoListApi.Storage.Entities
{
    public class TodoList
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TodoListTask> Tasks { get; set; }
    }
}