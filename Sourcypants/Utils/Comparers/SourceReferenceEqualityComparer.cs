using System.Collections.Generic;

namespace Blunder.SourceMap.Utils.Comparers
{
    public class SourceReferenceEqualityComparer : IEqualityComparer<SourceReference>
    {
        public bool Equals(SourceReference x, SourceReference y)
        {
            return x.LineNumber == y.LineNumber && x.File == y.File;
        }

        public int GetHashCode(SourceReference obj)
        {
            return (obj.LineNumber.GetHashCode() * 397) ^ obj.File.GetHashCode();
        }
    }
}