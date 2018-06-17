using System;

namespace TodoListApi.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public Guid Id { get; }

        public ItemNotFoundException(Guid id)
          : this(id, null)
        {            
        }

        public ItemNotFoundException(Guid id, Exception innerException)
            : base($"Item with Id:{id} is not found.", innerException)
        {
            Id = id;
        }
    }
}