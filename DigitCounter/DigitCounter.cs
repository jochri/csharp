/// Author: Ioannis Christodoulou
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountDigitOneDesktop
{
    /// <summary> Given an integer n, counts the total number of digit one(1) appearing in all non-negative integers less than or equal to n
    /// </summary>
    static class DigitCounter
    {
        /// <summary> Counts total number of digit one in the given range of all non-negative integers less than or equal to n.
        /// </summary>
        /// <param name="range">the range of non-negative integer</param>
        /// <returns></returns>
        public static int count(int range)
        {
            var limit = range;
            var counter = 0;
            for (var iterator = 1; iterator <= limit; iterator++)
            {
                var num = iterator;
                while (num != 0)
                {
                    var remDigit = num % 10; // check the remainder digit for match
                    num = num / 10;
                    if (remDigit == 1) // do the comparison with the match digit
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
    }
}
