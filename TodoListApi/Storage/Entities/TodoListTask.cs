using InMemoryStorage;
using System;

namespace TodoListApi.Storage.Entities
{
    public class TodoListTask : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid ListId { get; set; }

        public string Name { get; set; }

        public bool Completed { get; set; }
    }
}