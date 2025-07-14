using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class ContactInfoServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly PeopleService _peopleService;
    private readonly TestDataBuilder _builder = new();

    public ContactInfoServiceTests()
    {
        _peopleService = new PeopleService(_mockApiConnection, NullLogger<PeopleService>.Instance);
    }

    [Fact]
    public async Task AddPhoneNumberAsync_ShouldReturnCreatedPhoneNumber_WhenRequestIsValid()
    {
        // Arrange
        var personId = "123";
        var request = new PhoneNumberCreateRequest { Number = "+1-555-000-0000", Location = "Mobile", IsPrimary = true };

        var phoneNumberDto = new PhoneNumberDto
        {
            Id = "999",
            Attributes = new PhoneNumberAttributesDto
            {
                Number = request.Number,
                Location = request.Location,
                Primary = request.IsPrimary,
                Carrier = true
            }
        };
        var response = new JsonApiSingleResponse<PhoneNumberDto> { Data = phoneNumberDto };
        _mockApiConnection.SetupMutationResponse("POST", $"/people/v2/people/{personId}/phone_numbers", response);

        // Act
        var result = await _peopleService.AddPhoneNumberAsync(personId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("999");
        result.Number.Should().Be(request.Number);
        result.Location.Should().Be(request.Location);
    }
}
