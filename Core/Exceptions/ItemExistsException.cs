using System;

namespace TodoList.Core.Exceptions
{
    public class ItemExistsException : Exception
    {
        public object Id { get; }

        public ItemExistsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ItemExistsException(string message)
            :this(message, null)
        {

        }

        public ItemExistsException(object id, Exception innerException)
            :this($"Item '{id}' is alredy exists.", innerException)
        {
            Id = id;
        }

        public ItemExistsException(object id)
            :this(id, null)
        {

        }
    }
}
