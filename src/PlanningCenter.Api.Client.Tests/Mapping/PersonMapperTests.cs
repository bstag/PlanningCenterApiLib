using FluentAssertions;
using PlanningCenter.Api.Client.Mapping.People;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Mapping;

public class PersonMapperTests
{
    private readonly TestDataBuilder _builder = new();

    [Fact]
    public void MapToDomain_ShouldCopyBasicFields()
    {
        // Arrange
        var dto = _builder.CreatePersonDto(p =>
        {
            p.Id = "42";
            p.Attributes.FirstName = "Ada";
            p.Attributes.LastName = "Lovelace";
        });

        // Act
        var model = PersonMapper.MapToDomain(dto);

        // Assert
        model.Id.Should().Be("42");
        model.FirstName.Should().Be("Ada");
        model.LastName.Should().Be("Lovelace");
    }
}
