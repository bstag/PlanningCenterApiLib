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
/// Command for managing Giving module operations
/// </summary>
public class GivingCommand : BaseCommand
{
    public GivingCommand(
        ILogger<GivingCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("giving", "Manage donations, funds, and giving data", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddDonationsCommands();
        AddFundsCommands();
        AddBatchesCommands();
        AddPledgesCommands();
        AddRecurringDonationsCommands();
    }

    private void AddDonationsCommands()
    {
        var donationsCommand = new Command("donations", "Manage donations")
        {
            new Command("list", "List donations with optional filtering and pagination")
            {
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'created_at desc')"),
                new Option<string?>("--where", "Filter conditions (e.g., 'amount>=100')"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific donation by ID")
            {
                new Option<string>("--id", "Donation ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List donations handler
        ((Command)donationsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)donationsCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)donationsCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)donationsCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)donationsCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)donationsCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[0]).Options[8]);

                var givingService = GetService<IGivingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching donations with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await givingService.ListDonationsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} donations (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No donations found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing donations");
            }
        });

        // Get donation handler
        ((Command)donationsCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)donationsCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)donationsCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)donationsCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)donationsCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)donationsCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var givingService = GetService<IGivingService>(token, detailedLogging);

                Logger.LogDebug("Fetching donation with ID: {Id}", id);
                var donation = await givingService.GetDonationAsync(id!);

                if (donation != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(donation, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Donation with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting donation");
            }
        });

        AddCommand(donationsCommand);
    }

    private void AddFundsCommands()
    {
        var fundsCommand = new Command("funds", "Manage funds")
        {
            new Command("list", "List funds with optional filtering and pagination")
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
            new Command("get", "Get a specific fund by ID")
            {
                new Option<string>("--id", "Fund ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List funds handler
        ((Command)fundsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)fundsCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)fundsCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)fundsCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)fundsCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)fundsCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[0]).Options[8]);

                var givingService = GetService<IGivingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching funds with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await givingService.ListFundsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} funds (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No funds found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing funds");
            }
        });

        // Get fund handler
        ((Command)fundsCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)fundsCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)fundsCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)fundsCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)fundsCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)fundsCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var givingService = GetService<IGivingService>(token, detailedLogging);

                Logger.LogDebug("Fetching fund with ID: {Id}", id);
                var fund = await givingService.GetFundAsync(id!);

                if (fund != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(fund, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Fund with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting fund");
            }
        });

        AddCommand(fundsCommand);
    }

    private void AddBatchesCommands()
    {
        var batchesCommand = new Command("batches", "Manage donation batches")
        {
            new Command("list", "List batches with optional filtering and pagination")
            {
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
            new Command("get", "Get a specific batch by ID")
            {
                new Option<string>("--id", "Batch ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List batches handler
        ((Command)batchesCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)batchesCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)batchesCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)batchesCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)batchesCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)batchesCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[0]).Options[8]);

                var givingService = GetService<IGivingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching batches with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await givingService.ListBatchesAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} batches (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No batches found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing batches");
            }
        });

        // Get batch handler
        ((Command)batchesCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)batchesCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)batchesCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)batchesCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)batchesCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)batchesCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var givingService = GetService<IGivingService>(token, detailedLogging);

                Logger.LogDebug("Fetching batch with ID: {Id}", id);
                var batch = await givingService.GetBatchAsync(id!);

                if (batch != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(batch, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Batch with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting batch");
            }
        });

        AddCommand(batchesCommand);
    }

    private void AddPledgesCommands()
    {
        var pledgesCommand = new Command("pledges", "Manage pledges")
        {
            new Command("list", "List pledges with optional filtering and pagination")
            {
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
            new Command("get", "Get a specific pledge by ID")
            {
                new Option<string>("--id", "Pledge ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List pledges handler
        ((Command)pledgesCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)pledgesCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)pledgesCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)pledgesCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)pledgesCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)pledgesCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[0]).Options[8]);

                var givingService = GetService<IGivingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching pledges with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await givingService.ListPledgesAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} pledges (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No pledges found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing pledges");
            }
        });

        // Get pledge handler
        ((Command)pledgesCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)pledgesCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)pledgesCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)pledgesCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)pledgesCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)pledgesCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var givingService = GetService<IGivingService>(token, detailedLogging);

                Logger.LogDebug("Fetching pledge with ID: {Id}", id);
                var pledge = await givingService.GetPledgeAsync(id!);

                if (pledge != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(pledge, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Pledge with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting pledge");
            }
        });

        AddCommand(pledgesCommand);
    }

    private void AddRecurringDonationsCommands()
    {
        var recurringCommand = new Command("recurring-donations", "Manage recurring donations")
        {
            new Command("list", "List recurring donations with optional filtering and pagination")
            {
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
            new Command("get", "Get a specific recurring donation by ID")
            {
                new Option<string>("--id", "Recurring donation ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List recurring donations handler
        ((Command)recurringCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)recurringCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)recurringCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)recurringCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)recurringCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)recurringCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[0]).Options[8]);

                var givingService = GetService<IGivingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching recurring donations with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await givingService.ListRecurringDonationsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} recurring donations (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No recurring donations found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing recurring donations");
            }
        });

        // Get recurring donation handler
        ((Command)recurringCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)recurringCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)recurringCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)recurringCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)recurringCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)recurringCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var givingService = GetService<IGivingService>(token, detailedLogging);

                Logger.LogDebug("Fetching recurring donation with ID: {Id}", id);
                var recurringDonation = await givingService.GetRecurringDonationAsync(id!);

                if (recurringDonation != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(recurringDonation, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Recurring donation with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting recurring donation");
            }
        });

        AddCommand(recurringCommand);
    }
}