/// Copyright (c) 2013 Ioannis Christodoulou
/// Author: Ioannis Christodoulou
/// 
/// Problem Statement :
/// 
/// As of late, your usually high-performance computer has been acting rather sluggish. /// 
/// While you have plenty of free disk space on your machine, it is split up over many hard drives.
/// We decide that the secret to improving performance is to consolidate all the data on your computer onto as few hard drives as possible.
/// Given a Integer used, representing the amount of disk space used on each drive, and a corresponding Integer total , representing the total capacity of 
/// each drive mentioned in used, we should attempt to pack the data onto as few hard drives as possible. 
/// You may assume that the data consists of very small files, such that splitting it up and moving parts of it onto different hard drives never presents a problem.
/// The minDrives methods returns the minimum number of hard drives that still contain data after the consolidation is complete.
/// 
/// Constraints:
/// 
/// -used will contain between 1 and 50 elements, inclusive.
/// -used and total will contain the same number of elements.
/// -Each element of used will be between 1 and 1000, inclusive.
/// -Each element of total will be between 1 and 1000, inclusive.
/// -used[i] will always be less than or equal to total[i] , for every valid index i.
/// 
/// Examples:
/// 
/// (ex. 1)
/// {300,525,110}
/// {350,600,115}
/// Returns: 2
/// In this example, the computer contains three hard drives:
/// Hard drive 1: 350 MB total, 300 MB used, 50 MB free
/// Hard drive 2: 600 MB total, 525 MB used, 75 MB free
/// Hard drive 3: 115 MB total, 110 MB used, 5 MB free
/// One way to pack the data onto as few drives as possible is as follows. 
/// First, move 50 MB from hard drive 3 to hard drive 1, completely filling it up. 
/// Next, move the remaining 60 MB from hard drive 3 to hard drive 2. 
/// There are still two hard drives which contain data after this process, so your method should return 2.
/// 
/// (ex 2)
/// {1,200,200,199,200,200}
/// {1000,200,200,200,200,200}
/// Returns: 1
/// One way to consolidate the data would be to move the 1 MB from hard drive 1 to hard drive 4. 
/// However, this is a poor choice, as it results in only one free hard drive and five hard drives which still contain data. 
/// A better decision is to move all the data from the other five hard drives onto hard drive 1.
/// Now there is only one hard drive which contains data. Since this is the optimal strategy, the method returns 1.
 
