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
/// Command for managing Calendar module operations
/// </summary>
public class CalendarCommand : BaseCommand
{
    public CalendarCommand(
        ILogger<CalendarCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("calendar", "Manage calendar events, resources, and scheduling", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddEventsCommands();
        AddResourcesCommands();
    }

    private void AddEventsCommands()
    {
        var eventsCommand = new Command("events", "Manage calendar events");
        
        // Add events list command
        var eventsListCommand = new Command("list", "List calendar events with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'name=Event Name')"),
            new Option<string?>("--order", "Sort order (e.g., 'starts_at', '-created_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'event_instances,resource_bookings')"),
            new Option<string?>("--start-date", "Filter events starting from this date (YYYY-MM-DD)"),
            new Option<string?>("--end-date", "Filter events ending before this date (YYYY-MM-DD)"),
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
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)eventsListCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)eventsListCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[4]);
                var startDate = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[5]);
                var endDate = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[6]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsListCommand.Options[7]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)eventsListCommand.Options[8]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)eventsListCommand.Options[9]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)eventsListCommand.Options[10]);
                
                var calendarService = await GetServiceAsync<ICalendarService>(token, detailedLogging);

                Logger.LogDebug("Fetching calendar events list");
                var queryParams = new QueryParameters();
                
                if (!string.IsNullOrEmpty(where))
                {
                    var conditions = ParseWhereConditions(where);
                    foreach (var condition in conditions)
                    {
                        queryParams.Where.Add(condition.Key, condition.Value);
                    }
                }
                
                // Add date range filtering
                if (!string.IsNullOrEmpty(startDate))
                {
                    queryParams.Where.Add("starts_at", $">={startDate}");
                }
                
                if (!string.IsNullOrEmpty(endDate))
                {
                    queryParams.Where.Add("ends_at", $"<={endDate}");
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
                
                var response = await calendarService.ListEventsAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} calendar events (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No calendar events found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing calendar events");
            }
        });

        // Add events get command
        var eventsGetCommand = new Command("get", "Get a specific calendar event by ID")
        {
            new Argument<string>("id", "Calendar event ID"),
            new Option<string?>("--include", "Related resources to include (e.g., 'event_instances,resource_bookings')"),
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
                
                var calendarService = await GetServiceAsync<ICalendarService>(token, detailedLogging);

                Logger.LogDebug("Fetching calendar event: {EventId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var eventItem = await calendarService.GetEventAsync(id);
                
                if (eventItem != null)
                {
                    var eventList = new List<object> { eventItem };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(eventList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Calendar event with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting calendar event");
            }
        });

        eventsCommand.AddCommand(eventsListCommand);
        eventsCommand.AddCommand(eventsGetCommand);
        AddCommand(eventsCommand);
    }

    private void AddResourcesCommands()
    {
        var resourcesCommand = new Command("resources", "Manage calendar resources");
        
        // Add resources list command
        var resourcesListCommand = new Command("list", "List calendar resources with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'name=Resource Name')"),
            new Option<string?>("--order", "Sort order (e.g., 'name', '-created_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'resource_bookings')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        resourcesListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)resourcesListCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)resourcesListCommand.Options[1]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)resourcesListCommand.Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)resourcesListCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)resourcesListCommand.Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)resourcesListCommand.Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)resourcesListCommand.Options[6]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)resourcesListCommand.Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)resourcesListCommand.Options[8]);
                
                var calendarService = await GetServiceAsync<ICalendarService>(token, detailedLogging);

                Logger.LogDebug("Fetching calendar resources list");
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
                
                var response = await calendarService.ListResourcesAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} calendar resources (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No calendar resources found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing calendar resources");
            }
        });

        // Add resources get command
        var resourcesGetCommand = new Command("get", "Get a specific calendar resource by ID")
        {
            new Argument<string>("id", "Calendar resource ID"),
            new Option<string?>("--include", "Related resources to include (e.g., 'resource_bookings')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        resourcesGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var id = context.ParseResult.GetValueForArgument((Argument<string>)resourcesGetCommand.Arguments[0]);
                
                // Get global options
                var token = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token")));
                var format = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format")));
                var detailedLogging = context.ParseResult.GetValueForOption(context.ParseResult.RootCommandResult.Command.Options.OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging")));
                
                // Get local options
                var include = context.ParseResult.GetValueForOption((Option<string?>)resourcesGetCommand.Options[0]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)resourcesGetCommand.Options[1]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)resourcesGetCommand.Options[2]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)resourcesGetCommand.Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)resourcesGetCommand.Options[4]);
                
                ValidateRequiredParameter(id, "id");
                
                var calendarService = await GetServiceAsync<ICalendarService>(token, detailedLogging);

                Logger.LogDebug("Fetching calendar resource: {ResourceId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var resource = await calendarService.GetResourceAsync(id);
                
                if (resource != null)
                {
                    var resourceList = new List<object> { resource };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(resourceList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Calendar resource with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting calendar resource");
            }
        });

        resourcesCommand.AddCommand(resourcesListCommand);
        resourcesCommand.AddCommand(resourcesGetCommand);
        AddCommand(resourcesCommand);
    }
}