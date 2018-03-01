namespace Blunder.SourceMap
{
    /// <summary>
    /// Represents a reference to a particular position in a specified source file.
    /// </summary>
    public class SourceReference
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string File { get; set; }
        
        /// <summary>
        /// Gets or sets the line number within the file.
        /// </summary>
        public int LineNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the column within the file
        /// </summary>
        public int Column { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", File, LineNumber);
        }
    }
}