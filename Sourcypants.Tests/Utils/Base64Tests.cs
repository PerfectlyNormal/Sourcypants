using System;
using System.Linq;
using Blunder.SourceMap.Utils;
using NUnit.Framework;

namespace Blunder.SourceMap.Tests.Utils
{
    [TestFixture]
    public class Base64Tests
    {
        [Test]
        public void Decode_ThrowsArgumentOutOfRangeException_IfCharBelowBase64Range()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => Base64.Decode(char.MinValue));
        }

        [Test]
        public void Decode_ThrowsArgumentOutOfRangeException_IfCharAboveBase64Range()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => Base64.Decode(char.MaxValue));
        }

        [Test]
        public void Decode_ReturnsCorrectDecodes()
        {
            var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToArray();
            
            for (var i = 0; i < base64Chars.Length; i++)
            {
                Assert.That(Base64.Decode(base64Chars[i]), Is.EqualTo(i));
            }
        }

        [Test]
        public void Encode_ThrowsArgumentOutOfRangeException_IfValueGreaterThanOrEqualTo64()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => Base64.Encode(64));
        }

        [Test]
        public void Encode_ReturnsCorrectlyEncodedValues()
        {
            var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToArray();

            for (var i = 0; i < base64Chars.Length; i++)
            {
                Assert.That(Base64.Encode((byte) i), Is.EqualTo(base64Chars[i]));
            }
        }
    }
}