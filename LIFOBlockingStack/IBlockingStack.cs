/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoundedBlockingStack
{
    /// <summary>
    /// Defines the methods of the LIFO blocking stack structure: Push, Pop.
    /// </summary>
    interface IBlockingStack
    {
        void Push(object element); //Adds an element at the top of the stack

        object Pop(); //Removes an element from the top of the stack
    }
}
