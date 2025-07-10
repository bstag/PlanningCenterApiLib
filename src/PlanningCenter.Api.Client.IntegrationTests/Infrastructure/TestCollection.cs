using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Infrastructure;

/// <summary>
/// Collection definition for integration tests to share the test fixture.
/// </summary>
[CollectionDefinition("Integration Tests")]
public class TestCollection : ICollectionFixture<TestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
