using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Caching;

/// <summary>
/// Unit tests for InMemoryCacheProvider - in-memory caching implementation.
/// This tests the caching functionality used throughout the SDK.
/// </summary>
public class InMemoryCacheProviderTests : IDisposable
{
    private readonly InMemoryCacheProvider _cacheProvider;
    private readonly PlanningCenterOptions _options;

    public InMemoryCacheProviderTests()
    {
        _options = new PlanningCenterOptions
        {
            EnableCaching = true,
            DefaultCacheExpiration = TimeSpan.FromMinutes(5),
            MaxCacheSize = 1000
        };

        var optionsWrapper = Options.Create(_options);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cacheProvider = new InMemoryCacheProvider(memoryCache, optionsWrapper, NullLogger<InMemoryCacheProvider>.Instance);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var optionsWrapper = Options.Create(_options);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var provider = new InMemoryCacheProvider(memoryCache, optionsWrapper, NullLogger<InMemoryCacheProvider>.Instance);

        // Assert
        provider.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var act = () => new InMemoryCacheProvider(memoryCache, null!, NullLogger<InMemoryCacheProvider>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var act = () => new InMemoryCacheProvider(memoryCache, optionsWrapper, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var key = "non-existent-key";

        // Act
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithExistingKey_ShouldReturnCachedValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMinutes(1));

        // Act
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task GetAsync_WithExpiredKey_ShouldReturnNull()
    {
        // Arrange
        var key = "expired-key";
        var value = "test-value";
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMilliseconds(10));

        // Wait for expiration
        await Task.Delay(50);

        // Act
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithComplexObject_ShouldReturnCorrectObject()
    {
        // Arrange
        var key = "complex-object";
        var value = new TestObject { Id = 123, Name = "Test", Items = ["A", "B", "C"] };
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMinutes(1));

        // Act
        var result = await _cacheProvider.GetAsync<TestObject>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(123);
        result.Name.Should().Be("Test");
        result.Items.Should().BeEquivalentTo(["A", "B", "C"]);
    }

    [Fact]
    public async Task GetAsync_WithNullKey_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.GetAsync<string>(null!);
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
    }

