namespace Sourcypants
{
    public interface ISourceMapConsumer
    {
        SourceReference[] OriginalPositionsFor(int line, int col);
    }
}
