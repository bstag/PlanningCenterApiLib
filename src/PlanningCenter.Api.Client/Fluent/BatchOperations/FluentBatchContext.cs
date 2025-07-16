using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Fluent.BatchOperations;

/// <summary>
/// Fluent API for batch operations on people.
/// Allows efficient bulk creation, updates, and operations.
/// </summary>
public class FluentBatchContext
{
    private readonly IPeopleService _peopleService;
    private readonly List<PersonCreateRequest> _createRequests = new();
    private readonly List<(string Id, PersonUpdateRequest Request)> _updateRequests = new();
    private readonly List<string> _deleteIds = new();
    private readonly BatchOptions _options = new();

    public FluentBatchContext(IPeopleService peopleService)
    {
        _peopleService = peopleService ?? throw new ArgumentNullException(nameof(peopleService));
    }

    /// <summary>
    /// Adds a person creation request to the batch.
    /// </summary>
    public FluentBatchContext CreatePerson(PersonCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _createRequests.Add(request);
        return this;
    }

    /// <summary>
    /// Adds multiple person creation requests to the batch.
    /// </summary>
    public FluentBatchContext CreatePeople(IEnumerable<PersonCreateRequest> requests)
    {
        if (requests == null) throw new ArgumentNullException(nameof(requests));
        
        _createRequests.AddRange(requests);
        return this;
    }

    /// <summary>
    /// Adds a person update request to the batch.
    /// </summary>
    public FluentBatchContext UpdatePerson(string id, PersonUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _updateRequests.Add((id, request));
        return this;
    }

    /// <summary>
    /// Adds multiple person update requests to the batch.
    /// </summary>
    public FluentBatchContext UpdatePeople(IEnumerable<(string Id, PersonUpdateRequest Request)> requests)
    {
        if (requests == null) throw new ArgumentNullException(nameof(requests));
        
        _updateRequests.AddRange(requests);
        return this;
    }

    /// <summary>
    /// Adds a person deletion to the batch.
    /// </summary>
    public FluentBatchContext DeletePerson(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));
        
        _deleteIds.Add(id);
        return this;
    }

    /// <summary>
    /// Adds multiple person deletions to the batch.
    /// </summary>
    public FluentBatchContext DeletePeople(IEnumerable<string> ids)
    {
        if (ids == null) throw new ArgumentNullException(nameof(ids));
        
        _deleteIds.AddRange(ids.Where(id => !string.IsNullOrWhiteSpace(id)));
        return this;
    }

    /// <summary>
    /// Configures batch execution options.
    /// </summary>
    public FluentBatchContext WithOptions(Action<BatchOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        
        configure(_options);
        return this;
    }

    /// <summary>
    /// Sets the batch size for operations.
    /// </summary>
    public FluentBatchContext WithBatchSize(int batchSize)
    {
        if (batchSize < 1) throw new ArgumentException("Batch size must be greater than 0", nameof(batchSize));
        
        _options.BatchSize = batchSize;
        return this;
    }

    /// <summary>
    /// Sets whether to continue on errors.
    /// </summary>
    public FluentBatchContext ContinueOnError(bool continueOnError = true)
    {
        _options.ContinueOnError = continueOnError;
        return this;
    }

    /// <summary>
    /// Sets the maximum degree of parallelism.
    /// </summary>
    public FluentBatchContext WithParallelism(int maxDegreeOfParallelism)
    {
        if (maxDegreeOfParallelism < 1) throw new ArgumentException("Max degree of parallelism must be greater than 0", nameof(maxDegreeOfParallelism));
        
        _options.MaxDegreeOfParallelism = maxDegreeOfParallelism;
        return this;
    }

    /// <summary>
    /// Executes all batch operations.
    /// </summary>
    public async Task<BatchExecutionResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = new BatchExecutionResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Execute creates
            if (_createRequests.Any())
            {
                var createResults = await ExecuteCreatesAsync(cancellationToken);
                result.CreatedPeople.AddRange(createResults.SuccessfulResults);
                result.Errors.AddRange(createResults.Errors);
            }

            // Execute updates
            if (_updateRequests.Any())
            {
                var updateResults = await ExecuteUpdatesAsync(cancellationToken);
                result.UpdatedPeople.AddRange(updateResults.SuccessfulResults);
                result.Errors.AddRange(updateResults.Errors);
            }

            // Execute deletes
            if (_deleteIds.Any())
            {
                var deleteResults = await ExecuteDeletesAsync(cancellationToken);
                result.DeletedIds.AddRange(deleteResults.SuccessfulIds);
                result.Errors.AddRange(deleteResults.Errors);
            }

            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = !result.Errors.Any() || _options.ContinueOnError;

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = false;
            result.Errors.Add(new BatchOperationError
            {
                Operation = "Batch Execution",
                Error = ex.Message,
                Exception = ex
            });

            return result;
        }
    }

    private async Task<BatchCreateResult> ExecuteCreatesAsync(CancellationToken cancellationToken)
    {
        var result = new BatchCreateResult();
        var batches = _createRequests.Chunk(_options.BatchSize);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(batches, parallelOptions, async (batch, ct) =>
        {
            foreach (var request in batch)
            {
                try
                {
                    var person = await _peopleService.CreateAsync(request, ct);
                    lock (result.SuccessfulResults)
                    {
                        result.SuccessfulResults.Add(person);
                    }
                }
                catch (Exception ex)
                {
                    lock (result.Errors)
                    {
                        result.Errors.Add(new BatchOperationError
                        {
                            Operation = "Create Person",
                            Data = request,
                            Error = ex.Message,
                            Exception = ex
                        });
                    }

                    if (!_options.ContinueOnError)
                    {
                        throw;
                    }
                }
            }
        });

        return result;
    }

    private async Task<BatchUpdateResult> ExecuteUpdatesAsync(CancellationToken cancellationToken)
    {
        var result = new BatchUpdateResult();
        var batches = _updateRequests.Chunk(_options.BatchSize);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(batches, parallelOptions, async (batch, ct) =>
        {
            foreach (var (id, request) in batch)
            {
                try
                {
                    var person = await _peopleService.UpdateAsync(id, request, ct);
                    lock (result.SuccessfulResults)
                    {
                        result.SuccessfulResults.Add(person);
                    }
                }
                catch (Exception ex)
                {
                    lock (result.Errors)
                    {
                        result.Errors.Add(new BatchOperationError
                        {
                            Operation = "Update Person",
                            Data = new { Id = id, Request = request },
                            Error = ex.Message,
                            Exception = ex
                        });
                    }

                    if (!_options.ContinueOnError)
                    {
                        throw;
                    }
                }
            }
        });

        return result;
    }

    private async Task<BatchDeleteResult> ExecuteDeletesAsync(CancellationToken cancellationToken)
    {
        var result = new BatchDeleteResult();
        var batches = _deleteIds.Chunk(_options.BatchSize);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(batches, parallelOptions, async (batch, ct) =>
        {
            foreach (var id in batch)
            {
                try
                {
                    await _peopleService.DeleteAsync(id, ct);
                    lock (result.SuccessfulIds)
                    {
                        result.SuccessfulIds.Add(id);
                    }
                }
                catch (Exception ex)
                {
                    lock (result.Errors)
                    {
                        result.Errors.Add(new BatchOperationError
                        {
                            Operation = "Delete Person",
                            Data = id,
                            Error = ex.Message,
                            Exception = ex
                        });
                    }

                    if (!_options.ContinueOnError)
                    {
                        throw;
                    }
                }
            }
        });

        return result;
    }

    /// <summary>
    /// Gets information about the pending batch operations.
    /// </summary>
    public BatchInfo GetBatchInfo()
    {
        return new BatchInfo
        {
            CreateCount = _createRequests.Count,
            UpdateCount = _updateRequests.Count,
            DeleteCount = _deleteIds.Count,
            TotalOperations = _createRequests.Count + _updateRequests.Count + _deleteIds.Count,
            EstimatedBatches = CalculateEstimatedBatches(),
            Options = _options
        };
    }

    private int CalculateEstimatedBatches()
    {
        var totalOperations = _createRequests.Count + _updateRequests.Count + _deleteIds.Count;
        return (int)Math.Ceiling((double)totalOperations / _options.BatchSize);
    }
}

