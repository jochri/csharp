/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifoBlockingQueue
{
    /// <summary> Defines the methods of the FIFO bounded blocking queue structure
    /// Structure is capable of put(add) and get(remove) operations.
    /// </summary>
    interface IBlockingQueue
    {
        /**
         * Add an element to the queue.
         * Attempts to add an element to a full queue should block the calling thread.
         * 
         */
        void Put(Object element);

        /**
         * Remove an element from the queue.
         * Attempts to remove an element from an empty queue should block the calling thread.
         * 
         */
        Object Get();
    }
}
