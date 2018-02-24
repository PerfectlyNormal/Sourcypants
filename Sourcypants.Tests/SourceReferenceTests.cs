using NUnit.Framework;

namespace Blunder.SourceMap.Tests
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
                LineNumber = 44
            };

            Assert.That(reference.ToString(), Is.EqualTo("The file:44"));
        }
    }
}