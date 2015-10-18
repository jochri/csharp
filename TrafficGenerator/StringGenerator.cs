/// Author: Ioannis Christodoulou
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStringGenerator
{
    /// <summary>
    /// Provides the methods to generate string and characters randomly using C# Random class methods.
    /// It generates string names using fixed given length size.
    /// </summary>
   
   public class StringGenerator
   {      
      private int _Length { get; set; }
      private List<string> _Existing { get; set; }

      //constructor
      public StringGenerator(int length)
      {
         _Existing = new List<string>();
         _Length = length;
      }

      public StringGenerator()
      {
         _Existing = new List<string>();         
      }

      public List<string> NextStrings(int len)
      {         
         return GenerateStrings(len);         
      }
      
      public int Length
      {
         get { return _Length; }
         set { _Length = value; }
      }    
      
      private List<string> GenerateStrings(int length)
      {
         char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
         char[] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
         char[] numbers = "0123456789".ToCharArray();
         char[] special = "`~!@#$%^&()-_=+[]{};',.".ToCharArray();
         Dictionary<int,char> dictionary = ToDictionary(chars); //copy to collection of key/character pair

         List<string> list = new List<string>();    
         var counter = 0;
         do
         {
            var str = String.Empty;
            bool exists = false; //check if it exists
            do
            {
               str = Generate(8);
               if (list != null)
                  exists = Exists(str, list);
             
            }
            while (str != String.Empty && exists );
            list.Add(str);
            counter++;
         }
         while (counter < length);
         return list;
      }


      private static string Generate(int length)
      {
         char[] capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
         char[] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
         char[] numbers = "0123456789".ToCharArray();
         char[] special = "`~!@#$%^&()-_=+[]{};',".ToCharArray();
         char[] allCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789`~!@#$%^&()-_=+[]{};',".ToCharArray();
                   
         List<char> Characters = new List<char>();
         var str = string.Empty;

         if (length.Equals(0))
            throw new ArgumentException("Length cannot be null");
         
         int minCapitals = 4;
         int minLowerCase = 2;
         int minNumbers = 1;
         int minSpecial = 1;
         
         // adding chars to an array         
         for (int i = 0; i < minCapitals; i++)
            Characters.Add(GetRandomCharFromArray(capitals, Characters));
         for (int i = 0; i < minLowerCase; i++)
            Characters.Add(GetRandomCharFromArray(lowerCase, Characters));
         for (int i = 0; i < minNumbers; i++)
            Characters.Add(GetRandomCharFromArray(numbers, Characters));
         for (int i = 0; i < minSpecial; i++)
            Characters.Add(GetRandomCharFromArray(special, Characters));

         var random = new Random();
         for (int i = 0; i < length; i++)
         {
            int position = random.Next(0, 500) % Characters.Count; 
            char currentChar = Characters[position];
            str += currentChar.ToString();                
         }

         return str;
      }


      private static char GetRandomCharFromArray(char[] array, List<char> existing)
      {
         char Char = ' ';
         var random = new Random();
         do
         {
            Char = array[random.Next(0,500) % array.Length];
         } while (existing.Contains(Char));
         return Char;
      }

      private static string Generator(int length, Dictionary<int, char> dict)
      {
         if (length.Equals(0))
            throw new ArgumentException("Length cannot be null");
         var count = dict.Count();
         var random = new Random();
         var chars = new char[length];
         for (int i = 0; i < length; i++)
         {
            int keyPos = random.Next(0, count - 1); //random key
            chars[i] = dict[keyPos]; //get value from dictionary, i.e. a signle character           
         }

         var str = string.Empty;
         foreach (char c in chars)
         {
            str += c;
         }
         return str;
      }

      private static bool Exists(string name, List<string> list)
      {
         var exists = false;
         if (list == null)
            throw new ArgumentException("Cannot access list because it was null", "list");
         if (list.Contains(name))
            exists = true;
         return exists;
      }

      private static Dictionary<int, char> ToDictionary(char[] characters)
      {
         if (characters == null)
            throw new ArgumentException("Array of characters cannot be null");

         var dictionary = new Dictionary<int, char>();
         var key = 0;
         var count = characters.Count();
         foreach (char c in characters)
         {
            dictionary.Add(key, c); //add to collection
            if (!key.Equals(count - 1))
               key++;
         }
         return dictionary;
      }
   }
}
