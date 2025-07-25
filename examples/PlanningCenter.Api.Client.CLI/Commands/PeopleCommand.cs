using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client.CLI.Formatters;
using PlanningCenter.Api.Client.CLI.Services;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Services;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace PlanningCenter.Api.Client.CLI.Commands;

/// <summary>
/// Command for managing People module operations
/// </summary>
public class PeopleCommand : BaseCommand
{
    public PeopleCommand(
        ILogger<PeopleCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("people", "Manage people and related data", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddListCommand();
        AddGetCommand();
        AddSearchCommand();
        AddMeCommand();
        AddHouseholdsCommand();
        AddFieldDataCommand();
    }

    private void AddListCommand()
    {
        var listCommand = new Command("list", "List people with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'first_name=John')"),
            new Option<string?>("--order", "Sort order (e.g., 'last_name', '-created_at')"),
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var pageSizeOption = listCommand.Options[0];
        var pageOption = listCommand.Options[1];
        var whereOption = listCommand.Options[2];
        var orderOption = listCommand.Options[3];
        var includeOption = listCommand.Options[4];
        var includePropsOption = listCommand.Options[5];
        var excludePropsOption = listCommand.Options[6];
        var outputFileOption = listCommand.Options[7];
        var includeNullsOption = listCommand.Options[8];

        listCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)listCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)listCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)listCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)listCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)listCommand.Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)listCommand.Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)listCommand.Options[6]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)listCommand.Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)listCommand.Options[8]);
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Fetching people list");
                var queryParams = new QueryParameters();
                
                if (!string.IsNullOrEmpty(where))
                {
                    var conditions = ParseWhereConditions(where);
                    foreach (var condition in conditions)
                    {
                        queryParams.Where.Add(condition.Key, condition.Value);
                    }
                }
                
                if (!string.IsNullOrEmpty(order))
                {
                    queryParams.Order = order;
                }
                
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams.Include = include != null ? new[] { include } : null;
                }
                
                queryParams.PerPage = pageSize;
                queryParams.Offset = (page - 1) * pageSize;
                
                var response = await peopleService.ListAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} people (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No people found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing people");
            }
        });

        AddCommand(listCommand);
    }

    private void AddGetCommand()
    {
        var getCommand = new Command("get", "Get a specific person by ID")
        {
            new Argument<string>("id", "Person ID"),
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var idArgument = getCommand.Arguments[0];
        var getIncludeOption = getCommand.Options[0];
        var getIncludePropsOption = getCommand.Options[1];
        var getExcludePropsOption = getCommand.Options[2];
        var getOutputFileOption = getCommand.Options[3];
        var getIncludeNullsOption = getCommand.Options[4];

        getCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var id = context.ParseResult.GetValueForArgument((Argument<string>)getCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var include = context.ParseResult.GetValueForOption((Option<string?>)getCommand.Options[0]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)getCommand.Options[1]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)getCommand.Options[2]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)getCommand.Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)getCommand.Options[4]);
                
                ValidateRequiredParameter(id, "id");
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Fetching person: {PersonId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include != null ? new[] { include } : null
                    };
                }
                
                var person = await peopleService.GetAsync(id);
                
                if (person != null)
                {
                    var personList = new List<Person> { person };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(personList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Person with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting person");
            }
        });

        AddCommand(getCommand);
    }

    private void AddSearchCommand()
    {
        var searchCommand = new Command("search", "Search for people by name or email")
        {
            new Argument<string>("query", "Search query (name or email)"),
            new Option<int>("--limit", () => 25, "Maximum number of results"),
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var queryArgument = searchCommand.Arguments[0];
        var searchLimitOption = searchCommand.Options[0];
        var searchIncludeOption = searchCommand.Options[1];
        var searchIncludePropsOption = searchCommand.Options[2];
        var searchExcludePropsOption = searchCommand.Options[3];
        var searchOutputFileOption = searchCommand.Options[4];
        var searchIncludeNullsOption = searchCommand.Options[5];

        searchCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var query = context.ParseResult.GetValueForArgument((Argument<string>)searchCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var limit = context.ParseResult.GetValueForOption((Option<int>)searchCommand.Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)searchCommand.Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)searchCommand.Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)searchCommand.Options[3]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)searchCommand.Options[4]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)searchCommand.Options[5]);
                
                ValidateRequiredParameter(query, "query");
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Searching people with query: {Query}", query);
                
                var queryParams = new QueryParameters
                {
                    Where = { ["search_name_or_email"] = query },
                    PerPage = limit
                };
                
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams.Include = include != null ? new[] { include } : null;
                }
                
                var response = await peopleService.ListAsync(queryParams);

                if (response?.Data != null && response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nFound {response.Data.Count()} of {response.Meta.TotalCount} people matching '{query}'");
                    }
                }
                else
                {
                    Console.WriteLine($"No people found matching '{query}'.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "searching people");
            }
        });

        AddCommand(searchCommand);
    }

    private void AddMeCommand()
    {
        var meCommand = new Command("me", "Get information about the authenticated user")
        {
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var meIncludePropsOption = meCommand.Options[0];
        var meExcludePropsOption = meCommand.Options[1];
        var meOutputFileOption = meCommand.Options[2];
        var meIncludeNullsOption = meCommand.Options[3];

        meCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)meCommand.Options[0]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)meCommand.Options[1]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)meCommand.Options[2]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)meCommand.Options[3]);
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Fetching current user information");
                var currentUser = await peopleService.GetMeAsync();
                
                if (currentUser != null)
                {
                    var userList = new List<Person> { currentUser };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(userList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine("Unable to retrieve current user information.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting current user");
            }
        });

        AddCommand(meCommand);
    }

    private void AddHouseholdsCommand()
    {
        var householdsCommand = new Command("households", "List households")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions"),
            new Option<string?>("--order", "Sort order"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var householdsPageSizeOption = householdsCommand.Options[0];
        var householdsPageOption = householdsCommand.Options[1];
        var householdsWhereOption = householdsCommand.Options[2];
        var householdsOrderOption = householdsCommand.Options[3];
        var householdsIncludePropsOption = householdsCommand.Options[4];
        var householdsExcludePropsOption = householdsCommand.Options[5];
        var householdsOutputFileOption = householdsCommand.Options[6];
        var householdsIncludeNullsOption = householdsCommand.Options[7];

        householdsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)householdsCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)householdsCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)householdsCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)householdsCommand.Options[3]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)householdsCommand.Options[4]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)householdsCommand.Options[5]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)householdsCommand.Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)householdsCommand.Options[7]);
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Fetching households list");
                // Note: This is a placeholder - households would need a separate service or endpoint
                var queryParams = new QueryParameters();
                
                if (!string.IsNullOrEmpty(where))
                {
                    var conditions = ParseWhereConditions(where);
                    foreach (var condition in conditions)
                    {
                        queryParams.Where.Add(condition.Key, condition.Value);
                    }
                }
                
                if (!string.IsNullOrEmpty(order))
                {
                    queryParams.Order = order;
                }
                
                queryParams.PerPage = pageSize;
                queryParams.Offset = (page - 1) * pageSize;
                
                var response = await peopleService.ListAsync(queryParams); // Placeholder for households

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} households (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No households found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing households");
            }
        });

        AddCommand(householdsCommand);
    }

    private void AddFieldDataCommand()
    {
        var fieldDataCommand = new Command("field-data", "Get field data for a person")
        {
            new Argument<string>("person-id", "Person ID"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to")
        };

        var fieldDataPersonIdArgument = fieldDataCommand.Arguments[0];
        var fieldDataIncludePropsOption = fieldDataCommand.Options[0];
        var fieldDataExcludePropsOption = fieldDataCommand.Options[1];
        var fieldDataOutputFileOption = fieldDataCommand.Options[2];

        fieldDataCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var personId = context.ParseResult.GetValueForArgument((Argument<string>)fieldDataCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)fieldDataCommand.Options[0]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)fieldDataCommand.Options[1]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)fieldDataCommand.Options[2]);
                
                ValidateRequiredParameter(personId, "person-id");
                
                var peopleService = await GetServiceAsync<IPeopleService>(token, detailedLogging);

                Logger.LogDebug("Fetching field data for person: {PersonId}", personId);
                // Note: Field data would need a separate method or endpoint
                var person = await peopleService.GetAsync(personId);
                
                if (person != null)
                {
                    // For now, just return the person data as field data is not directly available
                    var personList = new List<Person> { person };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, outputFile: outputFile);
                    
                    OutputData(personList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Person with ID '{personId}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting field data");
            }
        });

        AddCommand(fieldDataCommand);
    }

    private Dictionary<string, object> ParseWhereConditions(string whereClause)
    {
        var conditions = new Dictionary<string, object>();
        
        if (string.IsNullOrWhiteSpace(whereClause))
        {
            return conditions;
        }

        // Parse simple key=value pairs separated by commas
        var pairs = whereClause.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                
                // Remove quotes if present
                if ((value.StartsWith('"') && value.EndsWith('"')) ||
                    (value.StartsWith('\'') && value.EndsWith('\'')))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                
                conditions[key] = value;
            }
        }

        return conditions;
    }
}