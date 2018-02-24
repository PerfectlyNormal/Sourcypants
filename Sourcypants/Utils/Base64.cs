using System;
using System.Collections.Generic;

namespace Blunder.SourceMap.Utils
{
    public static class Base64
    {
        private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static readonly Dictionary<char, byte> DecodeTable = GetDecodeTable();

        private static Dictionary<char, byte> GetDecodeTable()
        {
            var table = new Dictionary<char, byte>();

            for (var idx = 0; idx < Base64Alphabet.Length; idx++)
            {
                table[Base64Alphabet[idx]] = (byte)idx;
            }

            return table;
        }

        public static byte Decode(char value)
        {
            if (!DecodeTable.TryGetValue(value, out var toReturn))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return toReturn;
        }

        public static char Encode(byte value)
        {
            if (value >= Base64Alphabet.Length)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return Base64Alphabet[value];
        }
    }
}