using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Backend.Models
{
    public class TodoListTask
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Completed { get; set; }
    }
}