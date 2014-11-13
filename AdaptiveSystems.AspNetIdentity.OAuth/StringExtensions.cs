using System.Collections.Generic;
using NExtensions;

namespace AdaptiveSystems.AspNetIdentity.OAuth
{
    public static class StringExtensions
    {
        public static string HashEncodedForRowKey(this string input)
        {
            var options = new HashOptions {CharsToReplace = new List<KeyValuePair<char, char>>
            {
                new KeyValuePair<char, char>('/', '_'),
                new KeyValuePair<char, char>('\\', '-'),
                new KeyValuePair<char, char>('#', '|'),
                new KeyValuePair<char, char>('?', '~')
            }};
            return input.Hash(options);
        }
    }
}