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
/// Command for managing CheckIns module operations
/// </summary>
public class CheckInsCommand : BaseCommand
{
    public CheckInsCommand(
        ILogger<CheckInsCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("checkins", "Manage check-in events, attendance data, and locations", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddEventsCommands();
        AddCheckInsCommands();
    }

    private void AddEventsCommands()
    {
        var eventsCommand = new Command("events", "Manage check-in events");
        
        // Add events list command
        var eventsListCommand = new Command("list", "List check-in events with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'name=Event Name')"),
            new Option<string?>("--order", "Sort order (e.g., 'name', '-created_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'locations,check_ins')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        eventsListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)eventsListCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)eventsListCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsListCommand.Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsListCommand.Options[6]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)eventsListCommand.Options[8]);
                
                var checkInsService = await GetServiceAsync<ICheckInsService>(token, detailedLogging);

                Logger.LogDebug("Fetching check-in events list");
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
                    queryParams.Include = include.Split(',').Select(i => i.Trim()).ToArray();
                }
                
                queryParams.PerPage = pageSize;
                queryParams.Offset = (page - 1) * pageSize;
                
                var response = await checkInsService.ListEventsAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} check-in events (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No check-in events found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing check-in events");
            }
        });

        // Add events get command
        var eventsGetCommand = new Command("get", "Get a specific check-in event by ID")
        {
            new Argument<string>("id", "Check-in event ID"),
            new Option<string?>("--include", "Related resources to include (e.g., 'locations,check_ins')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        eventsGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var id = context.ParseResult.GetValueForArgument((Argument<string>)eventsGetCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var include = context.ParseResult.GetValueForOption((Option<string?>)eventsGetCommand.Options[0]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsGetCommand.Options[1]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsGetCommand.Options[2]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)eventsGetCommand.Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)eventsGetCommand.Options[4]);
                
                ValidateRequiredParameter(id, "id");
                
                var checkInsService = await GetServiceAsync<ICheckInsService>(token, detailedLogging);

                Logger.LogDebug("Fetching check-in event: {EventId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var eventItem = await checkInsService.GetEventAsync(id);
                
                if (eventItem != null)
                {
                    var eventList = new List<object> { eventItem };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(eventList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Check-in event with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting check-in event");
            }
        });

        eventsCommand.AddCommand(eventsListCommand);
        eventsCommand.AddCommand(eventsGetCommand);
        AddCommand(eventsCommand);
    }



    private void AddCheckInsCommands()
    {
        var checkInsCommand = new Command("check-ins", "Manage individual check-ins");
        
        // Add check-ins list command
        var checkInsListCommand = new Command("list", "List individual check-ins with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'event_id=123')"),
            new Option<string?>("--order", "Sort order (e.g., 'created_at', '-updated_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'person,event,location')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        checkInsListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)checkInsListCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)checkInsListCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)checkInsListCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)checkInsListCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)checkInsListCommand.Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)checkInsListCommand.Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)checkInsListCommand.Options[6]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)checkInsListCommand.Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)checkInsListCommand.Options[8]);
                
                var checkInsService = await GetServiceAsync<ICheckInsService>(token, detailedLogging);

                Logger.LogDebug("Fetching check-ins list");
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
                    queryParams.Include = include.Split(',').Select(i => i.Trim()).ToArray();
                }
                
                queryParams.PerPage = pageSize;
                queryParams.Offset = (page - 1) * pageSize;
                
                var response = await checkInsService.ListCheckInsAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} check-ins (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No check-ins found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing check-ins");
            }
        });

        // Add check-ins get command
        var checkInsGetCommand = new Command("get", "Get a specific check-in by ID")
        {
            new Argument<string>("id", "Check-in ID"),
            new Option<string?>("--include", "Related resources to include (e.g., 'person,event,location')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        checkInsGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var id = context.ParseResult.GetValueForArgument((Argument<string>)checkInsGetCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var include = context.ParseResult.GetValueForOption((Option<string?>)checkInsGetCommand.Options[0]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)checkInsGetCommand.Options[1]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)checkInsGetCommand.Options[2]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)checkInsGetCommand.Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)checkInsGetCommand.Options[4]);
                
                ValidateRequiredParameter(id, "id");
                
                var checkInsService = await GetServiceAsync<ICheckInsService>(token, detailedLogging);

                Logger.LogDebug("Fetching check-in: {CheckInId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var checkIn = await checkInsService.GetCheckInAsync(id);
                
                if (checkIn != null)
                {
                    var checkInList = new List<object> { checkIn };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(checkInList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Check-in with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting check-in");
            }
        });

        checkInsCommand.AddCommand(checkInsListCommand);
        checkInsCommand.AddCommand(checkInsGetCommand);
        AddCommand(checkInsCommand);
    }
}