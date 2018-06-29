using System;
using TodoList.Backend.Storage.Entities;
using TodoList.InMemoryStorage;

namespace TodoList.Backend.Storage
{
    public class StorageContext : IStorageContext
    {
        public IEntityStorage<TodoListItem, Guid> TodoLists { get; } = new EntityStorage<TodoListItem, Guid>();

        public IEntityStorage<TodoListTask, Guid> Tasks { get; } = new EntityStorage<TodoListTask, Guid>();
    }
}
