using System;
using System.Text;

namespace Entoarox.Framework.Extensions
{
    public static class StringExtensions
    {
        /*********
        ** Public methods
        *********/
        public static string Base64Encode(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string Base64Decode(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
