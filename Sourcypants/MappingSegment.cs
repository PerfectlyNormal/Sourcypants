using Sourcypants.Utils;

namespace Sourcypants
{
    public class MappingSegment
    {
        public int GeneratedLineIndex { get; set; }
        public int GeneratedColumnIndex { get; set; }
        public int? SourcesIndex { get; set; }
        public int? SourceLineIndex { get; set; }
        public int? SourceColumnIndex { get; set; }
        public int? NamesIndex { get; set; }

        public MappingSegment()
        {

        }

        public MappingSegment(int generatedLineIndex, string encodedFields)
        {
            GeneratedLineIndex = generatedLineIndex;
            GeneratedColumnIndex = VlqDecoder.Decode(ref encodedFields);
            SourcesIndex = GetNextValue(ref encodedFields);
            SourceLineIndex = GetNextValue(ref encodedFields);
            SourceColumnIndex = GetNextValue(ref encodedFields);
            NamesIndex = GetNextValue(ref encodedFields);
        }

        private static int? GetNextValue(ref string encoded)
        {
            return encoded.Length == 0 ? null : (int?) VlqDecoder.Decode(ref encoded);
        }
    }
}