using MinDiskDrives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinDiskDrives
{
    /// <summary> Contains the methods to define the used and total sizes to split and move and finally consolidate as appropriate to
    /// return the minimum number of elements that still contain data after the consolidation is complete.
    /// Write all data-values being moved to a shifting log and reports while running in console mode to inform the user.
    /// </summary>
    public class MinimumDrives
    {
        private DataShiftingEntry entry { get; set; } //entry to write data to move, sending and receiving index positions
        private ShiftingLog consolidationLog { get; set; } //log of data relocation Entries

        public MinimumDrives()
        {
            consolidationLog = new ShiftingLog();
        }

        /// <summary> Return the minimum number of elements that still contain data after the consolidation is complete.
        /// </summary>
        /// <param name="used"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public int minDrives(ref int[] used, ref int[] total)
        {
            int minimum = 0; //minimum number to return
            Dictionary<int, int> uDict;
            Dictionary<int, int> tDict;
            Validate(used, total); //check for null value or different length
            ToDictionary(used, total, out uDict, out tDict); //copy to dictionary in order of dictionary key
            Pack(ref uDict, ref tDict, out minimum); //rearrange used values
            OrderToArray(ref used, ref total, uDict, tDict); //arrange in ascending order of key(i.e initial array index) and copy to out reference arrays
            return minimum; //return no of arrays still containing data
        }

        /// <summary> Counts minimum number of disk drives that still contain data
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private static int countMin(int[] u)
        {
            try
            {
                if (u.Length != 0 && u.Length > 0) // count minimum number of disk drives that still contain data
                {
                    int minimum = u.Length;
                    foreach (int usedData in u)
                    {
                        if (usedData == 0) //do not count free disk drive
                            minimum--;
                    }
                    return minimum;
                }
            }
            catch (NullReferenceException ex)
            {
                ArgumentException argex = new ArgumentException("Null array found", "u", ex);
                throw argex;

            }
            return 0;
        }

        /// <summary>
        /// Coctail sorts given reference used and total array elements
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        private static void cocktailSort(ref int[] u, ref int[] t)
        {
            try
            {
                for (int j = t.Length - 1; j > 0; j--) // iterate from top to bottom
                {
                    bool isSwapped = false;
                    for (int i = j; i > 0; i--)
                        if (t[i] > t[i - 1])
                        {
                            // swap if next item is greater
                            int tTemp = t[i];
                            int uTemp = u[i];
                            t[i] = t[i - 1];
                            t[i - 1] = tTemp;
                            u[i] = u[i - 1];
                            u[i - 1] = uTemp;
                            isSwapped = true;
                        }

                    for (int i = 0; i < j; i++)
                        if (t[i] < t[i + 1])
                        {
                            // swap if previous item is smaller
                            int tTemp = t[i];
                            int uTemp = u[i];
                            t[i] = t[i + 1];
                            t[i + 1] = tTemp;
                            u[i] = u[i + 1];
                            u[i + 1] = uTemp;
                            isSwapped = true;
                        }

                    if (!isSwapped)
                        break;
                }
            }
            catch (IndexOutOfRangeException ie)
            {
                ArgumentException argex = new ArgumentException("Index is out of range", ie);
                throw ie;
            }
        }

        /// <summary>
        /// Moves data in given total and used reference arrays and returns the integer value of the minimum drives that still contain data.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        /// <param name="minimum"></param>
        private static void MoveData(ref int[] u, ref int[] t, out int minimum)
        {

            minimum = 0; // counter of minimum drives still containing data
            int i, j = 0; // loop counters

            // indicator for ending the loop over the elements, that the search
            // has reached the end of the list, no more consolidation to do
            bool reachedEnd;

            // Start looping over the disk arrays to move data from smaller to larger size disks
            // and repeat while the search has not performed a complete first pass
            // over all of the elements, i.e. not reached the last element

            try
            {
                do
                {
                    reachedEnd = true; // assume first pass of the whole disk list is done at first

                    // loop over USED disk size elements
                    for (i = 0; i < u.Length - 1; i++)
                    {
                        // store current used, total, and free space values
                        int used = u[i];
                        int total = t[i];
                        int free = t[i] - u[i];

                        // reached the last disk, flag the first pass indicator and escape from loop
                        if (i == u.Length - 1)
                        {
                            reachedEnd = true;
                            break;
                        }
                        // not the last element at first pass, continue checking for free space to move data to
                        else if (i != u.Length - 1)
                        {
                            // disk is full, check next disk (flag indicator) 
                            if (used == total)
                                reachedEnd = true;

                            // disk has some space free
                            else if (used != total)
                            {
                                // loop over next USED disk size element(s) to check for available data to move to current element i
                                for (j = i + 1; j < t.Length; j++)
                                {
                                    // re-calculate used, total, and free space values
                                    used = u[i];
                                    total = t[i];
                                    free = t[i] - u[i];

                                    // available space is available to receive data
                                    if (free > 0)
                                    {
                                        // next used data is moved all to receiving disk: receiving disk has no free space left
                                        if (u[j] == free)
                                        {
                                            u[i] += u[j];   // receiving disk's new used size
                                            u[j] = 0;       // sending disk's new used size: 0
                                            free = 0;       // no more free space left to receive data

                                            // reached last inner element, escape from loop
                                            if (j == t.Length - 1)
                                                break;
                                        }
                                        // next used data is moved all to receiving disk: recalculate free space
                                        else if (u[j] < free)
                                        {
                                            u[i] += u[j];       // receiving disk's new used size
                                            u[j] = 0;           // sending disk's new used size: 0
                                            free = t[i] - u[i]; // receiving disk's new free space

                                            // reached last inner element, escape from loop
                                            if (j == t.Length - 1)
                                                break;
                                        }
                                        // next used data is moved at a percentage only to receiving disk: recalculate free space
                                        else if (u[j] > free)
                                        {
                                            u[i] += free;       // receiving disk's new used size
                                            u[j] -= free;       // sending disk's new used size
                                            free = t[i] - u[i]; //re-calculate free space of receiving disk

                                            // reached last inner element to move data from, 
                                            // flag for first pass complete and escape from loop
                                            if (j == t.Length - 1)
                                            {
                                                reachedEnd = true;
                                                break;
                                            }
                                            // receiving disk is full, escape from loop and go to next disk to move data to
                                            break;
                                        }

                                    }
                                }
                            }
                        }
                    }
                } while (!reachedEnd);

            }
            catch (IndexOutOfRangeException ie)
            {
                ArgumentException argex = new ArgumentException("Index is out of range", ie);
                throw ie;
            }

            // count minimum number of disk drives that still contain data
            minimum = countMin(u);

        }

        /// <summary>
        /// Copies to used and total dictionary structures used and total array elements.
        /// Returns as out reference variables moved total and used values.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        /// <param name="uDictionary"></param>
        /// <param name="tDictionary"></param>
        private static void ToDictionary(int[] u, int[] t, out Dictionary<int, int> uDictionary, out Dictionary<int, int> tDictionary)
        {
            //initialize temp arrays and storage dictionaries
            int[] _u = new int[u.Length];
            int[] _t = new int[t.Length];
            tDictionary = new Dictionary<int, int>();
            uDictionary = new Dictionary<int, int>();

            //copy array elements into temp arrays and dictionaries

            for (int i = 0; i < u.Length; i++)
            {
                uDictionary.Add(i, u[i]);
            }

            for (int i = 0; i < t.Length; i++)
            {
                tDictionary.Add(i, t[i]);
            }

            //check for corresponding length of arrays and dictionaries
            if (uDictionary.Keys.Count != u.Length)
                throw new ArgumentException("Invalid dictionary size of used elements found : " + uDictionary.Keys.Count + ". Expected : " + u.Length);
            if (tDictionary.Keys.Count != t.Length)
                throw new ArgumentException("Invalid dictionary size of total elements found : " + tDictionary.Keys.Count + ". Expected : " + t.Length);

        }

        /// <summary>
        /// Re-arranges total and used elements in given dictionaries and returns the counter of minimum drives still containing data
        /// </summary>
        /// <param name="uDict"></param>
        /// <param name="tDict"></param>
        /// <param name="minimum"></param>
        private void Pack(ref Dictionary<int, int> uDict, ref Dictionary<int, int> tDict, out int minimum)
        {
            minimum = 0; // counter of minimum drives still containing data
            int i, j = 0; // loop counters
            List<KeyValuePair<int, int>> tList = tDict.OrderByDescending(v => v.Value).ToList<KeyValuePair<int, int>>();
            HashSet<int> keysUsed = new HashSet<int>();          
            
            try
            {
                for (i = 0; i < tList.Count; i++)
                {
                    int i_key = tList[i].Key;
                    int used = uDict[i_key];
                    int total = tList[i].Value;
                    int free = total - used;

                    if (free > 0) // disk has some space free
                    {
                        keysUsed.Add(i_key); //store dictionary key currently processed                        
                        for (j = 0; j < uDict.Count; j++) //read through dictionary of used sizes to
                        {
                            int currentUsed = uDict.ElementAt(j).Value; //store sender's current used size
                            //arrange used data if current used size is not zero and not already utilized and/or compared against itself
                            if (currentUsed != 0 && uDict.ElementAt(j).Key != i_key && !keysUsed.Contains(uDict.ElementAt(j).Key))
                            {
                                if (currentUsed == free) //sender send all of its used data, no left free space for the receiver
                                {
                                    uDict[i_key] += currentUsed; //add used index of the same key
                                    uDict[uDict.ElementAt(j).Key] = 0; //set sending used size to zero
                                    free = 0; //disk is full
                                    entry = new DataShiftingEntry(uDict.ElementAt(j).Key, currentUsed, i_key); //create log entry
                                    consolidationLog.AddEntry(entry); //add entry to log
                                    break; //exit the loop
                                }
                                else if (currentUsed < free) //sender send all if its used data, receiver has still free space
                                {
                                    uDict[i_key] += currentUsed; //add used index of the same key
                                    uDict[uDict.ElementAt(j).Key] = 0; //set sending used size to zero
                                    free -= currentUsed; //disk is not full
                                    entry = new DataShiftingEntry(uDict.ElementAt(j).Key, currentUsed, i_key); //create log entry
                                    consolidationLog.AddEntry(entry); //add entry to log
                                }
                                else if (currentUsed > free) //receiver becomes full, while sender still has free space
                                {
                                    uDict[i_key] += free; //add used index of the same key
                                    uDict[uDict.ElementAt(j).Key] -= free; //calculate sender's new used size
                                    entry = new DataShiftingEntry(uDict.ElementAt(j).Key, free, i_key); //create log entry
                                    consolidationLog.AddEntry(entry); //add entry to log
                                    free = 0; //disk is full
                                    break; //exit the loop
                                }
                            }
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ie)
            {
                ArgumentException argex = new ArgumentException("Index is out of range", ie);
                throw ie;
            }

            int[] _used = new int[uDict.Count];
            foreach (KeyValuePair<int, int> pair in uDict)
            {
                _used[pair.Key] = pair.Value; //copy to array 
            }

            // count minimum number of disk drives that still contain data
            minimum = countMin(_used);
        }

        /// <summary>
        /// Arranges in ascending order of key(i.e initial array index) and copy to out reference arrays
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        /// <param name="uDictionary"></param>
        /// <param name="tDictionary"></param>
        private static void OrderToArray(ref int[] u, ref int[] t, Dictionary<int, int> uDictionary, Dictionary<int, int> tDictionary)
        {
            //Order dictionaries by key (ascending)
            foreach (KeyValuePair<int, int> size in uDictionary.OrderBy(key => key.Key))
            {
                u[size.Key] = size.Value; //store updated used value to initial index position
            }
            foreach (KeyValuePair<int, int> size in tDictionary.OrderBy(key => key.Key))
            {
                t[size.Key] = size.Value;//store updated total value to initial index position
            }
        }
        
        /// <summary>
        /// Validates given used and total value in arrays.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        private static void Validate(int[] u, int[] t)
        {
            if (u == null)
                throw new ArgumentException("Null array found", "u");
            else if (t == null)
                throw new ArgumentException("Null array found", "t");
            else if (u.Length != t.Length)
                throw new UnequalArrayLengthException("\n\nNot equal array lengths found. Used items array length: " + u.Length + ". Total items array length: " + t.Length + ".\n\n");

            Dictionary<int, int> invalidItems = new Dictionary<int, int>();
            for (int i = 0; i < t.Count(); i++)
            {
                if (t[i] < u[i])
                    invalidItems.Add(i, u[i]);
            }
            if (invalidItems.Count != 0)
            {
                string message = String.Empty;
                message += "Invalid disk size found: \n";
                foreach (KeyValuePair<int,int> item in invalidItems)
                {
                    message += "\nUsed disk size [" + item.Value + "] at index [" + item.Key + "] should be less than total disk size [" + t[item.Key] + "].";
                }
                message += "\n";
                throw new InvalidDiskSizeException(message);
            }
        }

        /// <summary>
        /// Prints to console data consolidation logged reporting element, original and new location index.
        /// </summary>
        public void ShowLog()
        {
            consolidationLog.PrintLog();
        }
    }
    
}
