using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class PublishingServiceTests
{
    private readonly MockApiConnection _mockApiConnection;
    private readonly PublishingService _publishingService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public PublishingServiceTests()
    {
        _mockApiConnection = new MockApiConnection();
        _publishingService = new PublishingService(_mockApiConnection, NullLogger<PublishingService>.Instance);
    }

    /// <summary>
    /// Helper method to test that a service method calls the correct endpoint
    /// by verifying it throws the expected "No stub configured" exception.
    /// </summary>
    private async Task AssertCallsEndpoint(Func<Task> serviceCall, string expectedEndpoint, string httpMethod = "GET")
    {
        var exception = await Record.ExceptionAsync(serviceCall);
        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain($"No {httpMethod} stub configured for {expectedEndpoint}");
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
    public async Task GetEpisodeAsync_ShouldReturnNull_WhenEpisodeNotFound()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse<JsonApiSingleResponse<EpisodeDto>>("/publishing/v2/episodes/nonexistent", new JsonApiSingleResponse<EpisodeDto>());

        // Act
        var result = await _publishingService.GetEpisodeAsync("nonexistent");

        // Assert
        result.Should().BeNull();
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
    public async Task ListEpisodesAsync_ShouldReturnEmptyList_WhenNoEpisodesExist()
    {
        // Arrange
        var emptyResponse = _builder.BuildEpisodeCollectionResponse(0);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", emptyResponse);

        // Act
        var result = await _publishingService.ListEpisodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Meta.TotalCount.Should().Be(0);
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
    public async Task UpdateEpisodeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Arrange
        var request = new EpisodeUpdateRequest { Title = "Test" };

        // Act & Assert
        await _publishingService.Invoking(s => s.UpdateEpisodeAsync("", request))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    [Fact]
    public async Task UpdateEpisodeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.UpdateEpisodeAsync("episode123", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteEpisodeAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteEpisodeAsync("episode123"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteEpisodeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteEpisodeAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    #endregion

    #region Episode Publishing Tests

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
    public async Task PublishEpisodeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.PublishEpisodeAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
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

    [Fact]
    public async Task UnpublishEpisodeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.UnpublishEpisodeAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    #endregion

    #region Speaker Tests

    [Fact]
    public async Task GetSpeakerAsync_ShouldReturnSpeaker_WhenApiReturnsData()
    {
        // Arrange
        var speakerDto = _builder.CreateSpeakerDto(s => s.Id = "speaker123");
        var response = new JsonApiSingleResponse<SpeakerDto> { Data = speakerDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/speakers/speaker123", response);

        // Act
        var result = await _publishingService.GetSpeakerAsync("speaker123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("speaker123");
        result!.FirstName.Should().Be(speakerDto.Attributes.FirstName);
        result!.LastName.Should().Be(speakerDto.Attributes.LastName);
        result!.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public async Task GetSpeakerAsync_ShouldReturnNull_WhenSpeakerNotFound()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SpeakerDto>>("/publishing/v2/speakers/nonexistent", new JsonApiSingleResponse<SpeakerDto>());

        // Act
        var result = await _publishingService.GetSpeakerAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSpeakerAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.GetSpeakerAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Speaker ID cannot be null or empty*");
    }

    [Fact]
    public async Task ListSpeakersAsync_ShouldReturnPagedSpeakers_WhenApiReturnsData()
    {
        // Arrange
        var speakersResponse = _builder.BuildSpeakerCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/publishing/v2/speakers", speakersResponse);

        // Act
        var result = await _publishingService.ListSpeakersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateSpeakerAsync_ShouldReturnCreatedSpeaker_WhenRequestIsValid()
    {
        // Arrange
        var request = new SpeakerCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Active = true
        };

        var speakerDto = _builder.CreateSpeakerDto(s => 
        {
            s.Id = "newspeaker123";
            s.Attributes.FirstName = "John";
            s.Attributes.LastName = "Doe";
            s.Attributes.Email = "john.doe@example.com";
        });

        var response = new JsonApiSingleResponse<SpeakerDto> { Data = speakerDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/speakers", response);

        // Act
        var result = await _publishingService.CreateSpeakerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newspeaker123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task CreateSpeakerAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.CreateSpeakerAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateSpeakerAsync_ShouldReturnUpdatedSpeaker_WhenRequestIsValid()
    {
        // Arrange
        var request = new SpeakerUpdateRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com"
        };

        var speakerDto = _builder.CreateSpeakerDto(s => 
        {
            s.Id = "speaker123";
            s.Attributes.FirstName = "Jane";
            s.Attributes.LastName = "Smith";
            s.Attributes.Email = "jane.smith@example.com";
        });

        var response = new JsonApiSingleResponse<SpeakerDto> { Data = speakerDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/publishing/v2/speakers/speaker123", response);

        // Act
        var result = await _publishingService.UpdateSpeakerAsync("speaker123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("speaker123");
        result!.FirstName.Should().Be("Jane");
        result!.LastName.Should().Be("Smith");
        result!.Email.Should().Be("jane.smith@example.com");
    }

    [Fact]
    public async Task UpdateSpeakerAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Arrange
        var request = new SpeakerUpdateRequest { FirstName = "Test" };

        // Act & Assert
        await _publishingService.Invoking(s => s.UpdateSpeakerAsync("", request))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Speaker ID cannot be null or empty*");
    }

    [Fact]
    public async Task UpdateSpeakerAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.UpdateSpeakerAsync("speaker123", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteSpeakerAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSpeakerAsync("speaker123"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteSpeakerAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSpeakerAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Speaker ID cannot be null or empty*");
    }

    #endregion

    #region Series Tests

    [Fact]
    public async Task GetSeriesAsync_ShouldReturnSeries_WhenApiReturnsData()
    {
        // Arrange
        var seriesDto = _builder.CreateSeriesDto(s => s.Id = "series123");
        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/series/series123", response);

        // Act
        var result = await _publishingService.GetSeriesAsync("series123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("series123");
        result!.Title.Should().Be(seriesDto.Attributes.Title);
        result!.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public async Task GetSeriesAsync_ShouldReturnNull_WhenSeriesNotFound()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SeriesDto>>("/publishing/v2/series/nonexistent", new JsonApiSingleResponse<SeriesDto> { Data = null });

        // Act
        var result = await _publishingService.GetSeriesAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListSeriesAsync_ShouldReturnPagedSeries_WhenApiReturnsData()
    {
        // Arrange
        var seriesResponse = _builder.BuildSeriesCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/publishing/v2/series", seriesResponse);

        // Act
        var result = await _publishingService.ListSeriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateSeriesAsync_ShouldReturnCreatedSeries_WhenRequestIsValid()
    {
        // Arrange
        var request = new SeriesCreateRequest
        {
            Title = "Test Series",
            Description = "A test series"
        };

        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = "newseries123";
            s.Attributes.Title = "Test Series";
            s.Attributes.Description = "A test series";
        });

        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/series", response);

        // Act
        var result = await _publishingService.CreateSeriesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newseries123");
        result!.Title.Should().Be("Test Series");
        result!.Description.Should().Be("A test series");
    }

    [Fact]
    public async Task UpdateSeriesAsync_ShouldReturnUpdatedSeries_WhenRequestIsValid()
    {
        // Arrange
        var request = new SeriesUpdateRequest
        {
            Title = "Updated Series",
            Description = "Updated description"
        };

        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = "series123";
            s.Attributes.Title = "Updated Series";
            s.Attributes.Description = "Updated description";
        });

        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/publishing/v2/series/series123", response);

        // Act
        var result = await _publishingService.UpdateSeriesAsync("series123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("series123");
        result!.Title.Should().Be("Updated Series");
        result!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteSeriesAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSeriesAsync("series123"))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task PublishSeriesAsync_ShouldReturnPublishedSeries_WhenIdIsValid()
    {
        // Arrange
        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = "series123";
            s.Attributes.IsPublished = true;
        });
        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/series/series123/publish", response);

        // Act
        var result = await _publishingService.PublishSeriesAsync("series123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("series123");
        result!.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task UnpublishSeriesAsync_ShouldReturnUnpublishedSeries_WhenIdIsValid()
    {
        // Arrange
        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = "series123";
            s.Attributes.IsPublished = false;
        });
        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/series/series123/unpublish", response);

        // Act
        var result = await _publishingService.UnpublishSeriesAsync("series123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("series123");
        result!.IsPublished.Should().BeFalse();
    }

    #endregion

    #region Speakership Tests

    [Fact]
    public async Task ListSpeakershipsAsync_ShouldReturnPagedSpeakerships_WhenApiReturnsData()
    {
        // Arrange
        var speakershipsResponse = _builder.BuildSpeakershipCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes/episode123/speakerships", speakershipsResponse);

        // Act
        var result = await _publishingService.ListSpeakershipsAsync("episode123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task ListSpeakerEpisodesAsync_ShouldReturnPagedSpeakerships_WhenApiReturnsData()
    {
        // Arrange
        var speakershipsResponse = _builder.BuildSpeakershipCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/publishing/v2/speakers/speaker123/speakerships", speakershipsResponse);

        // Act
        var result = await _publishingService.ListSpeakerEpisodesAsync("speaker123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_ShouldReturnSpeakership_WhenRequestIsValid()
    {
        // Arrange
        var speakershipDto = _builder.CreateSpeakershipDto(s => 
        {
            s.Id = "speakership123";
            s.Relationships = new SpeakershipRelationshipsDto
            {
                Episode = new Models.JsonApi.Core.RelationshipData { Type = "Episode", Id = "episode123" },
                Speaker = new Models.JsonApi.Core.RelationshipData { Type = "Speaker", Id = "speaker123" }
            };
        });

        var response = new JsonApiSingleResponse<SpeakershipDto> { Data = speakershipDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/episodes/episode123/speakerships", response);

        // Act
        var result = await _publishingService.AddSpeakerToEpisodeAsync("episode123", "speaker123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("speakership123");
        result!.EpisodeId.Should().Be("episode123");
        result!.SpeakerId.Should().Be("speaker123");
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_ShouldThrowArgumentException_WhenEpisodeIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.AddSpeakerToEpisodeAsync("", "speaker123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_ShouldThrowArgumentException_WhenSpeakerIdIsEmpty()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.AddSpeakerToEpisodeAsync("episode123", ""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Speaker ID cannot be null or empty*");
    }

    [Fact]
    public async Task DeleteSpeakershipAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteSpeakershipAsync("speakership123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Media Tests

    [Fact]
    public async Task GetMediaAsync_ShouldReturnMedia_WhenApiReturnsData()
    {
        // Arrange
        var mediaDto = _builder.CreateMediaDto(m => m.Id = "media123");
        var response = new JsonApiSingleResponse<MediaDto> { Data = mediaDto };
        _mockApiConnection.SetupGetResponse("/publishing/v2/media/media123", response);

        // Act
        var result = await _publishingService.GetMediaAsync("media123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("media123");
        result!.FileName.Should().Be(mediaDto.Attributes.FileName);
        result!.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public async Task GetMediaAsync_ShouldReturnNull_WhenMediaNotFound()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse<JsonApiSingleResponse<MediaDto>>("/publishing/v2/media/nonexistent", new JsonApiSingleResponse<MediaDto>());

        // Act
        var result = await _publishingService.GetMediaAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListMediaAsync_ShouldReturnPagedMedia_WhenApiReturnsData()
    {
        // Arrange
        var mediaResponse = _builder.BuildMediaCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes/episode123/media", mediaResponse);

        // Act
        var result = await _publishingService.ListMediaAsync("episode123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task UploadMediaAsync_ShouldReturnUploadedMedia_WhenRequestIsValid()
    {
        // Arrange
        var request = new MediaUploadRequest
        {
            FileName = "test-audio.mp3",
            ContentType = "audio/mpeg",
            FileSizeInBytes = 1024000,
            MediaType = "audio",
            Quality = "high",
            IsPrimary = true
        };

        var mediaDto = _builder.CreateMediaDto(m => 
        {
            m.Id = "newmedia123";
            m.Attributes.FileName = "test-audio.mp3";
            m.Attributes.ContentType = "audio/mpeg";
            m.Attributes.MediaType = "audio";
        });

        var response = new JsonApiSingleResponse<MediaDto> { Data = mediaDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/episodes/episode123/media", response);

        // Act
        var result = await _publishingService.UploadMediaAsync("episode123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newmedia123");
        result!.FileName.Should().Be("test-audio.mp3");
        result!.ContentType.Should().Be("audio/mpeg");
        result!.MediaType.Should().Be("audio");
    }

    [Fact]
    public async Task UploadMediaAsync_ShouldThrowArgumentException_WhenEpisodeIdIsEmpty()
    {
        // Arrange
        var request = new MediaUploadRequest { FileName = "test.mp3" };

        // Act & Assert
        await _publishingService.Invoking(s => s.UploadMediaAsync("", request))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Episode ID cannot be null or empty*");
    }

    [Fact]
    public async Task UploadMediaAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.UploadMediaAsync("episode123", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateMediaAsync_ShouldReturnUpdatedMedia_WhenRequestIsValid()
    {
        // Arrange
        var request = new MediaUpdateRequest
        {
            FileName = "updated-audio.mp3",
            Quality = "medium",
            IsPrimary = false
        };

        var mediaDto = _builder.CreateMediaDto(m => 
        {
            m.Id = "media123";
            m.Attributes.FileName = "updated-audio.mp3";
            m.Attributes.Quality = "medium";
            m.Attributes.IsPrimary = false;
        });

        var response = new JsonApiSingleResponse<MediaDto> { Data = mediaDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/publishing/v2/media/media123", response);

        // Act
        var result = await _publishingService.UpdateMediaAsync("media123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("media123");
        result!.FileName.Should().Be("updated-audio.mp3");
        result!.Quality.Should().Be("medium");
        result!.IsPrimary.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteMediaAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _publishingService.Invoking(s => s.DeleteMediaAsync("media123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Distribution Tests

    [Fact]
    public async Task ListDistributionChannelsAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _publishingService.ListDistributionChannelsAsync(),
            "/publishing/v2/distribution_channels"
        );
    }

    [Fact]
    public async Task DistributeEpisodeAsync_ShouldReturnSuccessResult_WhenRequestIsValid()
    {
        // Arrange
        // Return a proper JsonApiSingleResponse with DistributionDto data
        var distributionDto = new DistributionDto
        {
            Id = "dist123",
            Type = "Distribution",
            Attributes = new DistributionAttributesDto
            {
                Success = true,
                Message = "Successfully distributed episode123 to channel123",
                DistributedAt = DateTime.UtcNow
            }
        };
        var response = new JsonApiSingleResponse<DistributionDto> { Data = distributionDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/distributions", response);

        // Act
        var result = await _publishingService.DistributeEpisodeAsync("episode123", "channel123");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("episode123");
        result.Message.Should().Contain("channel123");
    }

    [Fact]
    public async Task DistributeSeriesAsync_ShouldReturnSuccessResult_WhenRequestIsValid()
    {
        // Arrange
        // Return a proper JsonApiSingleResponse with DistributionDto data
        var distributionDto = new DistributionDto
        {
            Id = "dist124",
            Type = "Distribution",
            Attributes = new DistributionAttributesDto
            {
                Success = true,
                Message = "Successfully distributed series123 to channel123",
                DistributedAt = DateTime.UtcNow
            }
        };
        var response = new JsonApiSingleResponse<DistributionDto> { Data = distributionDto };
        _mockApiConnection.SetupMutationResponse("POST", "/publishing/v2/distributions", response);

        // Act
        var result = await _publishingService.DistributeSeriesAsync("series123", "channel123");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("series123");
        result.Message.Should().Contain("channel123");
    }

    #endregion

    #region Analytics Tests

    [Fact]
    public async Task GetEpisodeAnalyticsAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new AnalyticsRequest
        {
            StartDate = DateTime.Parse("2025-06-22"),
            EndDate = DateTime.Parse("2025-07-22"),
            Metrics = new List<string> { "views", "downloads" }
        };

        // Act & Assert - The service will build the endpoint with query parameters
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _publishingService.GetEpisodeAnalyticsAsync("episode123", request);
        });

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("No GET stub configured for /publishing/v2/episodes/episode123/analytics");
    }

    [Fact]
    public async Task GetSeriesAnalyticsAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new AnalyticsRequest
        {
            StartDate = DateTime.Parse("2025-06-22"),
            EndDate = DateTime.Parse("2025-07-22")
        };

        // Act & Assert - The service will build the endpoint with query parameters
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _publishingService.GetSeriesAnalyticsAsync("series123", request);
        });

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("No GET stub configured for /publishing/v2/series/series123/analytics");
    }

    [Fact]
    public async Task GeneratePublishingReportAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new PublishingReportRequest
        {
            StartDate = DateTime.Parse("2025-06-22"),
            EndDate = DateTime.Parse("2025-07-22"),
            IncludeEpisodeDetails = true,
            IncludeSeriesDetails = true,
            Format = "json"
        };

        // Act & Assert - The service will build the endpoint with query parameters
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _publishingService.GeneratePublishingReportAsync(request);
        });

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("No GET stub configured for /publishing/v2/reports");
    }

    #endregion

    #region Pagination Helper Tests

    [Fact]
    public async Task GetAllEpisodesAsync_ShouldReturnAllEpisodes_WhenMultiplePagesExist()
    {
        // Arrange
        var page1Response = _builder.BuildEpisodeCollectionResponse(2);
        page1Response.Links = new PagedResponseLinks { Next = "/publishing/v2/episodes?offset=2" };
        
        var page2Response = _builder.BuildEpisodeCollectionResponse(1);
        page2Response.Links = new PagedResponseLinks { Next = null };

        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes?per_page=100", page1Response);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes?per_page=100&offset=100", page2Response);


        // Act
        var result = await _publishingService.GetAllEpisodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task StreamEpisodesAsync_ShouldYieldEpisodes_WhenCalled()
    {
        // Arrange
        var episodesResponse = _builder.BuildEpisodeCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes?per_page=100", episodesResponse);

        // Act
        var episodes = new List<Episode>();
        await foreach (var episode in _publishingService.StreamEpisodesAsync())
        {
            episodes.Add(episode);
        }

        // Assert
        episodes.Should().HaveCount(3);
        episodes.All(e => e.DataSource == "Publishing").Should().BeTrue();
    }

    #endregion
}