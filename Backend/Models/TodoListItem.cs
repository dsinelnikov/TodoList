using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Backend.Models
{
    public class TodoListItem
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        private List<TodoListTask> _tasks;
        public List<TodoListTask> Tasks
        {
            get => _tasks ?? (_tasks = new List<TodoListTask>());
            set => _tasks = value;
        }
    }
}