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
/// Command for managing Publishing module operations
/// </summary>
public class PublishingCommand : BaseCommand
{
    public PublishingCommand(
        ILogger<PublishingCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("publishing", "Manage episodes, series, speakers, and media", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddEpisodesCommands();
        AddSeriesCommands();
        AddSpeakersCommands();
        AddSpeakershipsCommands();
        AddMediaCommands();
    }

    private void AddEpisodesCommands()
    {
        var episodesCommand = new Command("episodes", "Manage episodes")
        {
            new Command("list", "List episodes with optional filtering and pagination")
            {
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'published_at desc')"),
                new Option<string?>("--where", "Filter conditions (e.g., 'published=true')"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific episode by ID")
            {
                new Option<string>("--id", "Episode ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List episodes handler
        ((Command)episodesCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)episodesCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)episodesCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)episodesCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)episodesCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)episodesCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[0]).Options[8]);

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching episodes with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await publishingService.ListEpisodesAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} episodes (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No episodes found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing episodes");
            }
        });

        // Get episode handler
        ((Command)episodesCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)episodesCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)episodesCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)episodesCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)episodesCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)episodesCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

                Logger.LogDebug("Fetching episode with ID: {Id}", id);
                var episode = await publishingService.GetEpisodeAsync(id!);

                if (episode != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(episode, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Episode with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting episode");
            }
        });

        AddCommand(episodesCommand);
    }

    private void AddSeriesCommands()
    {
        var seriesCommand = new Command("series", "Manage series")
        {
            new Command("list", "List series with optional filtering and pagination")
            {
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'title asc')"),
                new Option<string?>("--where", "Filter conditions"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            },
            new Command("get", "Get a specific series by ID")
            {
                new Option<string>("--id", "Series ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List series handler
        ((Command)seriesCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)seriesCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)seriesCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)seriesCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)seriesCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)seriesCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[0]).Options[8]);

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching series with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await publishingService.ListSeriesAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} series (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No series found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing series");
            }
        });

        // Get series handler
        ((Command)seriesCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)seriesCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)seriesCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)seriesCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)seriesCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)seriesCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

                Logger.LogDebug("Fetching series with ID: {Id}", id);
                var series = await publishingService.GetSeriesAsync(id!);

                if (series != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(series, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Series with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting series");
            }
        });

        AddCommand(seriesCommand);
    }

    private void AddSpeakersCommands()
    {
        var speakersCommand = new Command("speakers", "Manage speakers")
        {
            new Command("list", "List speakers with optional filtering and pagination")
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
            new Command("get", "Get a specific speaker by ID")
            {
                new Option<string>("--id", "Speaker ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List speakers handler
        ((Command)speakersCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)speakersCommand.Subcommands[0]).Options[0]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)speakersCommand.Subcommands[0]).Options[1]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[0]).Options[2]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[0]).Options[3]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[0]).Options[4]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakersCommand.Subcommands[0]).Options[5]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakersCommand.Subcommands[0]).Options[6]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)speakersCommand.Subcommands[0]).Options[7]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[0]).Options[8]);

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

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

                Logger.LogDebug("Fetching speakers with parameters: PageSize={PageSize}, Page={Page}", pageSize, page);
                var response = await publishingService.ListSpeakersAsync(parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} speakers (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No speakers found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing speakers");
            }
        });

        // Get speaker handler
        ((Command)speakersCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)speakersCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakersCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakersCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)speakersCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakersCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

                Logger.LogDebug("Fetching speaker with ID: {Id}", id);
                var speaker = await publishingService.GetSpeakerAsync(id!);

                if (speaker != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(speaker, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Speaker with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting speaker");
            }
        });

        AddCommand(speakersCommand);
    }

    private void AddSpeakershipsCommands()
    {
        var speakershipsCommand = new Command("speakerships", "Manage speakerships")
        {
            new Command("list", "List speakerships with optional filtering and pagination")
            {
                new Option<string>("--episode-id", "Episode ID to list speakerships for") { IsRequired = true },
                new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
                new Option<int>("--page", () => 1, "Page number to retrieve"),
                new Option<string?>("--order", "Sort order (e.g., 'created_at desc')"),
                new Option<string?>("--where", "Filter conditions"),
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List speakerships handler
        ((Command)speakershipsCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var episodeId = context.ParseResult.GetValueForOption((Option<string>)((Command)speakershipsCommand.Subcommands[0]).Options[0]);
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)speakershipsCommand.Subcommands[0]).Options[1]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)speakershipsCommand.Subcommands[0]).Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakershipsCommand.Subcommands[0]).Options[3]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakershipsCommand.Subcommands[0]).Options[4]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakershipsCommand.Subcommands[0]).Options[5]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakershipsCommand.Subcommands[0]).Options[6]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)speakershipsCommand.Subcommands[0]).Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)speakershipsCommand.Subcommands[0]).Options[8]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)speakershipsCommand.Subcommands[0]).Options[9]);

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

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

                ValidateRequiredParameter(episodeId, "episode-id");

                Logger.LogDebug("Fetching speakerships for episode {EpisodeId} with parameters: PageSize={PageSize}, Page={Page}", episodeId, pageSize, page);
                var response = await publishingService.ListSpeakershipsAsync(episodeId!, parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} speakerships (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No speakerships found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing speakerships");
            }
        });



        AddCommand(speakershipsCommand);
    }

    private void AddMediaCommands()
    {
        var mediaCommand = new Command("media", "Manage media")
        {
            new Command("list", "List media with optional filtering and pagination")
            {
                new Option<string>("--episode-id", "Episode ID to list media for") { IsRequired = true },
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
            new Command("get", "Get a specific media by ID")
            {
                new Option<string>("--id", "Media ID") { IsRequired = true },
                new Option<string?>("--include", "Related resources to include"),
                new Option<string[]?>("--include-props", "Properties to include in output"),
                new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
                new Option<bool>("--include-nulls", "Include null values in output"),
                new Option<string?>("--output-file", "File to write output to")
            }
        };

        // List media handler
        ((Command)mediaCommand.Subcommands[0]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var episodeId = context.ParseResult.GetValueForOption((Option<string>)((Command)mediaCommand.Subcommands[0]).Options[0]);
                var pageSize = context.ParseResult.GetValueForOption((Option<int>)((Command)mediaCommand.Subcommands[0]).Options[1]);
                var page = context.ParseResult.GetValueForOption((Option<int>)((Command)mediaCommand.Subcommands[0]).Options[2]);
                var order = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[0]).Options[3]);
                var where = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[0]).Options[4]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[0]).Options[5]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)mediaCommand.Subcommands[0]).Options[6]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)mediaCommand.Subcommands[0]).Options[7]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)mediaCommand.Subcommands[0]).Options[8]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[0]).Options[9]);

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

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

                ValidateRequiredParameter(episodeId, "episode-id");

                Logger.LogDebug("Fetching media for episode {EpisodeId} with parameters: PageSize={PageSize}, Page={Page}", episodeId, pageSize, page);
                var response = await publishingService.ListMediaAsync(episodeId!, parameters);

                if (response.Data.Any())
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} media (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No media found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing media");
            }
        });

        // Get media handler
        ((Command)mediaCommand.Subcommands[1]).SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);

                var id = context.ParseResult.GetValueForOption((Option<string>)((Command)mediaCommand.Subcommands[1]).Options[0]);
                var include = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[1]).Options[1]);
                var includeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)mediaCommand.Subcommands[1]).Options[2]);
                var excludeProps = context.ParseResult.GetValueForOption((Option<string[]?>)((Command)mediaCommand.Subcommands[1]).Options[3]);
                var includeNulls = context.ParseResult.GetValueForOption((Option<bool>)((Command)mediaCommand.Subcommands[1]).Options[4]);
                var outputFile = context.ParseResult.GetValueForOption((Option<string?>)((Command)mediaCommand.Subcommands[1]).Options[5]);

                ValidateRequiredParameter(id, "id");

                var publishingService = GetService<IPublishingService>(token, detailedLogging);

                Logger.LogDebug("Fetching media with ID: {Id}", id);
                var media = await publishingService.GetMediaAsync(id!);

                if (media != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(media, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Media with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting media");
            }
        });

        AddCommand(mediaCommand);
    }
}