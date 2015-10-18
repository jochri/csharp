/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinDiskDrives
{
    /// <summary>
    /// Represents a shifting log containing the data shifted data entries and their original and new index after consolidation is performed.
    /// </summary>
    internal class ShiftingLog
    {
        public List<DataShiftingEntry> Entries { get; private set; } //collection of data shifting log entries
        public int Size { get { return Entries.Count; } } //returns the size of the log

        public ShiftingLog()
        {
            Entries = new List<DataShiftingEntry>(); //init Entries list
        }

        public void AddEntry(DataShiftingEntry entry)
        {
            if (entry == null)
                throw new ArgumentException("Null entry found", entry.ToString());

            Entries.Add(entry); //add entry to list
        }

        /// <summary>
        /// Prints to console the shifting log of entries.
        /// </summary>
        public void PrintLog()
        {
            if(Entries != null && !Entries.Count.Equals(0))
            {
                Console.WriteLine("\n\nData consolidation log:\n");
                foreach(DataShiftingEntry entry in Entries)
                {                       
                    Console.WriteLine("Moved {0} from index [{1}] to index [{2}]", entry.Size(), entry.SourceIndex(), entry.TargetIndex());
                }
            }

        }
    }
}
