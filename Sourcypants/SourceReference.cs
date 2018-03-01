namespace Sourcypants
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

        /// <summary>
        /// Gets or sets the original method name
        /// </summary>
        public string MethodName { get; set; }

        public override string ToString()
        {
            var result = string.Format("{0}:{1}:{2}", File, LineNumber, Column);

            if (!string.IsNullOrEmpty(MethodName))
                result += $"#{MethodName}";

            return result;
        }
    }
}
