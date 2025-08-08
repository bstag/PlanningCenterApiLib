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
/// Command for managing Services module operations
/// </summary>
public class ServicesCommand : BaseCommand
{
    public ServicesCommand(
        ILogger<ServicesCommand> logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("services", "Manage service plans, songs, and service types", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        AddPlansCommands();
        AddServiceTypesCommands();
        AddSongsCommands();
    }

    private void AddPlansCommands()
    {
        var plansCommand = new Command("plans", "Manage service plans");
        
        // Add plans list command
        var plansListCommand = new Command("list", "List service plans with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'service_type_id=123')"),
            new Option<string?>("--order", "Sort order (e.g., 'sort_date', '-created_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'service_type,items')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        plansListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, plansListCommand, 0, 25);
                var page = GetOptionValue<int>(context, plansListCommand, 1, 1);
                var where = GetOptionValue<string?>(context, plansListCommand, 2);
                var order = GetOptionValue<string?>(context, plansListCommand, 3);
                var include = GetOptionValue<string?>(context, plansListCommand, 4);
                var includeProps = GetOptionValue<string[]?>(context, plansListCommand, 5);
                var excludeProps = GetOptionValue<string[]?>(context, plansListCommand, 6);
                var outputFile = GetOptionValue<string?>(context, plansListCommand, 7);
                var includeNulls = GetOptionValue<bool>(context, plansListCommand, 8);
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching service plans list");
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
                
                var response = await servicesService.ListPlansAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} service plans (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No service plans found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing service plans");
            }
        });