    [Fact]
    public async Task GetAsync_WithEmptyKey_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.GetAsync<string>("");
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
    }

    #endregion

    #region SetAsync Tests

    [Fact]
    public async Task SetAsync_WithValidKeyAndValue_ShouldStoreValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(1);

        // Act
        await _cacheProvider.SetAsync(key, value, expiration);
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task SetAsync_WithNullValue_ShouldStoreNull()
    {
        // Arrange
        var key = "null-value-key";
        string? value = null;

        // Act
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMinutes(1));
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithZeroExpiration_ShouldExpireImmediately()
    {
        // Arrange
        var key = "zero-expiration";
        var value = "test-value";

        // Act
        await _cacheProvider.SetAsync(key, value, TimeSpan.Zero);
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithNegativeExpiration_ShouldExpireImmediately()
    {
        // Arrange
        var key = "negative-expiration";
        var value = "test-value";

        // Act
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMinutes(-1));
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_OverwriteExistingKey_ShouldUpdateValue()
    {
        // Arrange
        var key = "overwrite-key";
        var originalValue = "original-value";
        var newValue = "new-value";

        // Act
        await _cacheProvider.SetAsync(key, originalValue, TimeSpan.FromMinutes(1));
        await _cacheProvider.SetAsync(key, newValue, TimeSpan.FromMinutes(1));
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().Be(newValue);
    }

    [Fact]
    public async Task SetAsync_WithNullKey_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.SetAsync<string>(null!, "value", TimeSpan.FromMinutes(1));
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
    }

    [Fact]
    public async Task SetAsync_WithEmptyKey_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.SetAsync("", "value", TimeSpan.FromMinutes(1));
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
    }

    #endregion

    #region RemoveAsync Tests

    [Fact]
    public async Task RemoveAsync_WithExistingKey_ShouldRemoveValue()
    {
        // Arrange
        var key = "remove-key";
        var value = "test-value";
        await _cacheProvider.SetAsync(key, value, TimeSpan.FromMinutes(1));

        // Act
        await _cacheProvider.RemoveAsync(key);
        var result = await _cacheProvider.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_WithNonExistentKey_ShouldNotThrow()
    {
        // Arrange
        var key = "non-existent-key";

        // Act & Assert
        var act = async () => await _cacheProvider.RemoveAsync(key);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_WithNullKey_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.RemoveAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
    }

    [Fact]
    public async Task RemoveAsync_WithEmptyKey_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.RemoveAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
    }

    #endregion

    #region ClearAsync Tests

    [Fact]
    public async Task ClearAsync_WithMultipleItems_ShouldRemoveAllItems()
    {
        // Arrange
        await _cacheProvider.SetAsync("key1", "value1", TimeSpan.FromMinutes(1));
        await _cacheProvider.SetAsync("key2", "value2", TimeSpan.FromMinutes(1));
        await _cacheProvider.SetAsync("key3", "value3", TimeSpan.FromMinutes(1));

        // Act
        await _cacheProvider.ClearAsync();

        // Assert
        var result1 = await _cacheProvider.GetAsync<string>("key1");
        var result2 = await _cacheProvider.GetAsync<string>("key2");
        var result3 = await _cacheProvider.GetAsync<string>("key3");

        result1.Should().BeNull();
        result2.Should().BeNull();
        result3.Should().BeNull();
    }

    [Fact]
    public async Task ClearAsync_WithEmptyCache_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var act = async () => await _cacheProvider.ClearAsync();
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Cache Size Management Tests

    [Fact]
    public async Task SetAsync_ExceedingMaxCacheSize_ShouldEvictOldestItems()
    {
        // Arrange
        var smallCacheOptions = new PlanningCenterOptions
        {
            EnableCaching = true,
            MaxCacheSize = 3
        };
        var optionsWrapper = Options.Create(smallCacheOptions);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        using var smallCache = new InMemoryCacheProvider(memoryCache, optionsWrapper, NullLogger<InMemoryCacheProvider>.Instance);

        // Act - Add items beyond cache size
        await smallCache.SetAsync("key1", "value1", TimeSpan.FromMinutes(1));
        await smallCache.SetAsync("key2", "value2", TimeSpan.FromMinutes(1));
        await smallCache.SetAsync("key3", "value3", TimeSpan.FromMinutes(1));
        await smallCache.SetAsync("key4", "value4", TimeSpan.FromMinutes(1)); // Should evict key1

        // Assert
        var result1 = await smallCache.GetAsync<string>("key1"); // Should be evicted
        var result2 = await smallCache.GetAsync<string>("key2");
        var result3 = await smallCache.GetAsync<string>("key3");
        var result4 = await smallCache.GetAsync<string>("key4");

        result1.Should().BeNull();
        result2.Should().Be("value2");
        result3.Should().Be("value3");
        result4.Should().Be("value4");
    }

    #endregion

    #region Caching Disabled Tests

    [Fact]
    public async Task GetAsync_WithCachingDisabled_ShouldAlwaysReturnNull()
    {
        // Arrange
        var disabledCacheOptions = new PlanningCenterOptions
        {
            EnableCaching = false
        };
        var optionsWrapper = Options.Create(disabledCacheOptions);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        using var disabledCache = new InMemoryCacheProvider(memoryCache, optionsWrapper, NullLogger<InMemoryCacheProvider>.Instance);

        await disabledCache.SetAsync("key", "value", TimeSpan.FromMinutes(1));

        // Act
        var result = await disabledCache.GetAsync<string>("key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithCachingDisabled_ShouldNotStoreValue()
    {
        // Arrange
        var disabledCacheOptions = new PlanningCenterOptions
        {
            EnableCaching = false
        };
        var optionsWrapper = Options.Create(disabledCacheOptions);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        using var disabledCache = new InMemoryCacheProvider(memoryCache, optionsWrapper, NullLogger<InMemoryCacheProvider>.Instance);

        // Act
        await disabledCache.SetAsync("key", "value", TimeSpan.FromMinutes(1));
        var result = await disabledCache.GetAsync<string>("key");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task GetAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _cacheProvider.GetAsync<string>("key", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SetAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _cacheProvider.SetAsync("key", "value", TimeSpan.FromMinutes(1), cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task RemoveAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _cacheProvider.RemoveAsync("key", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ClearAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _cacheProvider.ClearAsync(cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Test Helper Classes

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string[] Items { get; set; } = [];
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        _cacheProvider?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}