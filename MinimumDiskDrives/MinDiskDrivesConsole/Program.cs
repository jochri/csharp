// Author: Ioannis Christodoulou
/// Description:
/// Console program to demonstrate the use of the MinDrives program to 
/// simulate data consolidation between hard drives of different sizes 
/// to move and pack all the data onto as few hard drives as possible.
/// 
using MinDiskDrives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunMinDiskDrives
{
    /// <summary>
    /// Runs the MinDrives consolidation program with defines total and used element values.
    /// Feel free to change array values to check the programs validity and correctness.
    /// </summary>
    public class Program
    {

        /* Prints to console screen the integer elements */
        public static void printNumberSequence(int[] numbers)
        {
            int len = numbers.Length;
            Console.Write("{ ");
            for (int i = 0; i < numbers.Length; i++)
            {
                if (i == len - 1)
                    Console.Write(numbers[i] + " }");
                else
                    Console.Write(numbers[i] + ", ");
            }
        }

        static void Main(string[] args)
        {

            int[] used1 = new int[10] { 331, 242, 384, 366, 428, 211, 145, 89, 581, 170};
            int[] total1 = new int[10] { 502, 249, 800, 900, 770, 573, 771, 565, 693, 714 };
            int min = 0;    //minimum number of disk drives contanining data    

            try
            {
                bool keepRunning; // loop flag to indicate to exit or re-run the program
                do
                {
                    keepRunning = false; // program is running for the first time, assume false after 1st loop

                    Console.WriteLine("\nData Consolidation Program - Starting...\n"); // Start of program 
                    // Print numbers
                    Console.WriteLine("\n\nUsed disk sizes entered: ");
                    printNumberSequence(used1);
                    Console.WriteLine("\n\nTotal disk sizes entered: ");
                    printNumberSequence(total1);

                    MinimumDrives mds = new MinimumDrives();
                    min = mds.minDrives(ref used1, ref total1); // return minimum disk drives still containing data
                    mds.ShowLog(); //print consolidation log

                    // Print numbers
                    Console.WriteLine("\n\nRe-arranged used disk sizes: ");
                    printNumberSequence(used1);
                    Console.WriteLine("\n\nTotal disk sizes: ");
                    printNumberSequence(total1);

                    Console.WriteLine();
                    Console.WriteLine("\nReturns: " + min + "\n");

                    Console.WriteLine("\nData Consolidation Program - Ending...\n"); // End of program                
                    Console.WriteLine();
                    Console.WriteLine("\nPress e to exit the program or r to re-run it :");
                    ConsoleKeyInfo keyinfo = Console.ReadKey(true); // select exit or re-run the program

                    if (keyinfo.KeyChar == 'e' || keyinfo.KeyChar == 'E')
                    {
                        keepRunning = false;
                    }
                    else if (keyinfo.KeyChar == 'r' || keyinfo.KeyChar == 'R')
                    {
                        keepRunning = true;
                    }
                    Console.WriteLine("\nYou pressed : " + keyinfo.KeyChar + "\n");

                } while (keepRunning);

            }

            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
