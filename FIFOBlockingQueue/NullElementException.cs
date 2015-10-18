/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifoBlockingQueue
{
    /// <summary>
    /// Thrown when element to be added to the queue structure is found to be null.
    /// </summary>
    internal class NullElementException : Exception
    {
        internal NullElementException() : base() { }
        internal NullElementException(String message) : base(message) { }
        internal NullElementException(String message, Exception inner) : base(message, inner) { } 
    }
}
