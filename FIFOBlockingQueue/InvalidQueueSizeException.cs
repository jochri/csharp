/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifoBlockingQueue
{
    /// <summary>
    /// Thrown when queue structure is initialized with invalid integer size.
    /// </summary>
    internal class InvalidQueueSizeException : Exception
    {
        internal InvalidQueueSizeException() : base() { }
        internal InvalidQueueSizeException(String message) : base(message) { }
        internal InvalidQueueSizeException(String message, Exception inner) : base(message, inner) { }
    }
}
