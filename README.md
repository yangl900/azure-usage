# Introduction 
A simple tool that aggregates Azure compute usage across multiple subscriptions. Output as JSON or ascii tables to console.

Sample output:

### Subscription Usage
| resource          | eastus   | westus2  | total    |
|-------------------|----------|----------|----------|
| availabilitySets  | 1        | 0        | 1        |
| cores             | 1        | 1        | 2        |
| virtualMachines   | 1        | 1        | 2        |

# Getting Started
```
Usage: azusage [options] [command]

Options:
  -?|-h|--help               Show help information
  -t|--tenant                The Azure tenant Id to sign in.
  -c|--client                Service principal client Id for accessing Azure subscriptions.
  -s|--secret                Service principal client secret for accessing Azure subscriptions.
  -o|--output                Output format: Json, Table, Markdown or Csv.
  --title                    Table and Markdown output title. Ignored in JSON output.
  -n|--usage-name            Show only specified usages, e.g. cores. Can specify multiple values.
  -p|--subscription-pattern  Show only specified subscriptions, support regex. Can specify multiple values.
  --access-token             Specify Azure Resource Manager access token directly. Service principal will be ignored in this case.

Commands:
  list     Lists subscription usages by individual subscriptions.
  summary  Summarize subscription usages by name groups.

Use "azusage [command] --help" for more information about a command.
```

### Run the tool inside Azure Cloud Shell
1. Open Cloud Shell [![Launch Cloud Shell](https://shell.azure.com/images/launchcloudshell.png "Launch Cloud Shell")](https://shell.azure.com)
2. Run following command to run the container in Azure Container Instance. Output will be print in container logs.

```
az container create -g demo -n azusage \
        --restart-policy Never \
        --image yangl/azure-usage:alpine \
        --command-line "dotnet azusage.dll summary --output table --access-token `az account get-access-token | jq .accessToken -r`"
az container logs -g demo -n azusage
```

### Run the tool on local machine using Docker & service principal credential:

```
docker run yangl/azure-usage:alpine summary --output table --client <sp-client-id> --secret <sp-secret> --tenant <tenant-id>
```

# Build and Test
Build from commandline where .Net Core SDK available:
```
dotnet build
```

# Contribute
Contributions are welcome.