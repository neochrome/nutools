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
                public string Letter  = "{0,-10}";
                public string Type    = "{1,-5}";
                public string Size    = "{2,9}";
                public string Used    = "{3,9}";
                public string Free    = "{4,9}";
                public string Percent = "{5,4}%";
            }
            
            public ColumnFormats()
            {
                header = new FormatDefinitions();
                column = new FormatDefinitions();
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

            public ColumnFormats WithFileSystemType()
            {
                showType = true;
                return this;
            }

            public string CreateHeader()
            {
                return BuildRowFrom(header);
            }

            public string CreateColumn()
            {
                return BuildRowFrom(column);
            }

            private string BuildRowFrom(FormatDefinitions input)
            {
                var result = new StringBuilder();
                const string space = "  ";
                result.Append(input.Letter); result.Append(space);
                if (showType) result.Append(input.Type);   result.Append(space);
                result.Append(input.Size);   result.Append(space);
                result.Append(input.Used);   result.Append(space);
                result.Append(input.Free);   result.Append(space);
                result.Append(input.Percent);
                return result.ToString();
            }

            private readonly FormatDefinitions header;
            private readonly FormatDefinitions column;
            private bool showType;
        }
        
        public DriveSummary(IEnumerable<IDrive> drives, bool humanReadable, bool printFileSystemType)
		{
            this.drives = drives;
            var sizeModifier = 1024;
            var columnFormats = new ColumnFormats();
            columnNames = new [] { "Drive", "Type", "1K-blocks", "Used", "Available", "Use" };

            if (humanReadable)
            {
                sizeModifier = 1;
                columnFormats = columnFormats.WithHumanReadableFormat();
                formatProvider = new FileSizeFormatProvider();
                columnNames[2] = "Size";
                columnNames[4] = "Avail";
            }

            if (printFileSystemType)
                columnFormats = columnFormats.WithFileSystemType();

            headerFormatDefinition = columnFormats.CreateHeader();
            formatDefinition = columnFormats.CreateColumn();
			columnType = drive =>
                string.Format(
			        formatProvider,
			        formatDefinition,
			        drive.Letter,
			        drive.Format,
			        drive.Size / sizeModifier,
			        drive.Used / sizeModifier,
			        drive.Free / sizeModifier,
			        PercentUsedOf(drive)
                );
		}

	    public string Render()
		{
			var summary = new StringBuilder();
		    summary.AppendLine(string.Format(headerFormatDefinition, columnNames));
			foreach (var drive in drives)
				summary.AppendLine(columnType(drive));
			return summary.ToString();
		}

		private int PercentUsedOf(IDrive drive)
		{
			return (drive.Size <= 0 || drive.Used <= 0) ? 0 : (int)Math.Round((drive.Used / (float)drive.Size) * 100);
		}

		private readonly Func<IDrive, string> columnType;
		private readonly IEnumerable<IDrive> drives;
	    private readonly string formatDefinition;
		private readonly string headerFormatDefinition;
	    private readonly FileSizeFormatProvider formatProvider;
	    private readonly string[] columnNames;
	}
}