/// <summary>
/// Configuration options for batch operations.
/// </summary>
public class BatchOptions
{
    /// <summary>
    /// Number of operations to process in each batch.
    /// </summary>
    public int BatchSize { get; set; } = 10;

    /// <summary>
    /// Whether to continue processing if an error occurs.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Maximum number of concurrent operations.
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

/// <summary>
/// Information about a batch operation.
/// </summary>
public class BatchInfo
{
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public int TotalOperations { get; set; }
    public int EstimatedBatches { get; set; }
    public BatchOptions Options { get; set; } = new();

    public TimeSpan EstimatedExecutionTime => TimeSpan.FromSeconds(EstimatedBatches * 2); // Rough estimate
}

/// <summary>
/// Result of executing batch operations.
/// </summary>
public class BatchExecutionResult
{
    public List<Person> CreatedPeople { get; set; } = new();
    public List<Person> UpdatedPeople { get; set; } = new();
    public List<string> DeletedIds { get; set; } = new();
    public List<BatchOperationError> Errors { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public bool Success { get; set; }

    public int TotalSuccessfulOperations => CreatedPeople.Count + UpdatedPeople.Count + DeletedIds.Count;
    public int TotalFailedOperations => Errors.Count;
    public int TotalOperations => TotalSuccessfulOperations + TotalFailedOperations;

    public string GetSummary()
    {
        return $"Batch execution completed in {ExecutionTime.TotalSeconds:F2}s. " +
               $"Successful: {TotalSuccessfulOperations}, Failed: {TotalFailedOperations}, " +
               $"Success rate: {(double)TotalSuccessfulOperations / TotalOperations:P1}";
    }
}

/// <summary>
/// Represents an error that occurred during a batch operation.
/// </summary>
public class BatchOperationError
{
    public string Operation { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string Error { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}

// Internal result classes
internal class BatchCreateResult
{
    public List<Person> SuccessfulResults { get; set; } = new();
    public List<BatchOperationError> Errors { get; set; } = new();
}

internal class BatchUpdateResult
{
    public List<Person> SuccessfulResults { get; set; } = new();
    public List<BatchOperationError> Errors { get; set; } = new();
}

internal class BatchDeleteResult
{
    public List<string> SuccessfulIds { get; set; } = new();
    public List<BatchOperationError> Errors { get; set; } = new();
}