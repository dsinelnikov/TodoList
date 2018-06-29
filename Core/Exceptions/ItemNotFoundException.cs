using System;

namespace TodoList.Core.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public object Id { get; }

        public ItemNotFoundException(object id)
          : this(id, null)
        {            
        }

        public ItemNotFoundException(object id, Exception innerException)
            : base($"Item with Id:{id} is not found.", innerException)
        {
            Id = id;
        }
    }
}