using System.Collections.Generic;
using System.Linq;

namespace Sourcypants.Utils
{
    public class MappingDecoder
    {
        private static MappingDecoder _defaultDecoder;
        public static MappingDecoder Default
        {
            get => _defaultDecoder ?? (_defaultDecoder = new MappingDecoder());
            set => _defaultDecoder = value;
        }

        public virtual IList<MappingGroup> GetMappingGroups(string encoded)
        {
            var groupsRaw = encoded.Split(';');
            var toReturn = groupsRaw.Select((t, i) => GetMappingGroup(i, t)).ToList();

            FixUpGroupSegmentOffsets(toReturn);

            return toReturn;
        }

        public virtual MappingGroup GetMappingGroup(int generatedLineIndex, string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                return new MappingGroup();

            return new MappingGroup
            {
                Segments = GetMappingSegments(generatedLineIndex, encoded)
            };
        }

        public virtual IList<MappingSegment> GetMappingSegments(int generatedLineIndex, string encoded)
        {
            return
                encoded
                .Split(',')
                .Select(x => GetMappingSegment(generatedLineIndex, x))
                .ToArray();
        }

        public virtual MappingSegment GetMappingSegment(int generatedLineIndex, string encoded)
        {
            return new MappingSegment(generatedLineIndex, encoded);
        }

        public static void FixUpGroupSegmentOffsets(IEnumerable<MappingGroup> groups)
        {
            int? lastSourcesIndex = null;
            int? lastSourceLineIndex = null;
            int? lastSourceColumnIndex = null;
            int? lastNamesIndex = null;

            foreach (var group in groups.Where(x => x.Segments != null))
            {
                var lastGeneratedColumnIndex = 0;
                foreach (var segment in group.Segments)
                {
                    lastGeneratedColumnIndex = segment.GeneratedColumnIndex = segment.GeneratedColumnIndex + lastGeneratedColumnIndex;

                    if (segment.SourcesIndex.HasValue && lastSourcesIndex.HasValue)
                    {
                        lastSourcesIndex = segment.SourcesIndex = segment.SourcesIndex + lastSourcesIndex;
                    }
                    else if (segment.SourcesIndex.HasValue)
                    {
                        lastSourcesIndex = segment.SourcesIndex;
                    }

                    if (segment.SourceLineIndex.HasValue && lastSourceLineIndex.HasValue)
                    {
                        lastSourceLineIndex = segment.SourceLineIndex = segment.SourceLineIndex + lastSourceLineIndex;
                    }
                    else if (segment.SourceLineIndex.HasValue)
                    {
                        lastSourceLineIndex = segment.SourceLineIndex;
                    }

                    if (segment.SourceColumnIndex.HasValue && lastSourceColumnIndex.HasValue)
                    {
                        lastSourceColumnIndex = segment.SourceColumnIndex = segment.SourceColumnIndex + lastSourceColumnIndex;
                    }
                    else if (segment.SourceColumnIndex.HasValue)
                    {
                        lastSourceColumnIndex = segment.SourceColumnIndex;
                    }

                    if (segment.NamesIndex.HasValue && lastNamesIndex.HasValue)
                    {
                        lastNamesIndex = segment.NamesIndex = segment.NamesIndex + lastNamesIndex;
                    }
                    else if (segment.NamesIndex.HasValue)
                    {
                        lastNamesIndex = segment.NamesIndex;
                    }
                }
            }
        }
    }
}
