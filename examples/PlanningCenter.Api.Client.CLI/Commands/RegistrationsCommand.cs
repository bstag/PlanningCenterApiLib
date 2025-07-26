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
/// Command for managing Registrations module operations
/// </summary>
public class RegistrationsCommand : BaseCommand
{
    public RegistrationsCommand(
        ILogger<RegistrationsCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("registrations", "Manage signups and attendee information", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddSignupsCommands();
    }



    private void AddSignupsCommands()
    {
        var signupsCommand = new Command("signups", "Manage event signups");
        
        // Add signups list command
        var signupsListCommand = new Command("list", "List event signups with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'event_id=123')"),
            new Option<string?>("--order", "Sort order (e.g., 'created_at', '-updated_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'person,event')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        signupsListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, signupsListCommand, 0, 25);
                var page = GetOptionValue<int>(context, signupsListCommand, 1, 1);
                var where = GetOptionValue<string?>(context, signupsListCommand, 2);
                var order = GetOptionValue<string?>(context, signupsListCommand, 3);
                var include = GetOptionValue<string?>(context, signupsListCommand, 4);
                var includeProps = GetOptionValue<string[]?>(context, signupsListCommand, 5);
                var excludeProps = GetOptionValue<string[]?>(context, signupsListCommand, 6);
                var outputFile = GetOptionValue<string?>(context, signupsListCommand, 7);
                var includeNulls = GetOptionValue<bool>(context, signupsListCommand, 8);
                
                var registrationsService = GetService<IRegistrationsService>(token, detailedLogging);

                Logger.LogDebug("Fetching event signups list");
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
                
                var response = await registrationsService.ListSignupsAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} event signups (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No event signups found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing event signups");
            }
        });

        // Add signups get command
        var signupsGetCommand = new Command("get", "Get a specific event signup by ID")
        {
            new Option<string>("--id", "Event signup ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include (e.g., 'person,event')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        signupsGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var id = GetOptionValue<string>(context, signupsGetCommand, 0);
                var include = GetOptionValue<string?>(context, signupsGetCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, signupsGetCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, signupsGetCommand, 3);
                var outputFile = GetOptionValue<string?>(context, signupsGetCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, signupsGetCommand, 5);
                
                ValidateRequiredParameter(id, "id");
                
                var registrationsService = GetService<IRegistrationsService>(token, detailedLogging);

                Logger.LogDebug("Fetching event signup: {SignupId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var signup = await registrationsService.GetSignupAsync(id);
                
                if (signup != null)
                {
                    var signupList = new List<object> { signup };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(signupList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Event signup with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting event signup");
            }
        });

        signupsCommand.AddCommand(signupsListCommand);
        signupsCommand.AddCommand(signupsGetCommand);
        AddCommand(signupsCommand);
    }
}