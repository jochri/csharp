
/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LIFOBlockingStack
{
    /// <summary> LIFO fix-sized Bounded Blocking Sstack structure to be used in multithreading environments.
    /// </summary>
    public class BoundedBlockingStack : IBlockingStack
    {
        //private LIFO stack fields
        private object[] stack { get; set; } //the stack
        private int capacity { get; set; } //capacity of the stack
        private int count { get; set; } //counter of inserted elements
        private int top { get; set; } //top position index in array
        private int insertedElementsCounter { get; set; }
        private int deletedElementsCounter { get; set; }
        private List<object> inserted {get; set;}
        private List<object> deleted {get; set;}

        public BoundedBlockingStack(int capacity)
        {
            if(capacity < 1)
                throw new InvalidStackSizeException("Invalid stack size entered : " + capacity + ". It should be greater than zero.");

            /* Initialize stack properties */
            this.capacity = capacity;
            stack = new object[capacity];
            count = 0;
            top = capacity - 1; //stack is empty at initialization
            insertedElementsCounter = 0;
            deletedElementsCounter = 0;
            inserted = new List<object>();
            deleted = new List<object>();
        }

        //Returns the number of elements in the stack
        public int Count()
        {
            return count;
        }

        //Returns the capacity of the stack
        public int Capacity()
        {
            return capacity;
        }

        //Checks if stack is empty
        public bool IsEmpty()
        {
            if (count == 0)
                return true; //stack is empty

            return false;
        }

        //Checks if stack is full
        public bool IsFull()
        {
            if (count == capacity)
                return true; //stack is full
            return false;
        }

        //Returns an array of all pushed objects in the stack
        public object[] Pushed()
        {            
            object[] obj = new object[insertedElementsCounter];
            if (!insertedElementsCounter.Equals(0))
            {
                for (int i = 0; i < insertedElementsCounter; i++)
                    obj[i] = inserted[i];
            }
            return obj;
        }

        //Returns an array of all popped objects out of the stack
        public object[] Popped()
        {
            object[] obj = new object[deletedElementsCounter];
            if (!deletedElementsCounter.Equals(0))
            {
                for (int i = 0; i < deletedElementsCounter; i++)
                    obj[i] = deleted[i];
            }
            return obj;
        }
        

        public void Push(object element)
        {
            if (element == null)
                throw new NullElementException("Element to add to the stack cannot be null.");

            lock (this) //aquire the lock
            {
                if (IsFull()) //stack is full, signal get() thread to release the lock
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
                if(!IsEmpty() && !IsFull()) //not empty nor full
                {
                    top--; //reposition top position
                }


                insertedElementsCounter++; //up the counter of inserted element
                stack[top] = element; //push element at the top position
                count++; //up the counter of inserted element
                Console.WriteLine("Pushing: " + element); //write item 
                Monitor.Pulse(this); //tell procuder that consumer is done
            }
        }

        public object Pop()
        {
            object element = null; //element to return
            lock (this) //aquire the lock
            {
                if (IsEmpty()) //stack is empty
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
                element = stack[top]; //pop element out of the top position
                count--; //decrease the counter of element
                if (!IsEmpty() || top != capacity - 1) //if not empty or top position not in bottom index position
                    top++; //increase top position index in array

                Console.WriteLine("Popping: " + element); //write element
                Monitor.Pulse(this); //tell procuder that consumer is done
            }
            return element;
        }
    }
}
