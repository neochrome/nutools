using System;   
using System.Collections.Generic;
using System.Text;

namespace NuTools.Df
{
	public class DriveSummary
	{
        private class ColumnFormats
        {
            private class FormatDefinitions
            {
                public string Letter  = "{0,-5}";
                public string Type    = "{1,-5}";
                public string Size    = "{2,14}";
                public string Used    = "{3,14}";
                public string Free    = "{4,14}";
                public string Percent = "{5,4}%";
            }
            
            public ColumnFormats()
            {
                header = new FormatDefinitions();
                column = new FormatDefinitions();
            }

            public string CreateHeader()
            {
                var result = new StringBuilder();
                const char space = ' ';
                result.Append(header.Letter); result.Append(space);
                result.Append(header.Type);   result.Append(space);
                result.Append(header.Size);   result.Append(space);
                result.Append(header.Used);   result.Append(space);
                result.Append(header.Free);   result.Append(space);
                result.Append(header.Percent);
                return result.ToString();
            }

            public string CreateColumn()
            {
                var result = new StringBuilder();
                const char space = ' ';
                result.Append(column.Letter); result.Append(space);
                result.Append(column.Type);   result.Append(space);
                result.Append(column.Size);   result.Append(space);
                result.Append(column.Used);   result.Append(space);
                result.Append(column.Free);   result.Append(space);
                result.Append(column.Percent);
                return result.ToString();
            }

            public ColumnFormats WithHumanReadableFormat()
            {
                header.Size = "{2,6}";
                header.Used = "{3,6}";
                header.Free = "{4,6}";

                column.Size = "{2,6:fs}";
                column.Used = "{3,6:fs}";
                column.Free = "{4,6:fs}";

                return this;
            }

            private readonly FormatDefinitions header;
            private readonly FormatDefinitions column;
        }
        
        public DriveSummary(IEnumerable<IDrive> drives, bool humanReadable, bool printFileSystemType)
		{
            this.drives = drives;
            var columnFormats = new ColumnFormats();
            if (humanReadable)
            {
                columnFormats = columnFormats.WithHumanReadableFormat();
                formatProvider = new FileSizeFormatProvider();
            }

            headerFormatDefinition = columnFormats.CreateHeader();
            formatDefinition = columnFormats.CreateColumn();
			columnType = drive => {
				return string.Format(
                    formatProvider,
					formatDefinition,
					drive.Letter,
					drive.Format,
					drive.Size,
					drive.Used,
					drive.Free,
					PercentUsedOf(drive)
				);
			};
		    this.printFileSystemType = printFileSystemType;
		}

		public void DriveSummaryHumanReadable(IEnumerable<IDrive> drives, bool printFileSystemType)
		{
            var formatProvider = new FileSizeFormatProvider();
            var columnFormats = new ColumnFormats().WithHumanReadableFormat();
            headerFormatDefinition = columnFormats.CreateColumn();
		    formatDefinition = columnFormats.CreateColumn();
            columnType = drive =>
            {
				return string.Format(formatProvider, formatDefinition,
					drive.Letter,
					drive.Format,
					drive.Size,
					drive.Used,
					drive.Free,
					PercentUsedOf(drive)
				);
			};

		    return;
		}

		public string Render()
		{
			var summary = new StringBuilder();
            string[] names = { "Drive", "Type", "Size", "Used", "Avail", "Use" };
		    summary.AppendLine(string.Format(headerFormatDefinition, names));
			foreach (var drive in drives)
				summary.AppendLine(columnType(drive));
			return summary.ToString();
		}

		private int PercentUsedOf(IDrive drive)
		{
			return (drive.Size <= 0 || drive.Used <= 0) ? 0 : (int)Math.Round((drive.Used / (float)drive.Size) * 100);
		}

		private Func<IDrive, string> columnType;
		private readonly IEnumerable<IDrive> drives;
	    private string formatDefinition;
		private string headerFormatDefinition;
	    private bool printFileSystemType;
	    private FileSizeFormatProvider formatProvider;
	}
}