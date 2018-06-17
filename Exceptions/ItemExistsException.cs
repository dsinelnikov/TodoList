using System;

namespace TodoListApi.Exceptions
{
    public class ItemExistsException : Exception
    {
        public Guid? Id { get; }

        public ItemExistsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ItemExistsException(string message)
            :this(message, null)
        {

        }

        public ItemExistsException(Guid id, Exception innerException)
            :this($"Item '{id}' is alredy exists.", innerException)
        {
            Id = id;
        }

        public ItemExistsException(Guid id)
            :this(id, null)
        {

        }
    }
}
