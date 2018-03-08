using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sourcypants.Utils;
using Sourcypants.Utils.Comparers;
using Newtonsoft.Json;

namespace Sourcypants
{
    /// <summary>
    /// Reads and parses source map files and provides the ability to query mappings
    /// between generated source positions and their corresponding original source positions.
    /// </summary>
    public class SourceMapConsumer : ISourceMapConsumer
    {
        private readonly SourceMapFile _file;
        private readonly IList<MappingGroup> _mappingGroups;

        /// <inheritdoc />
        /// <summary>
        /// Initialises a new SourceMapConsumer using the contents of a source map file.
        /// </summary>
        /// <param name="sourceMapJson">The contents of a source map file, expressed as JSON.</param>
        public SourceMapConsumer(string sourceMapJson)
            : this(JsonConvert.DeserializeObject<SourceMapFile>(sourceMapJson))
        {
        }

        /// <summary>
        /// Initialises a new SourceMapConsumer using an already-deserialised SourceMapFile instance.
        /// </summary>
        /// <param name="file">The SourceMapFile instance to be queried.</param>
        public SourceMapConsumer(SourceMapFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (file.Version != 3)
            {
                throw new ArgumentException("Unsupported version: " + file.Version);
            }

            _file = file;
            _mappingGroups = MappingDecoder.Default.GetMappingGroups(_file.Mappings);
        }

        /// <summary>
        /// Determines the set of positions in original source files to which a given 1-based line number
        /// in the generated source file corresponds. If a line in the generated source has no corresponding
        /// original source line, an empty array is returned.
        /// </summary>
        /// <param name="line">The 1-based line number of the generated source for which the set of
        /// original source lines that contributed is required.</param>
        /// <param name="col">The 0-based line number of the generated source for which the set of
        /// original source lines that contributed is required.</param>
        /// <returns>An array of SourceReference objects representing the original source lines that correspond
        /// to the requested generated source line, or an empty array if no such lines exist.</returns>
        public SourceReference[] OriginalPositionsFor(int line, int col)
        {
            if (line <= 0)
                throw new ArgumentOutOfRangeException(nameof(line), "must be greater than zero");
            if (col < 0)
                throw new ArgumentOutOfRangeException(nameof(col), "must be greater than or equal to zero");

            if (line > _mappingGroups.Count)
                return new SourceReference[0];

            var generatedLine = _mappingGroups[line - 1];

            if (generatedLine.Segments == null || !generatedLine.Segments.Any())
            {
                return new SourceReference[0];
            }

            List<MappingSegment> hits;
            var segments = generatedLine.Segments
                .Where(x => x.SourceLineIndex.HasValue)
                .ToList();

            // Exact matches are the greatest
            if (segments.Any(s => s.GeneratedColumnIndex == col))
            {
                hits = segments.Where(s => s.GeneratedColumnIndex == col).ToList();
            }
            else
            {
                hits = new List<MappingSegment>
                {
                    segments
                        .LastOrDefault(x => x.GeneratedColumnIndex < col)
                };
            }

            return hits
                .Select(x => new SourceReference
                {
                    File = FileName(x.SourcesIndex),
                    LineNumber = (x.SourceLineIndex ?? 0) + 1,
                    Column = x.SourceColumnIndex ?? 0,
                    MethodName = x.NamesIndex.HasValue ? _file.Names[x.NamesIndex.Value] : null
                })
                .Distinct(new SourceReferenceEqualityComparer())
                .ToArray();
        }

        private string FileName(int? sourceIndex)
        {
            var filename = sourceIndex.HasValue ? _file.Sources[sourceIndex.Value] : _file.File;

            if (!string.IsNullOrEmpty(_file.SourceRoot))
                return string.Join("/", _file.SourceRoot, filename);

            return filename;
        }
    }
}
