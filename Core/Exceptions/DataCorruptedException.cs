using System;
using System.Collections.Generic;
using System.Text;

namespace TodoList.Core.Exceptions
{
    public class DataCorruptedException : Exception
    {
        public DataCorruptedException()
            : this("Data access error. Some data can be corrupted.", null)
        {

        }

        public DataCorruptedException(string message)            
            :this(message, null)
        {

        }

        public DataCorruptedException(string message, Exception innerException)
            :base(message, innerException)
        {

        }
    }
}
