/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIFOBlockingStack
{
    /// <summary>
    /// Thrown when element to be pushed to the stack is found to be null.
    /// </summary>
    internal class NullElementException : Exception
    {
        internal NullElementException() : base() { }
        internal NullElementException(String message) : base(message) { }
        internal NullElementException(String message, Exception inner) : base(message, inner) { }
    }
}
