using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListApi.Models;

namespace TodoListApi.Services
{
    public interface ITodoListService
    {
        Task<List<TodoList>> GetTodoListsAsync(GetTodoListQuery query);

        Task<TodoList> GetTodoListAsync(Guid id);

        Task AddTodoListAsync(TodoList list);

        Task AddTaskAsync(Guid listId, TodoListTask task);

        Task CompleteTaskAsync(Guid id, bool isCompleted);
    }
}