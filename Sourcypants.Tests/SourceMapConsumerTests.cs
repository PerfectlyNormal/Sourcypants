using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sourcypants.Utils;
using Moq;
using NUnit.Framework;

namespace Sourcypants.Tests
{
    [TestFixture]
    public class SourceMapConsumerTests
    {
        private Mock<MappingDecoder> _decoderMock;
        private List<MappingGroup> _mappings = new List<MappingGroup>();

        [SetUp]
        public void Setup()
        {
            _mappings = new List<MappingGroup>
            {
                new MappingGroup
                {
                    Segments = new[]
                    {
                        new MappingSegment {GeneratedLineIndex = 0, SourceLineIndex = 1, SourcesIndex = 1},
                        new MappingSegment {GeneratedLineIndex = 0, SourceLineIndex = 10, SourcesIndex = 1}
                    }
                },
                new MappingGroup
                {
                    Segments = new[]
                    {
                        new MappingSegment {GeneratedLineIndex = 1, SourceLineIndex = 2, SourcesIndex = 1}
                    }
                }
            };

            _decoderMock = new Mock<MappingDecoder> {CallBase = true};
            _decoderMock
                .Setup(x => x.GetMappingGroups(It.IsAny<string>()))
                .Returns(_mappings);

            MappingDecoder.Default = _decoderMock.Object;
        }

