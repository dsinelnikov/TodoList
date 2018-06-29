using System;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Backend.Models;

namespace TodoList.Backend.Services
{
    public interface ITodoListTasksService
    {
        Task<TodoListTask> GetTaskAsync(Guid id, CancellationToken cancelationToken);

        Task AddTaskAsync(Guid listId, TodoListTask task, CancellationToken cancelationToken);        

        Task UpdateTaskAsync(TodoListTask task, CancellationToken cancelationToken);
    }
}
