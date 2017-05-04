using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database
{
    public static class RandomExtensions
    {
        public static UInt64 NextUInt64(this Random rnd)
        {
            var buffer = new byte[sizeof(UInt64)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static String GetRandomString(int length = 32, Boolean humanReadable = false)
        {
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            if (humanReadable) chars = "ABCDFGHJKMNPQRTUVWXYZ246789";

            String randomStr = String.Empty;
            Random rand = new Random();

            for (int i = 0; i < length; i++)
                randomStr += chars[rand.Next(0, chars.Length)];

            return randomStr;
        }
    }
}
