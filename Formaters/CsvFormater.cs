using System;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleTables;
using CsvHelper;

namespace azure_usage_report
{
    public static class CsvFormater
    {
        public static string FormatSummary(RegionalUsage[] regionalUsages)
        {
            var output = new StringWriter();
            var csv = new CsvWriter(output);

            csv.WriteRecords(regionalUsages);

            return output.ToString();
        }
    }
}