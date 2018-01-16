# Introduction 
A simple tool that aggregates Azure compute usage across multiple subscriptions. Output as JSON or ascii tables to console.

Sample output:

## MSDN Subscription Usage
| resource          | eastus   | westus2  | total    |
|-------------------|----------|----------|----------|
| availabilitySets  | 1        | 0        | 1        |
| cores             | 1        | 1        | 2        |
| virtualMachines   | 1        | 1        | 2        |
| standardFFamily   | 1        | 1        | 2        |
| StandardDiskCount | 4        | 1        | 5        |

# Getting Started
```
Usage: azusage [options] [command]

Options:
  -?|-h|--help               Show help information
  -t|--tenant                The Azure tenant Id to sign in.
  -c|--client                Service principal client Id for accessing Azure subscriptions.
  -s|--secret                Service principal client secret for accessing Azure subscriptions.
  -o|--output                Output format: Json or Text.
  -n|--usage-name            Show only specified usages, e.g. cores. Can specify multiple values.
  -p|--subscription-pattern  Show only specified subscriptions, support regex. Can specify multiple values.

Commands:
  list     Lists subscription usages by individual subscriptions.
  summary  Summarize subscription usages by name groups.

Use "azusage [command] --help" for more information about a command.
```


# Build and Test
Build from commandline where .Net Core SDK available:
```
dotnet build
```

# Contribute
Contributions are welcome.