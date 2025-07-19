using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class PublishingServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly PublishingService _publishingService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public PublishingServiceTests()
    {
        _publishingService = new PublishingService(_mockApiConnection, NullLogger<PublishingService>.Instance);
    }

    #region Episode Tests

    [Fact]
    public async Task GetEpisodeAsync_ShouldReturnEpisode_WhenApiReturnsData()
    {
        // Arrange
        var episodeDto = _builder.CreateEpisodeDto(e => e.Id = "episode123");
        var response = new JsonApiSingleResponse<EpisodeDto> { Data = episodeDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes/episode123", response);

        // Act
        var result = await _publishingService.GetEpisodeAsync("episode123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("episode123");
        result!.Title.Should().Be(episodeDto.Attributes.Title);
        result!.DataSource.Should().Be("Publishing");
    }


    [Fact]
    public async Task GetEpisodeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.GetEpisodeAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    [Fact]
    public async Task ListEpisodesAsync_ShouldReturnPagedEpisodes_WhenApiReturnsData()
    {
        // Arrange
        var episodesResponse = _builder.BuildEpisodeCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", episodesResponse);

        // Act
        var result = await _publishingService.ListEpisodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateEpisodeAsync_ShouldReturnCreatedEpisode_WhenRequestIsValid()
    {
        // Arrange
        var request = new EpisodeCreateRequest
        {
            Title = "Test Episode",
            Description = "A test episode",
            SeriesId = "series123"
        };

        var episodeDto = _builder.CreateEpisodeDto(e => 
        {
            e.Id = "newepisode123";
            e.Attributes.Title = "Test Episode";
            e.Attributes.Description = "A test episode";
        });

        var response = new JsonApiSingleResponse<EpisodeDto> { Data = episodeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/episodes", response);

        // Act
        var result = await _publishingService.CreateEpisodeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newepisode123");
        result!.Title.Should().Be("Test Episode");
        result!.Description.Should().Be("A test episode");
    }

    [Fact]
    public async Task CreateEpisodeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.CreateEpisodeAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateEpisodeAsync_ShouldReturnUpdatedEpisode_WhenRequestIsValid()
    {
        // Arrange
        var request = new EpisodeUpdateRequest
        {
            Title = "Updated Episode",
            Description = "Updated description"
        };

        var episodeDto = _builder.CreateEpisodeDto(e => 
        {
            e.Id = "episode123";
            e.Attributes.Title = "Updated Episode";
            e.Attributes.Description = "Updated description";
        });

        var response = new JsonApiSingleResponse<EpisodeDto> { Data = episodeDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/publishing/v2/episodes/episode123", response);

        // Act
        var result = await _publishingService.UpdateEpisodeAsync("episode123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("episode123");
        result!.Title.Should().Be("Updated Episode");
        result!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteEpisodeAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteEpisodeAsync("episode123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Publishing Tests

    [Fact]
    public async Task PublishEpisodeAsync_ShouldReturnPublishedEpisode_WhenIdIsValid()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic> 
        { 
            Data = new { id = "episode123", type = "Episode" } 
        };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/episodes/episode123/publish", response);

        // Act
        var result = await _publishingService.PublishEpisodeAsync("episode123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("episode123");
        result!.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task UnpublishEpisodeAsync_ShouldReturnUnpublishedEpisode_WhenIdIsValid()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic> 
        { 
            Data = new { id = "episode123", type = "Episode" } 
        };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/episodes/episode123/unpublish", response);

        // Act
        var result = await _publishingService.UnpublishEpisodeAsync("episode123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("episode123");
        result!.IsPublished.Should().BeFalse();
    }

    #endregion

    #region Speaker Tests

    [Fact]
    public async Task GetSpeakerAsync_ShouldReturnSpeaker_WhenApiReturnsData()
    {
        // Arrange
        var speakerDto = _builder.CreateSpeakerDto(s => s.Id = "speaker123");
        var response = new JsonApiSingleResponse<dynamic> { Data = speakerDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/speakers/speaker123", response);

        // Act
        var result = await _publishingService.GetSpeakerAsync("speaker123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("speaker123");
        result!.DataSource.Should().Be("Publishing");
    }




    [Fact]
    public async Task DeleteSpeakerAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSpeakerAsync("speaker123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Media Tests

    [Fact]
    public async Task GetMediaAsync_ShouldReturnMedia_WhenApiReturnsData()
    {
        // Arrange
        var mediaDto = _builder.CreateMediaDto(m => m.Id = "media123");
        var response = new JsonApiSingleResponse<dynamic> { Data = mediaDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/media/media123", response);

        // Act
        var result = await _publishingService.GetMediaAsync("media123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("media123");
        result!.DataSource.Should().Be("Publishing");
    }




    [Fact]
    public async Task DeleteMediaAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteMediaAsync("media123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Pagination Helper Tests



    #endregion

    #region Not Implemented Method Tests

    [Fact]
    public async Task CreateSeriesAsync_ShouldThrowNotImplementedException()
    {
        // Arrange
        var request = new SeriesCreateRequest { Title = "Test Series" };

        // Act & Assert
        await _publishingService.Invoking(s => s.CreateSeriesAsync(request))
            .Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public async Task UpdateSeriesAsync_ShouldThrowNotImplementedException()
    {
        // Arrange
        var request = new SeriesUpdateRequest { Title = "Updated Series" };

        // Act & Assert
        await _publishingService.Invoking(s => s.UpdateSeriesAsync("series123", request))
            .Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public async Task DeleteSeriesAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSeriesAsync("series123"))
            .Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public async Task ListSpeakershipsAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.ListSpeakershipsAsync("episode123"))
            .Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.AddSpeakerToEpisodeAsync("episode123", "speaker123"))
            .Should().ThrowAsync<NotImplementedException>();
    }

    #endregion
}