        // Add plans get command
        var plansGetCommand = new Command("get", "Get a specific service plan by ID")
        {
            new Option<string>("--id", "Service plan ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include (e.g., 'service_type,items')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        plansGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var id = GetOptionValue<string>(context, plansGetCommand, 0);
                var include = GetOptionValue<string?>(context, plansGetCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, plansGetCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, plansGetCommand, 3);
                var outputFile = GetOptionValue<string?>(context, plansGetCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, plansGetCommand, 5);
                
                ValidateRequiredParameter(id, "id");
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching service plan: {PlanId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var plan = await servicesService.GetPlanAsync(id);
                
                if (plan != null)
                {
                    var planList = new List<object> { plan };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(planList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Service plan with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting service plan");
            }
        });

        plansCommand.AddCommand(plansListCommand);
        plansCommand.AddCommand(plansGetCommand);
        AddCommand(plansCommand);
    }

    private void AddServiceTypesCommands()
    {
        var serviceTypesCommand = new Command("service-types", "Manage service types");
        
        // Add service-types list command
        var serviceTypesListCommand = new Command("list", "List service types with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions"),
            new Option<string?>("--order", "Sort order (e.g., 'name', '-created_at')"),
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        serviceTypesListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, serviceTypesListCommand, 0, 25);
                var page = GetOptionValue<int>(context, serviceTypesListCommand, 1, 1);
                var where = GetOptionValue<string?>(context, serviceTypesListCommand, 2);
                var order = GetOptionValue<string?>(context, serviceTypesListCommand, 3);
                var include = GetOptionValue<string?>(context, serviceTypesListCommand, 4);
                var includeProps = GetOptionValue<string[]?>(context, serviceTypesListCommand, 5);
                var excludeProps = GetOptionValue<string[]?>(context, serviceTypesListCommand, 6);
                var outputFile = GetOptionValue<string?>(context, serviceTypesListCommand, 7);
                var includeNulls = GetOptionValue<bool>(context, serviceTypesListCommand, 8);
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching service types list");
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
                
                var response = await servicesService.ListServiceTypesAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} service types (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No service types found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing service types");
            }
        });

        // Add service-types get command
        var serviceTypesGetCommand = new Command("get", "Get a specific service type by ID")
        {
            new Option<string>("--id", "Service type ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        serviceTypesGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var id = GetOptionValue<string>(context, serviceTypesGetCommand, 0);
                var include = GetOptionValue<string?>(context, serviceTypesGetCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, serviceTypesGetCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, serviceTypesGetCommand, 3);
                var outputFile = GetOptionValue<string?>(context, serviceTypesGetCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, serviceTypesGetCommand, 5);
                
                ValidateRequiredParameter(id, "id");
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching service type: {ServiceTypeId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var serviceType = await servicesService.GetServiceTypeAsync(id);
                
                if (serviceType != null)
                {
                    var serviceTypeList = new List<object> { serviceType };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(serviceTypeList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Service type with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting service type");
            }
        });

        serviceTypesCommand.AddCommand(serviceTypesListCommand);
        serviceTypesCommand.AddCommand(serviceTypesGetCommand);
        AddCommand(serviceTypesCommand);
    }

    private void AddSongsCommands()
    {
        var songsCommand = new Command("songs", "Manage songs from the song library");
        
        // Add songs list command
        var songsListCommand = new Command("list", "List songs with optional filtering and pagination")
        {
            new Option<int>("--page-size", () => Configuration.GetDefaultPageSize(), "Number of items per page"),
            new Option<int>("--page", () => 1, "Page number to retrieve"),
            new Option<string?>("--where", "Filter conditions (e.g., 'title=Amazing Grace')"),
            new Option<string?>("--order", "Sort order (e.g., 'title', '-created_at')"),
            new Option<string?>("--include", "Related resources to include (e.g., 'arrangements')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        songsListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var pageSize = GetOptionValue<int>(context, songsListCommand, 0, 25);
                var page = GetOptionValue<int>(context, songsListCommand, 1, 1);
                var where = GetOptionValue<string?>(context, songsListCommand, 2);
                var order = GetOptionValue<string?>(context, songsListCommand, 3);
                var include = GetOptionValue<string?>(context, songsListCommand, 4);
                var includeProps = GetOptionValue<string[]?>(context, songsListCommand, 5);
                var excludeProps = GetOptionValue<string[]?>(context, songsListCommand, 6);
                var outputFile = GetOptionValue<string?>(context, songsListCommand, 7);
                var includeNulls = GetOptionValue<bool>(context, songsListCommand, 8);
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching songs list");
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
                
                var response = await servicesService.ListSongsAsync(queryParams);

                if (response?.Data != null)
                {
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(response.Data, format, formatterOptions);
                    
                    if (response.Meta?.TotalCount != null)
                    {
                        Console.WriteLine($"\nShowing {response.Data.Count()} of {response.Meta.TotalCount} songs (Page {page})");
                    }
                }
                else
                {
                    Console.WriteLine("No songs found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "listing songs");
            }
        });

        // Add songs get command
        var songsGetCommand = new Command("get", "Get a specific song by ID")
        {
            new Option<string>("--id", "Song ID") { IsRequired = true },
            new Option<string?>("--include", "Related resources to include (e.g., 'arrangements')"),
            new Option<string[]?>("--include-props", "Properties to include in output"),
            new Option<string[]?>("--exclude-props", "Properties to exclude from output"),
            new Option<string?>("--output-file", "File to write output to"),
            new Option<bool>("--include-nulls", "Include null values in output")
        };

        songsGetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                // Get global options
                var token = GetTokenOption(context);
                var format = GetFormatOption(context);
                var detailedLogging = GetDetailedLoggingOption(context);
                
                // Get local options
                var id = GetOptionValue<string>(context, songsGetCommand, 0);
                var include = GetOptionValue<string?>(context, songsGetCommand, 1);
                var includeProps = GetOptionValue<string[]?>(context, songsGetCommand, 2);
                var excludeProps = GetOptionValue<string[]?>(context, songsGetCommand, 3);
                var outputFile = GetOptionValue<string?>(context, songsGetCommand, 4);
                var includeNulls = GetOptionValue<bool>(context, songsGetCommand, 5);
                
                ValidateRequiredParameter(id, "id");
                
                var servicesService = GetService<IServicesService>(token, detailedLogging);

                Logger.LogDebug("Fetching song: {SongId}", id);
                
                QueryParameters? queryParams = null;
                if (!string.IsNullOrEmpty(include))
                {
                    queryParams = new QueryParameters
                    {
                        Include = include.Split(',').Select(i => i.Trim()).ToArray()
                    };
                }
                
                var song = await servicesService.GetSongAsync(id);
                
                if (song != null)
                {
                    var songList = new List<object> { song };
                    var formatterOptions = CreateFormatterOptions(
                        includeProps, excludeProps, includeNulls, outputFile: outputFile);
                    
                    OutputData(songList, format, formatterOptions);
                }
                else
                {
                    Console.WriteLine($"Song with ID '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "getting song");
            }
        });

        songsCommand.AddCommand(songsListCommand);
        songsCommand.AddCommand(songsGetCommand);
        AddCommand(songsCommand);
    }

}