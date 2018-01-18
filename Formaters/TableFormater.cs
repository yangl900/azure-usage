using System;
using System.Linq;
using System.Text;
using ConsoleTables;

namespace azure_usage_report
{
    public static class TableFormater
    {
        public static string FormatSummary(OutputFormat format, string title, RegionalUsage[] regionalUsages)
        {
            var locations = regionalUsages
                .Select(u => u.Location)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var columns = new string[] { "resource" }.Concat(locations).Append("total").ToArray();
            var table = new ConsoleTable(columns);

            foreach (var resourceGroup in regionalUsages.GroupBy(u => u.Resource))
            {
                var row = new string[] { resourceGroup.Key }
                    .Concat(locations.Select(l => (resourceGroup.FirstOrDefault(u => u.Location.Equals(l, StringComparison.OrdinalIgnoreCase))?.Current ?? 0).ToString("N0")))
                    .Append(resourceGroup.Select(v => v.Current).Sum().ToString("N0"))
                    .ToArray();

                table.AddRow(row);
            }

            var output = new StringBuilder();
            if (!string.IsNullOrEmpty(title))
            {
                output.AppendLine(format == OutputFormat.Markdown ? "# " + title : title);
            }

            output.AppendLine(format == OutputFormat.Markdown ? table.ToMarkDownString() : table.ToStringAlternative());
            return output.ToString();
        }
    }
}