using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client.CLI.Formatters;
using PlanningCenter.Api.Client.CLI.Services;
using PlanningCenter.Api.Client.Abstractions;
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
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, listCommand, 0, 25);
                var page = GetOptionValue<int>(context, listCommand, 1, 1);
                var where = GetOptionValue<string?>(context, listCommand, 2);
                var order = GetOptionValue<string?>(context, listCommand, 3);
                var include = GetOptionValue<string?>(context, listCommand, 4);
                var includeProps = GetOptionValue<string[]?>(context, listCommand, 5);
                var excludeProps = GetOptionValue<string[]?>(context, listCommand, 6);
                var outputFile = GetOptionValue<string?>(context, listCommand, 7);
                var includeNulls = GetOptionValue<bool>(context, listCommand, 8);
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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
            new Option<string>("--id", "Person ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        var getIdOption = getCommand.Options[0];
        var getIncludeOption = getCommand.Options[1];
        var getIncludePropsOption = getCommand.Options[2];
        var getExcludePropsOption = getCommand.Options[3];
        var getOutputFileOption = getCommand.Options[4];
        var getIncludeNullsOption = getCommand.Options[5];

        getCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var id = GetOptionValue<string>(context, getCommand, 0);
                var include = GetOptionValue<string?>(context, getCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, getCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, getCommand, 3);
                var outputFile = GetOptionValue<string?>(context, getCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, getCommand, 5);
                
                ValidateRequiredParameter(id, "id");
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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
                var query = GetArgumentValue<string>(context, searchCommand, 0);
                
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var limit = GetOptionValue<int>(context, searchCommand, 0, 25);
                var include = GetOptionValue<string?>(context, searchCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, searchCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, searchCommand, 3);
                var outputFile = GetOptionValue<string?>(context, searchCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, searchCommand, 5);
                
                ValidateRequiredParameter(query, "query");
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var includeProps = GetOptionValue<string[]?>(context, meCommand, 0);
                var excludeProps = GetOptionValue<string[]?>(context, meCommand, 1);
                var outputFile = GetOptionValue<string?>(context, meCommand, 2);
                var includeNulls = GetOptionValue<bool>(context, meCommand, 3);
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, householdsCommand, 0, 25);
                var page = GetOptionValue<int>(context, householdsCommand, 1, 1);
                var where = GetOptionValue<string?>(context, householdsCommand, 2);
                var order = GetOptionValue<string?>(context, householdsCommand, 3);
                var includeProps = GetOptionValue<string[]?>(context, householdsCommand, 4);
                var excludeProps = GetOptionValue<string[]?>(context, householdsCommand, 5);
                var outputFile = GetOptionValue<string?>(context, householdsCommand, 6);
                var includeNulls = GetOptionValue<bool>(context, householdsCommand, 7);
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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
                var personId = GetArgumentValue<string>(context, fieldDataCommand, 0);
                
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var includeProps = GetOptionValue<string[]?>(context, fieldDataCommand, 0);
                var excludeProps = GetOptionValue<string[]?>(context, fieldDataCommand, 1);
                var outputFile = GetOptionValue<string?>(context, fieldDataCommand, 2);
                
                ValidateRequiredParameter(personId, "person-id");
                
                var peopleService = GetService<IPeopleService>(token, detailedLogging);

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

}