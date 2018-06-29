using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Backend.Models;

namespace TodoList.Backend.Services
{
    public interface ITodoListService
    {
        Task<IEnumerable<TodoListItem>> GetTodoListsAsync(GetTodoListQuery query, CancellationToken cancelationToken);

        Task<TodoListItem> GetTodoListAsync(Guid id, CancellationToken cancelationToken);

        Task AddTodoListAsync(TodoListItem list, CancellationToken cancelationToken);
    }
}