/// Author: Ioannis Christodoulou
/// Description:
/// Using StringGenerator it generates bytes and writes to randomly named files to path being run in.
/// It uses intervals to produce and write byte to created files.
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using RandomStringGenerator;

namespace FileRandomizer
{
    public class FileGenerator
    {
        public static void Main(string[] args)
        {
            Run();
        }

        public static bool GenerateBytes(int noOfFiles, int maxFileSize, out bool result, int sleep)
        {
           if (maxFileSize <= 0) //invalid max file size given
              throw new ArgumentException("File size should be greater that zero.", maxFileSize.ToString());

           byte[] firstBuffer; //first chunk of bytes
           byte[] secondBuffer; //second chunk of bytes
           var random = new Random(); //number generator                       
           StringGenerator generator = new StringGenerator();
           var fileNames = generator.NextStrings(noOfFiles); //generate random strings
           var interval = new TimeSpan(0, 0, 0, 0, sleep); //create seconds interval
           const int bytes = 1024; //bytes per KB

           try
           {
              for (var i = 0; i < noOfFiles; i++) //iterate over array elements
              {
                 try
                 {
                    var fi = new FileInfo(fileNames[i]); //create fileinfo object for the files in the array
                    var firstMaxBytesCount = random.Next(1, random.Next(1, (maxFileSize * bytes) / 2)); //random first count of bytes for read-write operation
                    var secondMaxBytesCount = (maxFileSize * bytes) - firstMaxBytesCount; //calculate remaining byte count for second read-write operation
                    var remaining = 0; //remaining length in bytes
                    var total = firstMaxBytesCount + secondMaxBytesCount; //total length in bytes
                    var length = 0; //length in bytes
                    var maxCount = 0; //max available count of bytes to write with this stream
                    var randomCount = 0; //random bytes count to write
                    firstBuffer = new byte[firstMaxBytesCount]; //init first buffer

                    using (var stream = fi.OpenWrite()) //create write only file stream
                    {
                       length = Convert.ToInt16(fi.Length);
                       maxCount = firstMaxBytesCount;
                       while (length < firstMaxBytesCount) //do stuff until file reaches first count of writable bytes
                       {
                          random.NextBytes(firstBuffer); //fill array of bytes with random numbers
                          randomCount = random.Next(1, maxCount); //generate random count of bytes                             
                          stream.Write(firstBuffer, 0, randomCount); //write bytes to the stream                            
                          length = Convert.ToInt16(stream.Length); //get new byte length                                    
                          maxCount = firstMaxBytesCount - length; //calculate maximum count of bytes

                          if (!length.Equals(0))
                             Console.WriteLine("Writing {0} byte(s) to file {1}.file", randomCount, fileNames[i]);
                       }

                       remaining = total - length; //current remaining length in bytes
                    }

                    secondBuffer = new byte[secondMaxBytesCount]; //init second buffer
                    length = 0; //reset length for the second chunk

                    if (remaining == secondMaxBytesCount) //check that remaining fits into the next chunk
                    {
                       using (var fs = fi.Open(FileMode.Open, FileAccess.ReadWrite)) //open file stream to write new data
                       {
                          try
                          {
                             var fileLength = fs.Length; //current file length
                             var canSeek = fs.CanSeek;// seeking capability
                             var offSet = random.Next(0, Convert.ToInt16(fileLength)); //the point to write relative to the end of the first chunk written
                             var maxWritable = Convert.ToInt16(fileLength) - offSet; //max byte length to write to the stream with the seek operation

                             if (canSeek) //seek random position and update with random bytes
                             {
                                fs.Seek(offSet, SeekOrigin.Begin); //set current stream position
                                var randSize = random.Next(0, maxWritable); //generate random size
                                byte[] temp = new byte[randSize]; //init temp buffer
                                random.NextBytes(temp); //fill with bytes
                                fs.Write(temp, 0, randSize); //write the bytes
                                Console.WriteLine("Updating with {0} byte(s) file {1}.file", randSize, fileNames[i]);
                             }

                             //sleep before writing next chunk of bytes
                             if (!sleep.Equals(0))
                             {
                                Console.WriteLine("\nSleep for {0} milliseconds...", sleep);
                                Thread.Sleep(interval); //delay
                             }

                             maxCount = secondMaxBytesCount; //max available count of bytes to write with the second stream
                             while (length < secondMaxBytesCount)
                             {
                                random.NextBytes(secondBuffer);
                                randomCount = random.Next(1, maxCount);
                                fs.Write(secondBuffer, 0, randomCount);
                                length += randomCount;
                                maxCount = secondMaxBytesCount - length;

                                if (!length.Equals(0))
                                   Console.WriteLine("Writing {0} byte(s) to file {1}.file", randomCount, fileNames[i]);
                             }
                          }
                          catch (ArgumentException ae)
                          {
                             throw new ArgumentException("Seeking is attempted before the beginning of the stream.", ae.InnerException);
                          }
                          catch (FileNotFoundException fe)
                          {
                             throw new ArgumentException("File was not found: " + fileNames[i], fe.InnerException);
                          }
                       }
                    }
                 }
                 catch (ArgumentNullException ane)
                 {
                    throw new ArgumentException("File name was null : " + fileNames[i], ane.InnerException);
                 }
                 catch (ArgumentException ae)
                 {
                    throw new ArgumentException(ae.Message, ae.InnerException);
                 }
              }
           }
           catch (IndexOutOfRangeException ie)
           {
              throw new ArgumentException("Array index was out of range", ie.InnerException);
           }

           result = true;
           return result;
        }

