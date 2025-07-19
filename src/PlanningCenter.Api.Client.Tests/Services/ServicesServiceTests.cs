using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for ServicesService.
/// Follows the same patterns as PeopleServiceTests.
/// </summary>
public class ServicesServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly ServicesService _servicesService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public ServicesServiceTests()
    {
        _servicesService = new ServicesService(_mockApiConnection, NullLogger<ServicesService>.Instance);
    }

    #region Plan Tests

    [Fact]
    public async Task GetPlanAsync_ShouldReturnPlan_WhenApiReturnsData()
    {
        // Arrange
        var planDto = _builder.CreatePlanDto(p => p.Id = "123");
        var response = new JsonApiSingleResponse<PlanDto> { Data = planDto };
        _mockApiConnection.SetupGetResponse("/services/v2/plans/123", response);

        // Act
        var result = await _servicesService.GetPlanAsync("123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("123");
        result!.Title.Should().Be(planDto.Attributes.Title);
        result!.DataSource.Should().Be("Services");
    }

    [Fact]
    public async Task GetPlanAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<PlanDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/services/v2/plans/nonexistent", response);

        // Act
        var result = await _servicesService.GetPlanAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPlanAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _servicesService.GetPlanAsync(""));
    }

    [Fact]
    public async Task ListPlansAsync_ShouldReturnPagedPlans_WhenApiReturnsData()
    {
        // Arrange
        var planDtos = new[]
        {
            _builder.CreatePlanDto(p => p.Id = "1"),
            _builder.CreatePlanDto(p => p.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/plans", planDtos);

        // Act
        var result = await _servicesService.ListPlansAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePlanAsync_ShouldReturnCreatedPlan_WhenValidRequest()
    {
        // Arrange
        var request = new PlanCreateRequest
        {
            Title = "Test Plan",
            ServiceTypeId = "service-type-123",
            Dates = "2024-12-25",
            IsPublic = true
        };

        var createdPlanDto = _builder.CreatePlanDto(p =>
        {
            p.Id = "new-plan-123";
            p.Attributes.Title = request.Title;
        });

        var response = new JsonApiSingleResponse<PlanDto> { Data = createdPlanDto };
        _mockApiConnection.SetupMutationResponse("POST", "/services/v2/plans", response);

        // Act
        var result = await _servicesService.CreatePlanAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("new-plan-123");
        result!.Title.Should().Be("Test Plan");
        result!.DataSource.Should().Be("Services");
    }

    [Fact]
    public async Task CreatePlanAsync_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new PlanCreateRequest
        {
            Title = "",
            ServiceTypeId = "service-type-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _servicesService.CreatePlanAsync(request));
    }

    [Fact]
    public async Task CreatePlanAsync_ShouldThrowArgumentException_WhenServiceTypeIdIsEmpty()
    {
        // Arrange
        var request = new PlanCreateRequest
        {
            Title = "Test Plan",
            ServiceTypeId = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _servicesService.CreatePlanAsync(request));
    }

    [Fact]
    public async Task UpdatePlanAsync_ShouldReturnUpdatedPlan_WhenValidRequest()
    {
        // Arrange
        var request = new PlanUpdateRequest
        {
            Title = "Updated Plan",
            IsPublic = false
        };

        var updatedPlanDto = _builder.CreatePlanDto(p =>
        {
            p.Id = "plan-123";
            p.Attributes.Title = "Updated Plan";
            p.Attributes.IsPublic = false;
        });

        var response = new JsonApiSingleResponse<PlanDto> { Data = updatedPlanDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/services/v2/plans/plan-123", response);

        // Act
        var result = await _servicesService.UpdatePlanAsync("plan-123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("plan-123");
        result!.Title.Should().Be("Updated Plan");
        result!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePlanAsync_ShouldCompleteSuccessfully_WhenValidId()
    {
        // Act & Assert (should not throw)
        await _servicesService.DeletePlanAsync("plan-123");
    }

    #endregion

    #region Song Tests

    [Fact]
    public async Task GetSongAsync_ShouldReturnSong_WhenApiReturnsData()
    {
        // Arrange
        var songDto = _builder.CreateSongDto(s => s.Id = "song-123");
        var response = new JsonApiSingleResponse<SongDto> { Data = songDto };
        _mockApiConnection.SetupGetResponse("/services/v2/songs/song-123", response);

        // Act
        var result = await _servicesService.GetSongAsync("song-123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("song-123");
        result!.Title.Should().Be(songDto.Attributes.Title);
        result!.DataSource.Should().Be("Services");
    }

    [Fact]
    public async Task ListSongsAsync_ShouldReturnPagedSongs_WhenApiReturnsData()
    {
        // Arrange
        var songDtos = new[]
        {
            _builder.CreateSongDto(s => s.Id = "1"),
            _builder.CreateSongDto(s => s.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/songs", songDtos);

        // Act
        var result = await _servicesService.ListSongsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task CreateSongAsync_ShouldReturnCreatedSong_WhenValidRequest()
    {
        // Arrange
        var request = new SongCreateRequest
        {
            Title = "Amazing Grace",
            Author = "John Newton",
            Copyright = "Public Domain",
            CcliNumber = "22025"
        };

        var createdSongDto = _builder.CreateSongDto(s =>
        {
            s.Id = "new-song-123";
            s.Attributes.Title = request.Title;
            s.Attributes.Author = request.Author;
        });

        var response = new JsonApiSingleResponse<SongDto> { Data = createdSongDto };
        _mockApiConnection.SetupMutationResponse("POST", "/services/v2/songs", response);

        // Act
        var result = await _servicesService.CreateSongAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("new-song-123");
        result!.Title.Should().Be("Amazing Grace");
        result!.Author.Should().Be("John Newton");
    }

    #endregion

    #region Service Type Tests

    [Fact]
    public async Task GetServiceTypeAsync_ShouldReturnServiceType_WhenApiReturnsData()
    {
        // Arrange
        var serviceTypeDto = _builder.CreateServiceTypeDto(st => st.Id = "service-type-123");
        var response = new JsonApiSingleResponse<ServiceTypeDto> { Data = serviceTypeDto };
        _mockApiConnection.SetupGetResponse("/services/v2/service_types/service-type-123", response);

        // Act
        var result = await _servicesService.GetServiceTypeAsync("service-type-123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("service-type-123");
        result!.Name.Should().Be(serviceTypeDto.Attributes.Name);
        result!.DataSource.Should().Be("Services");
    }

    [Fact]
    public async Task ListServiceTypesAsync_ShouldReturnPagedServiceTypes_WhenApiReturnsData()
    {
        // Arrange
        var serviceTypeDtos = new[]
        {
            _builder.CreateServiceTypeDto(st => st.Id = "1"),
            _builder.CreateServiceTypeDto(st => st.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/service_types", serviceTypeDtos);

        // Act
        var result = await _servicesService.ListServiceTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    #endregion

    #region Item Tests

    [Fact]
    public async Task GetPlanItemAsync_ShouldReturnItem_WhenApiReturnsData()
    {
        // Arrange
        var itemDto = _builder.CreateItemDto(i => i.Id = "item-123");
        var response = new JsonApiSingleResponse<ItemDto> { Data = itemDto };
        _mockApiConnection.SetupGetResponse("/services/v2/plans/plan-123/items/item-123", response);

        // Act
        var result = await _servicesService.GetPlanItemAsync("plan-123", "item-123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("item-123");
        result!.Title.Should().Be(itemDto.Attributes.Title);
        result!.DataSource.Should().Be("Services");
    }

    [Fact]
    public async Task ListPlanItemsAsync_ShouldReturnPagedItems_WhenApiReturnsData()
    {
        // Arrange
        var itemDtos = new[]
        {
            _builder.CreateItemDto(i => i.Id = "1"),
            _builder.CreateItemDto(i => i.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/plans/plan-123/items", itemDtos);

        // Act
        var result = await _servicesService.ListPlanItemsAsync("plan-123");

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task CreatePlanItemAsync_ShouldReturnCreatedItem_WhenValidRequest()
    {
        // Arrange
        var request = new ItemCreateRequest
        {
            Title = "Opening Song",
            Sequence = 1,
            ItemType = "song",
            SongId = "song-123"
        };

        var createdItemDto = _builder.CreateItemDto(i =>
        {
            i.Id = "new-item-123";
            i.Attributes.Title = request.Title;
            i.Attributes.Sequence = request.Sequence;
        });

        var response = new JsonApiSingleResponse<ItemDto> { Data = createdItemDto };
        _mockApiConnection.SetupMutationResponse("POST", "/services/v2/plans/plan-123/items", response);

        // Act
        var result = await _servicesService.CreatePlanItemAsync("plan-123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("new-item-123");
        result!.Title.Should().Be("Opening Song");
        result!.Sequence.Should().Be(1);
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task GetAllPlansAsync_ShouldReturnAllPlans_WhenMultiplePagesExist()
    {
        // Arrange
        var planDtos = new[]
        {
            _builder.CreatePlanDto(p => p.Id = "1"),
            _builder.CreatePlanDto(p => p.Id = "2"),
            _builder.CreatePlanDto(p => p.Id = "3")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/plans", planDtos);

        // Act
        var result = await _servicesService.GetAllPlansAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(3);
        result[0].Id.Should().Be("1");
        result[1].Id.Should().Be("2");
        result[2].Id.Should().Be("3");
    }

    [Fact]
    public async Task StreamPlansAsync_ShouldYieldPlans_WhenDataExists()
    {
        // Arrange
        var planDtos = new[]
        {
            _builder.CreatePlanDto(p => p.Id = "1"),
            _builder.CreatePlanDto(p => p.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/services/v2/plans", planDtos);

        // Act
        var plans = new List<Models.Services.Plan>();
        await foreach (var plan in _servicesService.StreamPlansAsync())
        {
            plans.Add(plan);
        }

        // Assert
        plans.Should().HaveCount(2);
        plans[0].Id.Should().Be("1");
        plans[1].Id.Should().Be("2");
    }

    #endregion
}