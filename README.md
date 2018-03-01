# SourcyPants

(Not to be confused with [saucy pants][saucypants])

Simple source map parser for .NET Standard.
Based on [SourceMapDotNet][SMDN], which titles itself as a "port of a small
portion of Mozilla's source-map module to .NET intended for reading a source
map".

This project goes a bit further, and adds support for asking "what line and
column number in the original source does line X and column Y in the generated
source map to?" instead of just what line.

It also handles a bit more for display purposes, such as supporting `SourceRoot`
and method names.

## Usage

Build a SourceMapConsumer by either supplying the JSON contents of a source map
file:

```c#
var consumer = new Sourcypants.SourceMapConsumer("{ source: 'map', goes: 'here'
}");
```

Or build directly from a `SourceMapFile` instance, built using your JSON decoder
of choice:

```c#
var file = JsonConvert.Deserialize<Sourcypants.SourceMapFile>("{ source: 'map',
goes: 'here' }");
var consumer = new Sourcypants.SourceMapConsumer(file);
```

Then find out which original source lines map to a given generated source line
number (line numbers are 1-based):

```c#
// Get the original source lines that map to line 12, column 0 of the generated
source
var matches = consumer.OriginalPositionsFor(12, 0);

foreach (var match in matches)
{
    var line = match.LineNumber;
    var filename = match.File;
    
    // Do useful things...
}
```

An empty array is returned if there are no matching original source lines for
the specified generated source line.

## License

[MIT licensed](LICENSE.md)

[saucypants]: https://www.urbandictionary.com/define.php?term=saucy%20pants
[SMDN]: https://github.com/Pablissimo/SourceMapDotNet
