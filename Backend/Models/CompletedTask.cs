using System.ComponentModel.DataAnnotations;

namespace TodoList.Backend.Models
{
    public class CompletedTask
    {
        [Required]
        public bool? Completed { get; set; }
    }
}