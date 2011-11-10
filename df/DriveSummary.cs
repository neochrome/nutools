using System;   
using System.Collections.Generic;
using System.Text;

namespace NuTools.Df
{
	public class DriveSummary
	{
        public DriveSummary(IEnumerable<IDrive> drives, bool humanReadable, bool printFileSystemType)
		{
            this.drives = drives;
            this.humanReadable = humanReadable;
            this.printFileSystemType = printFileSystemType;
            columnNames = new [] { "Drive", "Type", "1K-blocks", "Used", "Available", "Use" };
		}

	    public string Render()
		{
            var sizeModifier = 1024;
            var columnFormats = new ColumnFormats();

            if (humanReadable)
            {
                sizeModifier = 1;
                columnFormats.WithHumanReadableFormat();
                formatProvider = new FileSizeFormatProvider();
                columnNames[2] = "Size";
                columnNames[4] = "Avail";
            }

            if (printFileSystemType)
                columnFormats.WithFileSystemType();

            headerFormatDefinition = columnFormats.CreateHeader();
            formatDefinition = columnFormats.CreateColumn();

			var summary = new StringBuilder();
		    summary.AppendLine(string.Format(headerFormatDefinition, columnNames));
			foreach (var drive in drives)
            {
                var row = string.Format(
			        formatProvider,
			        formatDefinition,
			        drive.Letter,
			        drive.Format,
			        drive.Size / sizeModifier,
			        drive.Used / sizeModifier,
			        drive.Free / sizeModifier,
			        PercentUsedOf(drive)
                );

				summary.AppendLine(row);
            }
			return summary.ToString();
		}

		private int PercentUsedOf(IDrive drive)
		{
			return (drive.Size <= 0 || drive.Used <= 0) ? 0 : (int)Math.Round((drive.Used / (float)drive.Size) * 100);
		}

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

            public void WithHumanReadableFormat()
            {
                header.Size = "{2,6}";
                header.Used = "{3,6}";
                header.Free = "{4,6}";

                column.Size = "{2,6:fs}";
                column.Used = "{3,6:fs}";
                column.Free = "{4,6:fs}";
            }

            public void WithFileSystemType()
            {
                showType = true;
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
                if (showType) result.Append(input.Type); result.Append(space);
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

		private readonly IEnumerable<IDrive> drives;
	    private FileSizeFormatProvider formatProvider;
	    private string formatDefinition;
		private string headerFormatDefinition;
	    private readonly string[] columnNames;
	    private readonly bool humanReadable;
	    private readonly bool printFileSystemType;
	}
}