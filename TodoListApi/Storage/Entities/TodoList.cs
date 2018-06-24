using InMemoryStorage;
using System;

namespace TodoListApi.Storage.Entities
{
    public class TodoList : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}