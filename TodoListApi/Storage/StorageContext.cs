using InMemoryStorage;
using System;
using TodoListApi.Storage.Entities;

namespace TodoListApi.Storage
{
    public class StorageContext : IStorageContext
    {
        public IEntityStorage<TodoList, Guid> TodoLists { get; } = new EntityStorage<TodoList, Guid>();

        public IEntityStorage<TodoListTask, Guid> Tasks { get; } = new EntityStorage<TodoListTask, Guid>();
    }
}
