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
/// Command for managing Webhooks module operations
/// </summary>
public class WebhooksCommand : BaseCommand
{
    public WebhooksCommand(
        ILogger<WebhooksCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("webhooks", "Manage webhook subscriptions, events, and analytics", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddSubscriptionsCommands();
        AddEventsCommands();
        AddEventHistoryCommands();
    }

    private void AddSubscriptionsCommands()
    {
        var subscriptionsCommand = new Command("subscriptions", "Manage webhook subscriptions")
        {
            new Command("list", "List webhook subscriptions with optional filtering and pagination")
            {
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'created_at desc')"),
                new Option<string?>("--where", "Filter conditions (e.g., 'active=true')"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific webhook subscription by ID")
            {
                new Option<string>("--id", "Subscription ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("test", "Test a webhook subscription")
            {
                new Option<string>("--id", "Subscription ID") { IsRequired = true },
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("activate", "Activate a webhook subscription")
            {
                new Option<string>("--id", "Subscription ID") { IsRequired = true }
            },
            new Command("deactivate", "Deactivate a webhook subscription")
            {
                new Option<string>("--id", "Subscription ID") { IsRequired = true }
            }
        };

        // List subscriptions handler
        ((Command)subscriptionsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)subscriptionsCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)subscriptionsCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)subscriptionsCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[0]).Options[8]);

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching webhook subscriptions with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await webhooksService.ListSubscriptionsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} subscriptions (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No webhook subscriptions found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing webhook subscriptions");
            }
        });

        // Get subscription handler
        ((Command)subscriptionsCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)subscriptionsCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)subscriptionsCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Fetching webhook subscription with ID: {Id}", id);
                var subscription = await webhooksService.GetSubscriptionAsync(id!);

                if (subscription != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(subscription, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Webhook subscription with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting webhook subscription");
            }
        });

        // Test subscription handler
        ((Command)subscriptionsCommand.Subcommands[2]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)subscriptionsCommand.Subcommands[2]).Options[0]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[2]).Options[1]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)subscriptionsCommand.Subcommands[2]).Options[2]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)subscriptionsCommand.Subcommands[2]).Options[3]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)subscriptionsCommand.Subcommands[2]).Options[4]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Testing webhook subscription with ID: {Id}", id);
                var testResult = await webhooksService.TestSubscriptionAsync(id!);

                if (testResult != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(testResult, format, formatterOptions);
                    Console.WriteLine($"\nWebhook subscription test completed for ID '{id}'.");
                }
                else
                {
                    Console.WriteLine($"Failed to test webhook subscription with ID '{id}'.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "testing webhook subscription");
            }
        });

        // Activate subscription handler
        ((Command)subscriptionsCommand.Subcommands[3]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)subscriptionsCommand.Subcommands[3]).Options[0]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Activating webhook subscription with ID: {Id}", id);
                await webhooksService.ActivateSubscriptionAsync(id!);

                Console.WriteLine($"Webhook subscription with ID '{id}' has been activated.");
            }
            catch (Exception ex)
            {
                HandleError(ex, "activating webhook subscription");
            }
        });

        // Deactivate subscription handler
        ((Command)subscriptionsCommand.Subcommands[4]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)subscriptionsCommand.Subcommands[4]).Options[0]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Deactivating webhook subscription with ID: {Id}", id);
                await webhooksService.DeactivateSubscriptionAsync(id!);

                Console.WriteLine($"Webhook subscription with ID '{id}' has been deactivated.");
            }
            catch (Exception ex)
            {
                HandleError(ex, "deactivating webhook subscription");
            }
        });

        AddCommand(subscriptionsCommand);
    }

    private void AddEventsCommands()
    {
        var eventsCommand = new Command("events", "Manage webhook events")
        {
            new Command("list-available", "List available webhook events")
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
            new Command("validate", "Validate a webhook event")
            {
                new Option<string>("--event-id", "Event ID") { IsRequired = true },
                new Option<string>("--signature", "Webhook signature") { IsRequired = true },
                new Option<string>("--payload", "Event payload") { IsRequired = true },
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List available events handler
        ((Command)eventsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)eventsCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)eventsCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)eventsCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)eventsCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)eventsCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)eventsCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)eventsCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)eventsCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)eventsCommand.Subcommands[0]).Options[8]);

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching available webhook events with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await webhooksService.ListAvailableEventsAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} available events (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No available webhook events found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing available webhook events");
            }
        });

        // Validate event handler
        ((Command)eventsCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var eventId = context.ParseResult.GetValueForOption((Option<string>)((Command)eventsCommand.Subcommands[1]).Options[0]);
                var signature = context.ParseResult.GetValueForOption((Option<string>)((Command)eventsCommand.Subcommands[1]).Options[1]);
                var payload = context.ParseResult.GetValueForOption((Option<string>)((Command)eventsCommand.Subcommands[1]).Options[2]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)eventsCommand.Subcommands[1]).Options[3]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)eventsCommand.Subcommands[1]).Options[4]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)eventsCommand.Subcommands[1]).Options[5]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)eventsCommand.Subcommands[1]).Options[6]);

                ValidateRequiredParameter(eventId, "event-id");
                ValidateRequiredParameter(signature, "signature");
                ValidateRequiredParameter(payload, "payload");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Validating webhook event with ID: {EventId}", eventId);
                var isValid = await webhooksService.ValidateEventSignatureAsync(payload!, signature!, eventId!);

                var validationResult = new { EventId = eventId, IsValid = isValid, Message = isValid ? "Signature is valid" : "Signature is invalid" };
                
                var formatterOptions = CreateFormatterOptions(
                    includeProps, excludeProps, includeNulls, outputFile: outputFile);
                
                OutputData(validationResult, format, formatterOptions);
            }
            catch (Exception ex)
            {
                HandleError(ex, "validating webhook event");
            }
        });

        AddCommand(eventsCommand);
    }

    private void AddEventHistoryCommands()
    {
        var historyCommand = new Command("events", "Manage webhook events")
        {
            new Command("list", "List webhook events with optional filtering and pagination")
            {
                new Option<string>("--subscription-id", "Subscription ID") { IsRequired = true },
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
            new Command("get", "Get a specific event by ID")
            {
                new Option<string>("--id", "Event ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("redeliver", "Redeliver a webhook event")
            {
                new Option<string>("--id", "Event ID") { IsRequired = true }
            }
        };

        // List event history handler
        ((Command)historyCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var subscriptionId = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[0]).Options[0]);
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)historyCommand.Subcommands[0]).Options[1]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)historyCommand.Subcommands[0]).Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[0]).Options[3]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[0]).Options[4]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[0]).Options[5]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)historyCommand.Subcommands[0]).Options[6]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)historyCommand.Subcommands[0]).Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)historyCommand.Subcommands[0]).Options[8]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[0]).Options[9]);

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

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

                ValidateRequiredParameter(subscriptionId, "subscription-id");

                Logger.LogDebug("Fetching webhook events for subscription {SubscriptionId} with parameters: PageSize={PageSize}, Page={Page}", subscriptionId, pageSize, page);
                
                var response = await webhooksService.ListEventsAsync(subscriptionId!, parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} events (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No webhook events found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing webhook events");
            }
        });

        // Get event history handler
        ((Command)historyCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)historyCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)historyCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)historyCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)historyCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)historyCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Fetching webhook event with ID: {Id}", id);
                var eventHistory = await webhooksService.GetEventAsync(id!);

                if (eventHistory != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(eventHistory, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Webhook event with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting webhook event");
            }
        });

        // Redeliver event handler
        ((Command)historyCommand.Subcommands[2]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)historyCommand.Subcommands[2]).Options[0]);

                ValidateRequiredParameter(id, "id");

                var webhooksService = GetService<IWebhooksService>(token, detailedLogging);

                Logger.LogDebug("Redelivering webhook event with ID: {Id}", id);
                await webhooksService.RedeliverEventAsync(id!);

                Console.WriteLine($"Webhook event with ID '{id}' has been redelivered.");
            }
            catch (Exception ex)
            {
                HandleError(ex, "redelivering webhook event");
            }
        });

        AddCommand(historyCommand);
    }


}