        [Test]
        public void ThrowsArgumentNullException_IfNullSourceMapFileSupplied()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new SourceMapConsumer((SourceMapFile) null));
        }

        [Test]
        public void ThrowsArgumentException_IfVersionNotEqualToThree()
        {
            Assert.Throws(typeof(ArgumentException), () => new SourceMapConsumer(new SourceMapFile {Version = 4}));
        }

        [Test]
        public void ParsesMappingsUsingDefaultDecoder()
        {
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3, Mappings = "encoded mappings"});

            _decoderMock.Verify(x => x.GetMappingGroups("encoded mappings"), Times.Once());
        }

        [Test]
        public void OriginalPositionsFor_ReturnsPositionsMappingToGeneratedSourceLine()
        {
            // Line 1 maps to lines 2 and 11
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3, Sources = new[] {"File", "Other"}});
            var sourceLines = consumer.OriginalPositionsFor(1, 0);

            Assert.That(sourceLines, Is.Not.Null);
            Assert.That(sourceLines.Length, Is.EqualTo(2));

            // The segments store indices - add 1 to get line numbers
            Assert.That(sourceLines.Any(x => x.LineNumber == 2));
            Assert.That(sourceLines.Any(x => x.Column == 0));
            Assert.That(sourceLines.Any(x => x.LineNumber == 11));
            Assert.That(sourceLines.All(x => x.File == "Other"));
        }

        [Test]
        [TestCase(1,  1, "/the/root/one.js", 1,  1, null)]
        [TestCase(1,  5, "/the/root/one.js", 1,  5, null)]
        [TestCase(1,  9, "/the/root/one.js", 1, 11, null)]
        [TestCase(1, 18, "/the/root/one.js", 1, 21, "bar")]
        [TestCase(1, 21, "/the/root/one.js", 2,  3, null)]
        [TestCase(1, 28, "/the/root/one.js", 2, 10, "baz")]
        [TestCase(1, 32, "/the/root/one.js", 2, 14, "bar")]
        [TestCase(2,  1, "/the/root/two.js", 1,  1, null)]
        [TestCase(2,  5, "/the/root/two.js", 1,  5, null)]
        [TestCase(2,  9, "/the/root/two.js", 1, 11, null)]
        [TestCase(2, 18, "/the/root/two.js", 1, 21, "n")]
        [TestCase(2, 21, "/the/root/two.js", 2,  3, null)]
        [TestCase(2, 28, "/the/root/two.js", 2, 10, "n")]
        public void TestMappingTokensBackExactly(int sourceLine, int sourceCol, string file, int orgLine, int orgCol, string methodName)
        {
            // Force reset so we don't use the mocked decoder here
            MappingDecoder.Default = null;

            var consumer = new SourceMapConsumer(new SourceMapFile()
            {
                Version = 3,
                File = "min.js",
                Names = new [] { "bar", "baz", "n"},
                SourceRoot = "/the/root",
                Sources = new [] { "one.js", "two.js"},
                Mappings = "CAAC,IAAI,IAAM,SAAUA,GAClB,OAAOC,IAAID;CCDb,IAAI,IAAM,SAAUE,GAClB,OAAOA"
            });

            var firstMap = consumer.OriginalPositionsFor(sourceLine, sourceCol);
            Assert.That(firstMap, Is.Not.Null);
            Assert.That(firstMap.Length, Is.EqualTo(1), "Should have 1 source line");
            Assert.That(firstMap.All(x => x.LineNumber == orgLine), $"Original line number should be {orgLine}");
            Assert.That(firstMap.All(x => x.Column == orgCol), $"Original column should be {orgCol}");
            Assert.That(firstMap.All(x => x.File == file), $"Source file should be {file}");
            Assert.That(firstMap.All(x => x.MethodName == methodName), $"Original method name should be {methodName}");
        }

        [Test]
        [TestCase(1,  3, "/the/root/one.js", 1,  1, null)]
        [TestCase(1,  6, "/the/root/one.js", 1,  5, null)]
        [TestCase(1, 11, "/the/root/one.js", 1, 11, null)]
        [TestCase(1, 20, "/the/root/one.js", 1, 21, "bar")]
        [TestCase(1, 26, "/the/root/one.js", 2,  3, null)]
        [TestCase(1, 30, "/the/root/one.js", 2, 10, "baz")]
        [TestCase(1, 33, "/the/root/one.js", 2, 14, "bar")]
        [TestCase(2,  4, "/the/root/two.js", 1,  1, null)]
        [TestCase(2,  6, "/the/root/two.js", 1,  5, null)]
        [TestCase(2, 17, "/the/root/two.js", 1, 11, null)]
        [TestCase(2, 20, "/the/root/two.js", 1, 21, "n")]
        [TestCase(2, 27, "/the/root/two.js", 2,  3, null)]
        [TestCase(2, 28, "/the/root/two.js", 2, 10, "n")]
        [TestCase(2, 29, "/the/root/two.js", 2, 10, "n")]
        public void TestMappingTokensBackBiased(int sourceLine, int sourceCol, string file, int orgLine, int orgCol, string methodName)
        {
            // Force reset so we don't use the mocked decoder here
            MappingDecoder.Default = null;

            var consumer = new SourceMapConsumer(new SourceMapFile()
            {
                Version = 3,
                File = "min.js",
                Names = new [] { "bar", "baz", "n"},
                SourceRoot = "/the/root",
                Sources = new [] { "one.js", "two.js"},
                Mappings = "CAAC,IAAI,IAAM,SAAUA,GAClB,OAAOC,IAAID;CCDb,IAAI,IAAM,SAAUE,GAClB,OAAOA"
            });

            var firstMap = consumer.OriginalPositionsFor(sourceLine, sourceCol);
            Assert.That(firstMap, Is.Not.Null);
            Assert.That(firstMap.Length, Is.EqualTo(1), "Should have 1 source line");
            Assert.That(firstMap.All(x => x.LineNumber == orgLine), $"Original line number should be {orgLine}");
            Assert.That(firstMap.All(x => x.Column == orgCol), $"Original column should be {orgCol}");
            Assert.That(firstMap.All(x => x.File == file), $"Source file should be {file}");
            Assert.That(firstMap.All(x => x.MethodName == methodName), $"Original method name should be {methodName}");
        }

        [Test]
        public void OriginalPositionsFor_ReturnsEmptyArray_IfNoMatchingSourceLines()
        {
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});
            var sourceLines = consumer.OriginalPositionsFor(15, 0);

            Assert.That(sourceLines, Is.Not.Null);
            Assert.That(sourceLines.Length, Is.EqualTo(0));
        }

        [Test]
        public void OriginalPositionsFor_ReturnsEmptyArray_IfMatchingMappingGroupHasNullSegments()
        {
            _mappings[1].Segments = null;

            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});
            var sourceLines = consumer.OriginalPositionsFor(2, 0);

            Assert.That(sourceLines, Is.Not.Null);
            Assert.That(sourceLines.Length, Is.EqualTo(0));
        }

        [Test]
        public void OriginalPositionsFor_ReturnsEmptyArray_IfMatchingMappingGroupHasEmptySegments()
        {
            _mappings[1].Segments = Enumerable.Empty<MappingSegment>();

            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});
            var sourceLines = consumer.OriginalPositionsFor(2, 0);

            Assert.That(sourceLines, Is.Not.Null);
            Assert.That(sourceLines.Length, Is.EqualTo(0));
        }

        [Test]
        public void OriginalPositionsFor_ThrowsArgumentOutOfRangeException_IfLineNumberLessThanZero()
        {
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => consumer.OriginalPositionsFor(-2, 0));
        }

        [Test]
        public void OriginalPositionsFor_ThrowsArgumentOutOfRangeException_IfLineNumberEqualsZero()
        {
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => consumer.OriginalPositionsFor(0, 0));
        }

        [Test]
        public void OriginalPositionsFor_ThrowsArgumentOutOfRangeException_IfColumnLessThanZero()
        {
            var consumer = new SourceMapConsumer(new SourceMapFile {Version = 3});

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => consumer.OriginalPositionsFor(1, -1));
        }
    }
}