        public static string[] RandomFileNames(int length)
        {
            var random = new Random();
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            var names = new string[length];
            
            for (int i = 0; i < length; i++)
            {
                var stringBuilder = new StringBuilder();
                var _ch = random.Next(0, 1600).ToString();
                var _char = random.Next(1601, 6000).ToString();
                stringBuilder.Append(_char).Append(_ch).Append(".file");
                names[i] = stringBuilder.ToString();
            }

            return names;
        }

        public static void Run()
        {

            try
            {
                bool keepRunning; // loop flag to indicate to exit or re-run the program
                do
                {
                    keepRunning = false; //running for the first time
                    Console.WriteLine("Files Generator starting...\n");

                    /* get number of files */
                    Console.WriteLine("Enter number of random files generated:");
                    var numberOfFiles = Console.ReadLine(); //red number of file
                    var numberPattern = @"^[1-9]{1}[0-9]*$"; //use regular expression pattern
                    Regex regex = new Regex(numberPattern);
                    var isValid = regex.IsMatch(numberOfFiles); //try to match
                    while (!isValid) //invalid
                    {
                        Console.WriteLine("Invalid number entered. Re-enter number of random files generated:");
                        numberOfFiles = Console.ReadLine();
                        isValid = regex.IsMatch(numberOfFiles);
                    }

                    var files = Convert.ToInt32(numberOfFiles);

                    /* get maximum file size */
                    Console.WriteLine("Enter maximum file size in KBs:");
                    var size = Console.ReadLine(); //read size
                    var sizePattern = @"^[1-9]{1}[0-9]*$"; //use regular expression pattern
                    regex = new Regex(sizePattern);
                    isValid = regex.IsMatch(size); //try to match
                    while (!isValid)
                    {
                        Console.WriteLine("Invalid file size entered. Re-enter max file size:");
                        size = Console.ReadLine();
                        isValid = regex.IsMatch(size);
                    }
                    var fileSize = Convert.ToInt32(size);


                    /* get interval in seconds */
                    Console.WriteLine("Enter delay in milliseconds: ");
                    var delay = Console.ReadLine(); //read delay
                    var delayPattern = @"^[0-9]{1}[0-9]*$"; //use regular expression pattern
                    regex = new Regex(delayPattern);
                    isValid = regex.IsMatch(delay); //try to match               
                    while (!isValid)
                    {
                        Console.WriteLine("Invalid delay entered. Re-enter delay in seconds:");
                        delay = Console.ReadLine();
                        isValid = regex.IsMatch(delay); //try to match   
                    }
                    var interval = Convert.ToInt32(delay); //finally convert

                    var result = false;
                    GenerateBytes(files, fileSize, out result, interval); //generate bytes and files

                    if (result == true)
                        Console.WriteLine("\nDone."); //complete

                    Console.WriteLine("Press R to re-run or E to exit the program: ");
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
                Console.WriteLine("\n\n" + e.Message);
            }
        }
    }
}
