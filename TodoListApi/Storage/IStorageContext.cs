using InMemoryStorage;
using System;
using TodoListApi.Storage.Entities;

namespace TodoListApi.Storage
{
    public interface IStorageContext
    {
        IEntityStorage<TodoList, Guid> TodoLists { get; }

        IEntityStorage<TodoListTask, Guid> Tasks { get; }
    }
}
