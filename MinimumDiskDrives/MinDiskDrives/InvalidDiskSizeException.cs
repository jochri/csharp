// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinDiskDrives
{
    /// <summary>
    /// Thrown if disk size gien is invalid.
    /// </summary>
    internal class InvalidDiskSizeException : Exception
    {
      internal InvalidDiskSizeException() : base() {}
      internal InvalidDiskSizeException(String message) : base(message) { }
      internal InvalidDiskSizeException(String message, Exception inner) : base(message, inner) { }
    }
}
