using System.ComponentModel.DataAnnotations;

namespace TodoListApi.Models
{
    public class CompletedTask
    {
        [Required]
        public bool? Completed { get; set; }
    }
}