using System.IO;

namespace Client
{
    public class CSVWriter
    {
        private readonly bool checkDelimForQuote;
        private string doubleQuoteString = "\"\"";

        private readonly char[] quoteRequiredChars;
        private string quoteString = "\"";

        private int recordFieldCount;
        private readonly TextWriter wr;

        public CSVWriter(TextWriter wr, string delimiter = ",")
        {
            this.wr = wr;
            Delimiter = delimiter;
            checkDelimForQuote = delimiter.Length > 1;
            quoteRequiredChars = checkDelimForQuote ? new[] { '\r', '\n' } : new[] { '\r', '\n', delimiter[0] };
        }

        public string Delimiter { get; }

        public string QuoteString
        {
            get => quoteString;
            set
            {
                quoteString = value;
                doubleQuoteString = value + value;
            }
        }

        public bool QuoteAllFields { get; set; } = false;

        public bool Trim { get; set; } = false;

        public void WriteField(string field)
        {
            bool shouldQuote = QuoteAllFields;

            field = field ?? string.Empty;

            if (field.Length > 0 && Trim) field = field.Trim();

            if (field.Length > 0)
                if (shouldQuote // Quote all fields
                    || field.Contains(quoteString) // Contains quote
                    || field[0] == ' ' // Starts with a space
                    || field[field.Length - 1] == ' ' // Ends with a space
                    || field.IndexOfAny(quoteRequiredChars) > -1 // Contains chars that require quotes
                    || checkDelimForQuote && field.Contains(Delimiter) // Contains delimiter
                   )
                    shouldQuote = true;

            // All quotes must be doubled.       
            if (shouldQuote && field.Length > 0) field = field.Replace(quoteString, doubleQuoteString);

            if (shouldQuote) field = quoteString + field + quoteString;
            if (recordFieldCount > 0) wr.Write(Delimiter);
            if (field.Length > 0)
                wr.Write(field);
            recordFieldCount++;
        }

        public void NextRecord()
        {
            wr.WriteLine();
            recordFieldCount = 0;
        }

        public void Save(string path)
        {
            
        }
    }
}