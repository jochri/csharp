/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIFOBlockingStack
{
    /// <summary>
    /// Thrown when the stack structure is initialized with invalid size.
    /// </summary>
    internal class InvalidStackSizeException : Exception
    {
        internal InvalidStackSizeException() : base() { }
        internal InvalidStackSizeException(String message) : base(message) { }
        internal InvalidStackSizeException(String message, Exception inner) : base(message, inner) { }
    }
}
