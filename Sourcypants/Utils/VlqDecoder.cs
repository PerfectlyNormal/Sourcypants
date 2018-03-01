using System;

namespace Sourcypants.Utils
{
    public static class VlqDecoder
    {
        private const byte VlqBaseShift = 5;
        private const byte VlqBase = 1 << VlqBaseShift;
        private const byte VlqBaseMask = VlqBase - 1;
        private const byte VlqContinuationBit = VlqBase;

        public static int Decode(ref string encoded)
        {
            var result = 0;
            var encodedLength = encoded.Length;
            var encodedIdx = 0;
            var isContinued = true;
            var shift = 0;

            do
            {
                if (encodedIdx >= encodedLength)
                {
                    throw new ArgumentException("Too few characters in supplied encoded value");
                }

                var b = Base64.Decode(encoded[encodedIdx++]);
                isContinued = (b & VlqContinuationBit) == VlqContinuationBit;

                var digit = b & VlqBaseMask;
                result = result + (digit << shift);
                shift += VlqBaseShift;
            }
            while (isContinued);

            encoded = encoded.Substring(encodedIdx);
            return FromVlqSigned(result);
        }

        private static int FromVlqSigned(int signedValue)
        {
            var isNegative = ((signedValue & 0x1) == 0x1);
            var shifted = signedValue >> 1;

            return isNegative ? -shifted : shifted;
        }
    }
}
