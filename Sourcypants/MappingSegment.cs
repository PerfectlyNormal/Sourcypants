﻿using Blunder.SourceMap.Utils;

namespace Blunder.SourceMap
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
            this.GeneratedLineIndex = generatedLineIndex;
            this.GeneratedColumnIndex = VlqDecoder.Decode(ref encodedFields);
            this.SourcesIndex = GetNextValue(ref encodedFields);
            this.SourceLineIndex = GetNextValue(ref encodedFields);
            this.SourceColumnIndex = GetNextValue(ref encodedFields);
            this.NamesIndex = GetNextValue(ref encodedFields);
        }

        private static int? GetNextValue(ref string encoded)
        {
            return encoded.Length == 0 ? null : (int?) VlqDecoder.Decode(ref encoded);
        }
    }
}