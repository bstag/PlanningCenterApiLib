using FluentAssertions;
using PlanningCenter.Api.Client.Mapping.Services;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Mapping;

/// <summary>
/// Unit tests for Services module mappers.
/// Follows the same patterns as PersonMapperTests.
/// </summary>
public class ServicesMapperTests
{
    private readonly ExtendedTestDataBuilder _builder = new();

    #region Plan Mapper Tests

    [Fact]
    public void MapToDomain_ShouldMapPlanDto_WhenValidDto()
    {
        // Arrange
        var dto = _builder.CreatePlanDto(p =>
        {
            p.Id = "plan-123";
            p.Attributes.Title = "Sunday Service";
            p.Attributes.Dates = "2024-12-25";
            p.Attributes.IsPublic = true;
        });

        // Act
        var result = PlanMapper.MapToDomain(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("plan-123");
        result.Title.Should().Be("Sunday Service");
        result.Dates.Should().Be("2024-12-25");
        result.IsPublic.Should().BeTrue();
        result.DataSource.Should().Be("Services");
    }

    [Fact]
    public void MapToDomain_ShouldThrowArgumentNullException_WhenDtoIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => PlanMapper.MapToDomain(null!));
    }

    [Fact]
    public void MapCreateRequestToJsonApi_ShouldMapPlanCreateRequest_WhenValidRequest()
    {
        // Arrange
        var request = new PlanCreateRequest
        {
            Title = "Easter Service",
            ServiceTypeId = "service-type-123",
            Dates = "2024-03-31",
            IsPublic = true,
            Notes = "Special Easter celebration"
        };

        // Act
        var result = PlanMapper.MapCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Plan");
        result.Data.Attributes.Title.Should().Be("Easter Service");
        result.Data.Attributes.Dates.Should().Be("2024-03-31");
        result.Data.Attributes.IsPublic.Should().BeTrue();
        result.Data.Attributes.Notes.Should().Be("Special Easter celebration");
        result.Data.Relationships.Should().NotBeNull();
        result.Data.Relationships!.ServiceType.Should().NotBeNull();
        result.Data.Relationships!.ServiceType!.Id.Should().Be("service-type-123");
    }

    [Fact]
    public void MapCreateRequestToJsonApi_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => PlanMapper.MapCreateRequestToJsonApi(null!));
    }

    [Fact]
    public void MapUpdateRequestToJsonApi_ShouldMapPlanUpdateRequest_WhenValidRequest()
    {
        // Arrange
        var request = new PlanUpdateRequest
        {
            Title = "Updated Service",
            IsPublic = false,
            Notes = "Updated notes"
        };

        // Act
        var result = PlanMapper.MapUpdateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Plan");
        result.Data.Attributes.Title.Should().Be("Updated Service");
        result.Data.Attributes.IsPublic.Should().BeFalse();
        result.Data.Attributes.Notes.Should().Be("Updated notes");
    }

    #endregion

    #region Song Mapper Tests

    [Fact]
    public void MapSongToDomain_ShouldMapSongDto_WhenValidDto()
    {
        // Arrange
        var dto = _builder.CreateSongDto(s =>
        {
            s.Id = "song-123";
            s.Attributes.Title = "Amazing Grace";
            s.Attributes.Author = "John Newton";
            s.Attributes.Copyright = "Public Domain";
            s.Attributes.CcliNumber = "22025";
            s.Attributes.Hidden = false;
        });

        // Act
        var result = PlanMapper.MapSongToDomain(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("song-123");
        result.Title.Should().Be("Amazing Grace");
        result.Author.Should().Be("John Newton");
        result.Copyright.Should().Be("Public Domain");
        result.CcliNumber.Should().Be("22025");
        result.Hidden.Should().BeFalse();
        result.DataSource.Should().Be("Services");
    }

    [Fact]
    public void MapSongCreateRequestToJsonApi_ShouldMapSongCreateRequest_WhenValidRequest()
    {
        // Arrange
        var request = new SongCreateRequest
        {
            Title = "How Great Thou Art",
            Author = "Carl Boberg",
            Copyright = "Public Domain",
            CcliNumber = "14181",
            Hidden = false,
            Themes = "Worship, Praise"
        };

        // Act
        var result = PlanMapper.MapSongCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Song");
        result.Data.Attributes.Title.Should().Be("How Great Thou Art");
        result.Data.Attributes.Author.Should().Be("Carl Boberg");
        result.Data.Attributes.Copyright.Should().Be("Public Domain");
        result.Data.Attributes.CcliNumber.Should().Be("14181");
        result.Data.Attributes.Hidden.Should().BeFalse();
        result.Data.Attributes.Themes.Should().Be("Worship, Praise");
    }

    #endregion

    #region Item Mapper Tests

    [Fact]
    public void MapItemToDomain_ShouldMapItemDto_WhenValidDto()
    {
        // Arrange
        var dto = _builder.CreateItemDto(i =>
        {
            i.Id = "item-123";
            i.Attributes.Title = "Opening Song";
            i.Attributes.Sequence = 1;
            i.Attributes.ItemType = "song";
            i.Attributes.Length = 4;
        });

        // Act
        var result = PlanMapper.MapItemToDomain(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("item-123");
        result.Title.Should().Be("Opening Song");
        result.Sequence.Should().Be(1);
        result.ItemType.Should().Be("song");
        result.Length.Should().Be(4);
        result.DataSource.Should().Be("Services");
    }

    [Fact]
    public void MapItemCreateRequestToJsonApi_ShouldMapItemCreateRequest_WhenValidRequest()
    {
        // Arrange
        var request = new ItemCreateRequest
        {
            Title = "Closing Song",
            Sequence = 10,
            ItemType = "song",
            Description = "Final worship song",
            SongId = "song-456",
            Length = 5
        };

        // Act
        var result = PlanMapper.MapItemCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Item");
        result.Data.Attributes.Title.Should().Be("Closing Song");
        result.Data.Attributes.Sequence.Should().Be(10);
        result.Data.Attributes.ItemType.Should().Be("song");
        result.Data.Attributes.Description.Should().Be("Final worship song");
        result.Data.Attributes.Length.Should().Be(5);
        result.Data.Relationships.Should().NotBeNull();
        result.Data.Relationships!.Song.Should().NotBeNull();
        result.Data.Relationships!.Song!.Id.Should().Be("song-456");
    }

    #endregion

    #region Service Type Mapper Tests

    [Fact]
    public void MapServiceTypeToDomain_ShouldMapServiceTypeDto_WhenValidDto()
    {
        // Arrange
        var dto = _builder.CreateServiceTypeDto(st =>
        {
            st.Id = "service-type-123";
            st.Attributes.Name = "Sunday Morning";
            st.Attributes.Sequence = 1;
        });

        // Act
        var result = PlanMapper.MapServiceTypeToDomain(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("service-type-123");
        result.Name.Should().Be("Sunday Morning");
        result.Sequence.Should().Be(1);
        result.DataSource.Should().Be("Services");
    }

    #endregion
}