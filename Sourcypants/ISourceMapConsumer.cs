namespace Blunder.SourceMap
{
    public interface ISourceMapConsumer
    {
        SourceReference[] OriginalPositionsFor(int line);
    }
}