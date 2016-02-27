using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    static class Utils
    {
        /// <summary>
        /// Decodes a Base64 string into a the original Guid.
        /// </summary>
        /// <param name="token">a base64 string encoded Guid</param>
        /// <returns>The guid encoded in the token</returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DecoderFallbackException"></exception>
        public static string Base64Decode(this string token)
        {
            string decodedString = "";
            if (!string.IsNullOrWhiteSpace(token))
            {
                var inputBytes = Convert.FromBase64String(token);
                decodedString = Encoding.ASCII.GetString(inputBytes);
           
            }
            return decodedString;
        }
    }
}
