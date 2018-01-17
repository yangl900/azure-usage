
namespace azure_usage_report
{
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Extensions.CommandLineUtils;
    using System.Text.RegularExpressions;
    using ConsoleTables;

    class Program
    {
        static void Main(params string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "azusage";
            app.FullName = "Azure Subscription Usage Report";

            app.HelpOption("-?|-h|--help");

            var tenantId = app.Option("-t|--tenant", "The Azure tenant Id to sign in.", CommandOptionType.SingleValue);
            var clientId = app.Option("-c|--client", "Service principal client Id for accessing Azure subscriptions.", CommandOptionType.SingleValue);
            var clientSecret = app.Option("-s|--secret", "Service principal client secret for accessing Azure subscriptions.", CommandOptionType.SingleValue);
            var outputOption = app.Option("-o|--output", "Output format: Json, Table or Markdown.", CommandOptionType.SingleValue);
            var titleOption = app.Option("--title", "Table and Markdown output title. Ignored in JSON output.", CommandOptionType.SingleValue);
            var usageOption = app.Option("-n|--usage-name", "Show only specified usages, e.g. cores. Can specify multiple values.", CommandOptionType.MultipleValue);
            var subscriptionFilterOption = app.Option("-p|--subscription-pattern", "Show only specified subscriptions, support regex. Can specify multiple values.", CommandOptionType.MultipleValue);
            var accessTokenOption = app.Option("--access-token", "Specify Azure Resource Manager access token directly. Service principal will be ignored in this case.", CommandOptionType.SingleValue);

            app.Command("list", (command) =>
            {
                command.Description = "Lists subscription usages by individual subscriptions.";
                command.HelpOption("-?|-h|--help");

                command.Options.Add(clientId);
                command.Options.Add(clientSecret);
                command.Options.Add(tenantId);
                command.Options.Add(outputOption);
                command.Options.Add(usageOption);
                command.Options.Add(subscriptionFilterOption);
                command.Options.Add(accessTokenOption);

                command.OnExecute(() =>
                    {
                        if (!accessTokenOption.HasValue() && !(tenantId.HasValue() && clientId.HasValue() && clientSecret.HasValue()))
                        {
                            Console.WriteLine(app.GetHelpText());
                            return 1;
                        }

                        var outputFormatStr = outputOption.HasValue()
                            ? outputOption.Value()
                            : OutputFormat.Json.ToString();

                        OutputFormat format;
                        if (!Enum.TryParse<OutputFormat>(outputFormatStr, ignoreCase: true, result: out format))
                        {
                            Console.WriteLine(outputFormatStr);
                            Console.WriteLine(app.GetHelpText());
                            return 1;
                        }

                        if (format != OutputFormat.Json)
                        {
                            Console.WriteLine("List usage only support JSON output.");
                            return 1;
                        }

                        var subscriptionPattern = subscriptionFilterOption.HasValue()
                            ? new Regex(subscriptionFilterOption.Value(), RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5))
                            : new Regex(".*", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                        var filter = usageOption.HasValue() ? usageOption.Values.ToArray() : new string[0];

                        var accessToken = Program.GetAccessToken(accessTokenOption, tenantId, clientId, clientSecret);
                        if (string.IsNullOrEmpty(accessToken))
                        {
                            Console.WriteLine("Failed to get access token.");
                            return 1;
                        }

                        var usages = Program.GetUsage(accessToken, filter, subscriptionPattern).Result;
                        Console.WriteLine(JsonConvert.SerializeObject(usages, Formatting.Indented));

                        return 0;
                    });
            });

            app.Command("summary", (command) =>
            {
                command.Description = "Summarize subscription usages by name groups.";
                command.HelpOption("-?|-h|--help");

                command.Options.Add(clientId);
                command.Options.Add(clientSecret);
                command.Options.Add(tenantId);
                command.Options.Add(outputOption);
                command.Options.Add(usageOption);
                command.Options.Add(subscriptionFilterOption);
                command.Options.Add(titleOption);
                command.Options.Add(accessTokenOption);

                command.OnExecute(() =>
                {
                    if (!accessTokenOption.HasValue() && !(tenantId.HasValue() && clientId.HasValue() && clientSecret.HasValue()))
                    {
                        Console.WriteLine(app.GetHelpText());
                        return 1;
                    }

                    var outputFormatStr = outputOption.HasValue()
                        ? outputOption.Value()
                        : OutputFormat.Table.ToString();

                    OutputFormat format;
                    if (!Enum.TryParse<OutputFormat>(outputFormatStr, ignoreCase: true, result: out format))
                    {
                        Console.WriteLine(outputFormatStr);
                        Console.WriteLine(app.GetHelpText());
                        return 1;
                    }

                    if (format != OutputFormat.Table && format != OutputFormat.Markdown)
                    {
                        Console.WriteLine("Summary usage only support table or markdown output.");
                        return 1;
                    }

                    var subscriptionPattern = subscriptionFilterOption.HasValue()
                        ? new Regex(subscriptionFilterOption.Value(), RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5))
                        : new Regex(".*", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                    var specifiedUsages = usageOption.HasValue() ? usageOption.Values.ToArray() : new string[0];

                    var accessToken = Program.GetAccessToken(accessTokenOption, tenantId, clientId, clientSecret);
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        Console.WriteLine("Failed to get access token.");
                        return 1;
                    }

                    var usages = Program.GetUsage(
                        accessToken: accessToken, 
                        specifiedUsages: specifiedUsages,
                        subscriptionPattern: subscriptionPattern).Result;

                    if (!usages.Any())
                    {
                        Console.WriteLine("No usage reported.");
                        return 0;
                    }

                    var groupedUsages = usages
                        .SelectMany(usage => usage.Usages)
                        .GroupBy(u => u.Name.Value);

                    var locations = usages
                        .SelectMany(usage => usage.Usages)
                        .Select(u => u.Location)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    var summary = new Dictionary<string, int[]>();
                    foreach (var group in groupedUsages)
                    {
                        var usageSummary = locations
                            .Select(location => group.Where(u => string.Equals(u.Location, location)).Sum(u => u.CurrentValue))
                            .ToArray();

                        summary.Add(group.Key, usageSummary);
                    }

                    var columns = new string[] { "resource" }.Concat(locations).Append("total").ToArray();
                    var table = new ConsoleTable(columns);
                    
                    foreach (var usageSummary in summary)
                    {
                        var row = new string[] { usageSummary.Key }
                            .Concat(usageSummary.Value.Select(v => v.ToString("N0")))
                            .Append(usageSummary.Value.Sum().ToString("N0"))
                            .ToArray();

                        table.AddRow(row);
                    }

                    if (titleOption.HasValue())
                    {
                        var title = format == OutputFormat.Markdown
                            ? "# " + titleOption.Value()
                            : titleOption.Value();

                        Console.WriteLine(title);
                    }

                    table.Write(format == OutputFormat.Markdown ? Format.MarkDown : Format.Alternative);
                    return 0;
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("No options specified!");
                Console.WriteLine(app.GetHelpText());
                return 0;
            });

            app.Execute(args);
        }

        private static string GetAccessToken(CommandOption accessTokenOption, CommandOption tenantIdOption, CommandOption clientIdOption, CommandOption clientSecretOption)
        {
            return accessTokenOption.HasValue()
                ? accessTokenOption.Value()
                : AuthUtil.GetToken(tenantIdOption.Value(), clientIdOption.Value(), clientSecretOption.Value())?.AccessToken;
        }

        static async Task<SubscriptionUsage[]> GetUsage(string accessToken, string[] specifiedUsages, Regex subscriptionPattern)
        {
            var subscriptionsProvider = new SubscriptionsDataProvider();
            var usageProvider = new UsageDataProvider();
            var locationsProvider = new LocationsDataProvider();

            var locations = (await locationsProvider.GetLocations(accessToken)).Select(l => l.Name).ToArray();
            var subscriptions = await subscriptionsProvider.GetSubscriptions(accessToken);
            var subscriptionUsages = new List<SubscriptionUsage>();

            foreach (var sub in subscriptions)
            {
                if (!subscriptionPattern.IsMatch(sub.DisplayName))
                {
                    continue;
                }

                var subscriptionUsage = new SubscriptionUsage() { Subscription = sub, Usages = new Usage[0] };
                var tasks = locations.Select(l => Program.GetRegionalUsage(accessToken, sub.SubscriptionId, l)).ToArray();
                await Task.WhenAll(tasks);

                var regionalUsages = tasks.Select(task => task.Result).ToArray();

                foreach (var regionalUsage in regionalUsages)
                {
                    var reportUsages = regionalUsage.Value
                        .Where(u => (!specifiedUsages.Any() && u.CurrentValue != 0) || specifiedUsages.Any(f => u.Name.Value.Equals(f, StringComparison.CurrentCultureIgnoreCase)))
                        .ToArray();

                    Array.ForEach(reportUsages, u => u.Location = regionalUsage.Key);

                    subscriptionUsage.Usages = subscriptionUsage.Usages.Concat(reportUsages).ToArray();
                }
                subscriptionUsages.Add(subscriptionUsage);
            }

            return subscriptionUsages.ToArray();
        }

        static async Task<KeyValuePair<string, Usage[]>> GetRegionalUsage(string accessToken, string subscriptionId, string location)
        {
            var usageProvider = new UsageDataProvider();
            var usages = await usageProvider.GetComputeUsage(accessToken, subscriptionId, location);

            return KeyValuePair.Create(location, usages.Where(u => u.CurrentValue != 0).ToArray());
        }
    }
}
