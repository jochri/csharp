/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FifoBlockingQueue
{
    /// <summary> FIFO fix-sized Bounded Blocking Queue structure to be used in vis-à-vis multithreading environments.
    /// </summary>
    public class BoundedBlockingQueue : IBlockingQueue
    {
        //private FIFO queue fields **/
        private object[] queue { get; set; }
        private int capacity { get; set; }
        private int count { get; set; }
        private int head { get; set; }
        private int tail { get; set; }
        private int insertedElementsCounter { get; set; }
        private int deletedElementsCounter { get; set; }

        //constructor, initializing fields
        public BoundedBlockingQueue(int capacity)
        {
            //check given size and throw exception
            if (capacity < 1)
                throw new InvalidQueueSizeException("Invalid queue size entered : " + capacity + ". It should be greater than zero.");

            this.capacity = capacity; //define size
            queue = new object[capacity]; //initialize queue capacity
            count = 0; //initialize numbers of elements in the queue
            head = 0; //head starts at zero position index
            tail = 0; //tail starts at zero position index
            insertedElementsCounter = 0;
            deletedElementsCounter = 0;
        }

        //Returns the number of elements in the queue
        public int Count()
        {
            return count;
        }

        //Returns the capacity of the queue
        public int Capacity()
        {
            return capacity;
        }

        //Checks if queue is empty
        public bool IsEmpty()
        {
            if (count == 0)
                return true; //queue is empty

            return false;
        }

        //Checks if queue is full
        public bool IsFull()
        {
            if (count == capacity)
                return true; //queue is full
            return false;
        }


        public void Put(object element)
        {
            if (element == null)
                throw new NullElementException("Element to add to the queue cannot be null.");
      
            lock (this) //aquire the lock
            {
                if(IsFull()) //queue is full, signal get() thread to release the lock
                {
                    try
                    {
                        Monitor.Wait(this); //signal to wait
                    }
                    catch (SynchronizationLockException sle)
                    {
                        Console.WriteLine(sle);
                    }
                    catch (ThreadInterruptedException tie)
                    {
                        Console.WriteLine(tie);
                    }

                }
                insertedElementsCounter++; //up the counter of inserted element
                queue[tail] = element; //enqueue element at tail position
                count++; //up the counter of inserted element
                tail++; //reposition tail index
                if (tail == capacity) //tail reached the end of array                
                    tail = 0; //reset tail position to index 0
                
                Console.WriteLine("Produce: " + element); //write item
                Monitor.Pulse(this); //tell procuder that consumer is done
            }
            

        }

        public object Get()
        {
            object item = null; //item to return
            lock (this) //aquire the lock
            {
                if (IsEmpty()) //queue is empty
                {
                    try
                    {
                        Monitor.Wait(this); //signal put() thread to release the lock
                    }
                    catch (SynchronizationLockException sle)
                    {
                        Console.WriteLine(sle);
                    }
                    catch (ThreadInterruptedException tie)
                    {
                        Console.WriteLine(tie);
                    }
                }
                deletedElementsCounter++;
                if (count != 0) //exists item to get
                {
                    item = queue[head]; //get item from head position
                    count--; //decrease the count of items
                    head++; //reposition head index
                }

                if (head == capacity) //head reached the end of the queue
                    head = 0; //reset head position to index 0

                Console.WriteLine("Consume: " + item); //write item
                Monitor.Pulse(this); //tell procuder that consumer is done
            }            
            return item;
        }
    }
}
