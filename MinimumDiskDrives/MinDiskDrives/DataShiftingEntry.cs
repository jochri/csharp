// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinDiskDrives
{
    /// <summary>
    /// Represents the entry of the shifted data containing the source-original index
    /// the size to be moved to a new index, and the target index-new index.
    /// </summary>
    internal class DataShiftingEntry
    {
        private int sourceIndex { get; set; } //index of data to be moved
        private int sizeToMove { get; set; } //data to be moved
        private int targetIndex { get; set; } //index of data to receive new data

        public DataShiftingEntry(int sourceIndex, int sizeToMove, int targetIndex)
        {
            this.sourceIndex = sourceIndex;
            this.sizeToMove = sizeToMove;
            this.targetIndex = targetIndex;
        }

        public int SourceIndex()
        {
            return sourceIndex;
        }

        public int TargetIndex()
        {
            return targetIndex;
        }

        public int Size()
        {
            return sizeToMove;
        }

    }
}
