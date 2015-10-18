// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinDiskDrives
{
    /// <summary>
    /// Thrown when used and total element arrays length are not equal.
    /// </summary>
   internal class UnequalArrayLengthException : Exception
   {
      internal UnequalArrayLengthException() : base() {}
      internal UnequalArrayLengthException(String message) : base(message) { }
      internal UnequalArrayLengthException(String message, Exception inner) : base(message, inner) { }
   }
}
