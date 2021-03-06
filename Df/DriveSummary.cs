﻿using System;   
using System.Collections.Generic;
using System.Text;

using NuTools.Common;

namespace NuTools.Df
{
	public class DriveSummary
	{
        public DriveSummary(IEnumerable<IDrive> drives, bool humanReadable, bool humanReadableWithSi, bool posixFormat, bool printFileSystemType, bool customBlockSize, int blockSize)
		{
            this.drives = drives;
            this.humanReadable = humanReadable;
            this.humanReadableWithSi = humanReadableWithSi;
            this.posixFormat = posixFormat;
            this.printFileSystemType = printFileSystemType;
            this.customBlockSize = customBlockSize;
            this.blockSize = blockSize;
            sizeModifier = 1024;
            optionActions = new List<Action>();
        }

	    public string Render()
		{
            var columnNames = new [] { "Drive", "Type", "1K-blocks", "Used", "Available", "Use" };
            var columnFormats = new ColumnFormats();

            if (humanReadableWithSi && !customBlockSize)
                optionActions.Add(() => { 
                    ModifyForHumanReadableOutput(columnFormats, columnNames);
                    sizeModifier = 1000;
                });

            if (!humanReadableWithSi && humanReadable && !customBlockSize)
                optionActions.Add(() => ModifyForHumanReadableOutput(columnFormats, columnNames));

            if (posixFormat && !humanReadable)
                optionActions.Add(() => {
                    columnFormats.WithPosixFormat();
                    columnNames[2] = "1024-blocks";
                    columnNames[5] = "Capacity";
                });

            if (customBlockSize)
                optionActions.Add(() => {
                    sizeModifier = blockSize;
                    columnNames[2] = string.Format(new FileSizeFormatProvider(true), "{0:fs}-blocks", sizeModifier);
                });

            if (printFileSystemType)
                optionActions.Add(columnFormats.WithFileSystemType);

            optionActions.Each(a => a.Invoke());

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

	    private void ModifyForHumanReadableOutput(ColumnFormats columnFormats, string[] columnNames)
	    {
	        sizeModifier = 1;
	        columnFormats.WithHumanReadableFormat();
	        formatProvider = new FileSizeFormatProvider();
	        columnNames[2] = "Size";
	        columnNames[4] = "Avail";
	    }

	    private int PercentUsedOf(IDrive drive)
		{
			return (drive.Size <= 0 || drive.Used <= 0) ? 0 : (int)Math.Round((drive.Used / (float)drive.Size) * 100);
		}

        private class ColumnFormats
        {
            private class FormatDefinitions
            {
                public string Letter  = "{0,-11}";
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

            public void WithPosixFormat()
            {
                header.Size = "{2,11}";

                var values = column.Size.TrimStart('{').TrimEnd('}').Split(',');
                var index = values[0];
                var padding = values[1];
                var modifiedPadding = padding.Contains(":fs") ? "11:fs" : "11";
                column.Size = "{" + index + "," + modifiedPadding + "}";

                header.Percent = "{5,8}";
                column.Percent = "{5,7}%";
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

            private FormatDefinitions header;
            private FormatDefinitions column;
            private bool showType;
        }

        private IList<Action> optionActions;

		private readonly IEnumerable<IDrive> drives;
	    private FileSizeFormatProvider formatProvider;
	    private string formatDefinition;
		private string headerFormatDefinition;
	    private bool humanReadable;
	    private bool humanReadableWithSi;
	    private bool printFileSystemType;
	    private readonly bool customBlockSize;
	    private bool posixFormat;
        private int sizeModifier;
	    private int blockSize;
	}
}