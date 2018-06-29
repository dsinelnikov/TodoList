using System;
using TodoList.Backend.Storage.Entities;
using TodoList.InMemoryStorage;

namespace TodoList.Backend.Storage
{
    public interface IStorageContext
    {
        IEntityStorage<TodoListItem, Guid> TodoLists { get; }

        IEntityStorage<TodoListTask, Guid> Tasks { get; }
    }
}
