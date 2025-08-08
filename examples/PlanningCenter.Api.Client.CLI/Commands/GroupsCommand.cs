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
/// Command for managing Groups module operations
/// </summary>
public class GroupsCommand : BaseCommand
{
    public GroupsCommand(
        ILogger<GroupsCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("groups", "Manage groups, group types, and memberships", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddGroupsCommands();
        AddGroupTypesCommands();
        AddMembershipsCommands();
    }

    private void AddGroupsCommands()
    {
        var groupsCommand = new Command("list", "List groups with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--order", "Sort order (e.g., 'name asc')"),
            new Option<string?>("--where", "Filter conditions (e.g., 'name=*youth*')"),
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<bool>("--include-nulls", "Include null values in output"),
            new Option<string?>("--output-file", "File to write output to")
        };

        var getGroupCommand = new Command("get", "Get a specific group by ID")
        {
            new Option<string>("--id", "Group ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<bool>("--include-nulls", "Include null values in output"),
            new Option<string?>("--output-file", "File to write output to")
        };

        // List groups handler
        groupsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)groupsCommand.Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)groupsCommand.Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)groupsCommand.Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)groupsCommand.Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)groupsCommand.Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)groupsCommand.Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)groupsCommand.Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)groupsCommand.Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)groupsCommand.Options[8]);

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                var parameters = new QueryParameters
                {
                    PerPage = pageSize,
                    Offset = (page - 1) * pageSize
                };

                if (!string.IsNullOrEmpty(order))
                {
                    parameters.Order = order;
                }

                if (!string.IsNullOrEmpty(where))
                {
                    var whereConditions = ParseWhereConditions(where);
                    foreach (var condition in whereConditions)
                    {
                        parameters.Where.Add(condition.Key, condition.Value);
                    }
                }

                if (!string.IsNullOrEmpty(include))
                {
                    parameters.Include = ParseCommaSeparatedString(include);
                }

                Logger.LogDebug("Fetching groups with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await groupsService.ListGroupsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} groups (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No groups found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing groups");
            }
        });

        // Get group handler
        getGroupCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)getGroupCommand.Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)getGroupCommand.Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)getGroupCommand.Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)getGroupCommand.Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)getGroupCommand.Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)getGroupCommand.Options[5]);

                ValidateRequiredParameter(id, "id");

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                Logger.LogDebug("Fetching group with ID: {Id}", id);
                var group = await groupsService.GetGroupAsync(id!);

                if (group != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(group, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Group with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting group");
            }
        });

        AddCommand(groupsCommand);
        AddCommand(getGroupCommand);
    }

    private void AddGroupTypesCommands()
    {
        var groupTypesCommand = new Command("group-types", "Manage group types")
        {
            new Command("list", "List group types with optional filtering and pagination")
            {
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'name asc')"),
                new Option<string?>("--where", "Filter conditions"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific group type by ID")
            {
                new Option<string>("--id", "Group type ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List group types handler
        ((Command)groupTypesCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)groupTypesCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)groupTypesCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)groupTypesCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)groupTypesCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)groupTypesCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[0]).Options[8]);

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                var parameters = new QueryParameters
                {
                    PerPage = pageSize,
                    Offset = (page - 1) * pageSize
                };

                if (!string.IsNullOrEmpty(order))
                {
                    parameters.Order = order;
                }

                if (!string.IsNullOrEmpty(where))
                {
                    var whereConditions = ParseWhereConditions(where);
                    foreach (var condition in whereConditions)
                    {
                        parameters.Where.Add(condition.Key, condition.Value);
                    }
                }

                if (!string.IsNullOrEmpty(include))
                {
                    parameters.Include = ParseCommaSeparatedString(include);
                }

                Logger.LogDebug("Fetching group types with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await groupsService.ListGroupTypesAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} group types (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No group types found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing group types");
            }
        });

        // Get group type handler
        ((Command)groupTypesCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)groupTypesCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)groupTypesCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)groupTypesCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)groupTypesCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)groupTypesCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                Logger.LogDebug("Fetching group type with ID: {Id}", id);
                var groupType = await groupsService.GetGroupTypeAsync(id!);

                if (groupType != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(groupType, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Group type with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting group type");
            }
        });

        AddCommand(groupTypesCommand);
    }

    private void AddMembershipsCommands()
    {
        var membershipsCommand = new Command("memberships", "Manage group memberships")
        {
            new Command("list", "List memberships with optional filtering and pagination")
            {
                new Option<string>("--group-id", "Group ID to list memberships for") { IsRequired = true },
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'created_at desc')"),
                new Option<string?>("--where", "Filter conditions"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific membership by ID")
            {
                new Option<string>("--group-id", "Group ID") { IsRequired = true },
                new Option<string>("--membership-id", "Membership ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List memberships handler
        ((Command)membershipsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var groupId = context.ParseResult.GetValueForOption((Option<string>)((Command)membershipsCommand.Subcommands[0]).Options[0]);
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)membershipsCommand.Subcommands[0]).Options[1]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)membershipsCommand.Subcommands[0]).Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[0]).Options[3]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[0]).Options[4]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[0]).Options[5]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)membershipsCommand.Subcommands[0]).Options[6]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)membershipsCommand.Subcommands[0]).Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)membershipsCommand.Subcommands[0]).Options[8]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[0]).Options[9]);

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                var parameters = new QueryParameters
                {
                    PerPage = pageSize,
                    Offset = (page - 1) * pageSize
                };

                if (!string.IsNullOrEmpty(order))
                {
                    parameters.Order = order;
                }

                if (!string.IsNullOrEmpty(where))
                {
                    var whereConditions = ParseWhereConditions(where);
                    foreach (var condition in whereConditions)
                    {
                        parameters.Where.Add(condition.Key, condition.Value);
                    }
                }

                if (!string.IsNullOrEmpty(include))
                {
                    parameters.Include = ParseCommaSeparatedString(include);
                }

                ValidateRequiredParameter(groupId, "group-id");

                Logger.LogDebug("Fetching memberships for group {GroupId} with parameters: PageSize={PageSize}, Page={Page}", groupId, pageSize, page);
                var response = await groupsService.ListGroupMembershipsAsync(groupId!, parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} memberships (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No memberships found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing memberships");
            }
        });

        // Get membership handler
        ((Command)membershipsCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var groupId = context.ParseResult.GetValueForOption((Option<string>)((Command)membershipsCommand.Subcommands[1]).Options[0]);
                var membershipId = context.ParseResult.GetValueForOption((Option<string>)((Command)membershipsCommand.Subcommands[1]).Options[1]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[1]).Options[2]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)membershipsCommand.Subcommands[1]).Options[3]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)membershipsCommand.Subcommands[1]).Options[4]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)membershipsCommand.Subcommands[1]).Options[5]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)membershipsCommand.Subcommands[1]).Options[6]);

                ValidateRequiredParameter(groupId, "group-id");
                ValidateRequiredParameter(membershipId, "membership-id");

                var groupsService = GetService<IGroupsService>(token, detailedLogging);

                Logger.LogDebug("Fetching membership with ID: {MembershipId} for group: {GroupId}", membershipId, groupId);
                var membership = await groupsService.GetGroupMembershipAsync(groupId!, membershipId!);

                if (membership != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(membership, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Membership with ID '{membershipId}' not found in group '{groupId}'.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting membership");
            }
        });

        AddCommand(membershipsCommand);
    }
}