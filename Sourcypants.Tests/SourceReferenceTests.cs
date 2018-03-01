using NUnit.Framework;

namespace Sourcypants.Tests
{
    [TestFixture]
    public class SourceReferenceTests
    {
        [Test]
        public void ToString_IncludesFilenameAndLineNumber()
        {
            var reference = new SourceReference
            {
                File = "The file",
                LineNumber = 44,
                Column = 1
            };

            Assert.That(reference.ToString(), Is.EqualTo("The file:44:1"));
        }

        [Test]
        public void ToString_IncludesMethodName()
        {
            var reference = new SourceReference
            {
                File = "The file",
                LineNumber = 44,
                Column = 1,
                MethodName = "dummy"
            };

            Assert.That(reference.ToString(), Is.EqualTo("The file:44:1#dummy"));
        }
    }
}
