using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class PeopleServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly PeopleService _peopleService;
    private readonly TestDataBuilder _builder = new();

    public PeopleServiceTests()
    {
        _peopleService = new PeopleService(_mockApiConnection, NullLogger<PeopleService>.Instance);
    }

    [Fact]
    public async Task GetMeAsync_ShouldReturnCurrentUser_WhenApiReturnsData()
    {
        // Arrange
        var personDto = _builder.CreatePersonDto(p => p.Id = "42");
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        _mockApiConnection.SetupGetResponse("/people/v2/me", response);

        // Act
        var result = await _peopleService.GetMeAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("42");
        result.FirstName.Should().Be(personDto.Attributes.FirstName);
        result.LastName.Should().Be(personDto.Attributes.LastName);
    }
}
