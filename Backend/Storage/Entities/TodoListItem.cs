using System;
using TodoList.InMemoryStorage;

namespace TodoList.Backend.Storage.Entities
{
    public class TodoListItem : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}