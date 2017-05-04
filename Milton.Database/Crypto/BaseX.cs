using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Crypto
{
    public class BaseX
    {
        private static string _defaultChars = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string Characters { get { return _defaultChars; } }

        private static Random _random = new Random();

        /// <summary>
        /// Default Constructor. Is by default base36
        /// </summary>
        public BaseX() { }

        /// <summary>
        /// Overload for constructor. Can passthrough character list to define what base it should be.
        /// </summary>
        /// <param name="charList">A String containing all the characters the class should use</param>
        /// <example>456789abcdefghijklmnopqrstuvw</example>
        public BaseX(string charList)
        {
            _defaultChars = charList;
        }

        /// <summary>
        /// Create a new instance of the BaseX class
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static BaseX New(String chars = null)
        {
            return chars == null ? new BaseX() : new BaseX(chars);
        }

        /// <summary>
        /// Encode the given number into a base of object in string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static String Encode(long input, String chars = null)
        {
            if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");

            BaseX bx = BaseX.New(chars);
            char[] clistarr = chars.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input % clistarr.Length]);
                input /= clistarr.Length;
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// Decode the base of class Encoded string into a number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Int64 Decode(string input)
        {
            var reversed = input.ToLower().Reverse();
            long result = 0;
            int pos = 0;
            foreach (char c in reversed)
            {
                result += _defaultChars.IndexOf(c) * (long)Math.Pow(_defaultChars.Length, pos);
                pos++;
            }
            return result;
        }

        public static String GenerateString(int length)
        {
            Int64 seed = (long)Math.Pow(_defaultChars.Length, length);

            //restricts the seed to a int32 as random can only take ints
            if (seed > Int32.MaxValue)
            {
                seed = Int32.MaxValue - 1;
            }

            int num = _random.Next(0, (int)seed);

            String refNumber = BaseX.Encode(num, _defaultChars);

            String firstChar = _defaultChars.Substring(0, 1);
            while (refNumber.Length < length)
            {
                refNumber = refNumber.Insert(0, firstChar);
            }

            return refNumber;
        }

    }
}
