using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Models;

namespace TodoListApi.Services
{
    public interface ITodoListService
    {
        Task<IEnumerable<TodoList>> GetTodoListsAsync(GetTodoListQuery query, CancellationToken cancelationToken);

        Task<TodoList> GetTodoListAsync(Guid id, CancellationToken cancelationToken);

        Task AddTodoListAsync(TodoList list, CancellationToken cancelationToken);
    }
}