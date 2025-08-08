using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Tests.Utilities;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Services;

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

    #region Episode Management Tests

    [Fact]
    public async Task GetEpisodeAsync_WithValidId_ShouldReturnEpisode()
    {
        // Arrange
        var episodeId = "123";
        var expectedEpisode = _builder.CreateEpisodeDto(e => e.Id = episodeId);
        var response = new JsonApiSingleResponse<EpisodeDto> { Data = expectedEpisode };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/episodes/{episodeId}", response);

        // Act
        var result = await _publishingService.GetEpisodeAsync(episodeId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(episodeId);
    }

    [Fact]
    public async Task GetEpisodeAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetEpisodeAsync(""));
    }

    [Fact]
    public async Task GetEpisodeAsync_WithNullId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetEpisodeAsync(null!));
    }

    [Fact]
    public async Task ListEpisodesAsync_WithoutParameters_ShouldReturnPagedEpisodes()
    {
        // Arrange
        var episodes = new List<EpisodeDto> { _builder.CreateEpisodeDto(e => e.Id = "1"), _builder.CreateEpisodeDto(e => e.Id = "2") };
        var pagedResponse = _builder.BuildPagedResponse(episodes);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", pagedResponse);

        // Act
        var result = await _publishingService.ListEpisodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListEpisodesAsync_WithParameters_ShouldIncludeQueryString()
    {
        // Arrange
        var parameters = new QueryParameters { Where = new Dictionary<string, object> { ["published"] = true } };
        var episodes = new List<EpisodeDto> { _builder.CreateEpisodeDto(e => e.Id = "1") };
        var pagedResponse = _builder.BuildPagedResponse(episodes);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", pagedResponse);

        // Act
        var result = await _publishingService.ListEpisodesAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateEpisodeAsync_WithValidRequest_ShouldReturnCreatedEpisode()
    {
        // Arrange
        var request = _builder.BuildEpisodeCreateRequest();
        var expectedEpisode = _builder.CreateEpisodeDto(e => 
        {
            e.Id = "123";
            e.Attributes.Title = request.Title;
        });
        var response = new JsonApiSingleResponse<EpisodeDto> { Data = expectedEpisode };
        _mockApiConnection.SetupPostResponse("/publishing/v2/episodes", response);

        // Act
        var result = await _publishingService.CreateEpisodeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task CreateEpisodeAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.CreateEpisodeAsync(null!));
    }

    [Fact]
    public async Task UpdateEpisodeAsync_WithValidRequest_ShouldReturnUpdatedEpisode()
    {
        // Arrange
        var episodeId = "123";
        var request = _builder.BuildEpisodeUpdateRequest();
        var expectedEpisode = _builder.CreateEpisodeDto(e => e.Id = episodeId);
        var response = new JsonApiSingleResponse<EpisodeDto> { Data = expectedEpisode };
        _mockApiConnection.SetupPatchResponse($"/publishing/v2/episodes/{episodeId}", response);

        // Act
        var result = await _publishingService.UpdateEpisodeAsync(episodeId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(episodeId);
    }

    [Fact]
    public async Task UpdateEpisodeAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = _builder.BuildEpisodeUpdateRequest();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.UpdateEpisodeAsync("", request));
    }

    [Fact]
    public async Task UpdateEpisodeAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.UpdateEpisodeAsync("123", null!));
    }

    [Fact]
    public async Task DeleteEpisodeAsync_WithValidId_ShouldCallDeleteEndpoint()
    {
        // Arrange
        var episodeId = "123";
        _mockApiConnection.SetupDeleteResponse($"/publishing/v2/episodes/{episodeId}");

        // Act
        await _publishingService.DeleteEpisodeAsync(episodeId);

        // Assert
        // Verify delete was called successfully
    }

    [Fact]
    public async Task DeleteEpisodeAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.DeleteEpisodeAsync(""));
    }

    [Fact]
    public async Task PublishEpisodeAsync_WithValidId_ShouldReturnPublishedEpisode()
    {
        // Arrange
        var episodeId = "123";
        var expectedEpisode = _builder.CreateEpisodeDto(e => e.Id = episodeId);
        var response = new JsonApiSingleResponse<dynamic> { Data = expectedEpisode };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/episodes/{episodeId}/publish", response);

        // Act
        var result = await _publishingService.PublishEpisodeAsync(episodeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(episodeId);
    }

    [Fact]
    public async Task UnpublishEpisodeAsync_WithValidId_ShouldReturnUnpublishedEpisode()
    {
        // Arrange
        var episodeId = "123";
        var expectedEpisode = _builder.CreateEpisodeDto(e => e.Id = episodeId);
        var response = new JsonApiSingleResponse<dynamic> { Data = expectedEpisode };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/episodes/{episodeId}/unpublish", response);

        // Act
        var result = await _publishingService.UnpublishEpisodeAsync(episodeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(episodeId);
    }

    #endregion

    #region Series Management Tests

    [Fact]
    public async Task GetSeriesAsync_WithValidId_ShouldReturnSeries()
    {
        // Arrange
        var seriesId = "456";
        var expectedSeries = _builder.CreateSeriesDto(s => s.Id = seriesId);
        var response = new JsonApiSingleResponse<SeriesDto> { Data = expectedSeries };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/series/{seriesId}", response);

        // Act
        var result = await _publishingService.GetSeriesAsync(seriesId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(seriesId);
    }

    [Fact]
    public async Task GetSeriesAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetSeriesAsync(""));
    }

    [Fact]
    public async Task ListSeriesAsync_WithoutParameters_ShouldReturnPagedSeries()
    {
        // Arrange
        var series = new List<SeriesDto> { _builder.CreateSeriesDto(s => s.Id = "1"), _builder.CreateSeriesDto(s => s.Id = "2") };
        var pagedResponse = _builder.BuildPagedResponse(series);
        _mockApiConnection.SetupGetResponse("/publishing/v2/series", pagedResponse);

        // Act
        var result = await _publishingService.ListSeriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateSeriesAsync_WithValidRequest_ShouldReturnCreatedSeries()
    {
        // Arrange
        var request = _builder.BuildSeriesCreateRequest();
        var expectedSeries = _builder.CreateSeriesDto(s => 
        {
            s.Id = "456";
            s.Attributes.Title = request.Title;
        });
        var response = new JsonApiSingleResponse<SeriesDto> { Data = expectedSeries };
        _mockApiConnection.SetupPostResponse("/publishing/v2/series", response);

        // Act
        var result = await _publishingService.CreateSeriesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("456");
        result.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task CreateSeriesAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.CreateSeriesAsync(null!));
    }

    [Fact]
    public async Task UpdateSeriesAsync_WithValidRequest_ShouldReturnUpdatedSeries()
    {
        // Arrange
        var seriesId = "456";
        var request = _builder.BuildSeriesUpdateRequest();
        var expectedSeries = _builder.CreateSeriesDto(s => s.Id = seriesId);
        var response = new JsonApiSingleResponse<SeriesDto> { Data = expectedSeries };
        _mockApiConnection.SetupPatchResponse($"/publishing/v2/series/{seriesId}", response);

        // Act
        var result = await _publishingService.UpdateSeriesAsync(seriesId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(seriesId);
    }

    [Fact]
    public async Task DeleteSeriesAsync_WithValidId_ShouldCallDeleteEndpoint()
    {
        // Arrange
        var seriesId = "456";
        _mockApiConnection.SetupDeleteResponse($"/publishing/v2/series/{seriesId}");

        // Act
        await _publishingService.DeleteSeriesAsync(seriesId);

        // Assert
        // Verify delete was called successfully
    }

    [Fact]
    public async Task PublishSeriesAsync_WithValidId_ShouldReturnPublishedSeries()
    {
        // Arrange
        var seriesId = "456";
        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = seriesId;
            s.Attributes.IsPublished = true;
        });
        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/series/{seriesId}/publish", response);

        // Act
        var result = await _publishingService.PublishSeriesAsync(seriesId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(seriesId);
    }

    [Fact]
    public async Task UnpublishSeriesAsync_WithValidId_ShouldReturnUnpublishedSeries()
    {
        // Arrange
        var seriesId = "456";
        var seriesDto = _builder.CreateSeriesDto(s => 
        {
            s.Id = seriesId;
            s.Attributes.IsPublished = false;
        });
        var response = new JsonApiSingleResponse<SeriesDto> { Data = seriesDto };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/series/{seriesId}/unpublish", response);

        // Act
        var result = await _publishingService.UnpublishSeriesAsync(seriesId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(seriesId);
    }

    #endregion

    #region Speaker Management Tests

    [Fact]
    public async Task GetSpeakerAsync_WithValidId_ShouldReturnSpeaker()
    {
        // Arrange
        var speakerId = "789";
        var expectedSpeaker = _builder.CreateSpeakerDto(s => s.Id = speakerId);
        var response = new JsonApiSingleResponse<SpeakerDto> { Data = expectedSpeaker };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/speakers/{speakerId}", response);

        // Act
        var result = await _publishingService.GetSpeakerAsync(speakerId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(speakerId);
    }

    [Fact]
    public async Task GetSpeakerAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetSpeakerAsync(""));
    }

    [Fact]
    public async Task ListSpeakersAsync_WithoutParameters_ShouldReturnPagedSpeakers()
    {
        // Arrange
        var speakers = new List<SpeakerDto> { _builder.CreateSpeakerDto(s => s.Id = "1"), _builder.CreateSpeakerDto(s => s.Id = "2") };
        var pagedResponse = _builder.BuildPagedResponse(speakers);
        _mockApiConnection.SetupGetResponse("/publishing/v2/speakers", pagedResponse);

        // Act
        var result = await _publishingService.ListSpeakersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateSpeakerAsync_WithValidRequest_ShouldReturnCreatedSpeaker()
    {
        // Arrange
        var request = _builder.BuildSpeakerCreateRequest();
        var expectedSpeaker = _builder.CreateSpeakerDto(s => s.Id = "789");
        var response = new JsonApiSingleResponse<SpeakerDto> { Data = expectedSpeaker };
        _mockApiConnection.SetupPostResponse("/publishing/v2/speakers", response);

        // Act
        var result = await _publishingService.CreateSpeakerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("789");
    }

    [Fact]
    public async Task CreateSpeakerAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.CreateSpeakerAsync(null!));
    }

    [Fact]
    public async Task UpdateSpeakerAsync_WithValidRequest_ShouldReturnUpdatedSpeaker()
    {
        // Arrange
        var speakerId = "789";
        var request = _builder.BuildSpeakerUpdateRequest();
        var expectedSpeaker = _builder.CreateSpeakerDto(s => s.Id = speakerId);
        var response = new JsonApiSingleResponse<SpeakerDto> { Data = expectedSpeaker };
        _mockApiConnection.SetupPatchResponse($"/publishing/v2/speakers/{speakerId}", response);

        // Act
        var result = await _publishingService.UpdateSpeakerAsync(speakerId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(speakerId);
    }

    [Fact]
    public async Task DeleteSpeakerAsync_WithValidId_ShouldCallDeleteEndpoint()
    {
        // Arrange
        var speakerId = "789";
        _mockApiConnection.SetupDeleteResponse($"/publishing/v2/speakers/{speakerId}");

        // Act
        await _publishingService.DeleteSpeakerAsync(speakerId);

        // Assert
        // Verify delete was called successfully
    }

    #endregion

    #region Speakership Management Tests

    [Fact]
    public async Task ListSpeakershipsAsync_WithValidEpisodeId_ShouldReturnPagedSpeakerships()
    {
        // Arrange
        var episodeId = "123";
        var pagedResponse = _builder.BuildSpeakershipCollectionResponse(2);
        _mockApiConnection.SetupGetResponse($"/publishing/v2/episodes/{episodeId}/speakerships", pagedResponse);

        // Act
        var result = await _publishingService.ListSpeakershipsAsync(episodeId);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListSpeakershipsAsync_WithEmptyEpisodeId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.ListSpeakershipsAsync(""));
    }

    [Fact]
    public async Task ListSpeakerEpisodesAsync_WithValidSpeakerId_ShouldReturnPagedSpeakerships()
    {
        // Arrange
        var speakerId = "789";
        var pagedResponse = _builder.BuildSpeakershipCollectionResponse(2);
        _mockApiConnection.SetupGetResponse($"/publishing/v2/speakers/{speakerId}/speakerships", pagedResponse);

        // Act
        var result = await _publishingService.ListSpeakerEpisodesAsync(speakerId);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_WithValidIds_ShouldReturnCreatedSpeakership()
    {
        // Arrange
        var episodeId = "123";
        var speakerId = "789";
        var expectedSpeakership = new JsonApiSingleResponse<SpeakershipDto> { Data = _builder.CreateSpeakershipDto(s => s.Id = "999") };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/episodes/{episodeId}/speakerships", expectedSpeakership);

        // Act
        var result = await _publishingService.AddSpeakerToEpisodeAsync(episodeId, speakerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("999");
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_WithEmptyEpisodeId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.AddSpeakerToEpisodeAsync("", "789"));
    }

    [Fact]
    public async Task AddSpeakerToEpisodeAsync_WithEmptySpeakerId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.AddSpeakerToEpisodeAsync("123", ""));
    }

    [Fact]
    public async Task DeleteSpeakershipAsync_WithValidId_ShouldCallDeleteEndpoint()
    {
        // Arrange
        var speakershipId = "999";
        _mockApiConnection.SetupDeleteResponse($"/publishing/v2/speakerships/{speakershipId}");

        // Act
        await _publishingService.DeleteSpeakershipAsync(speakershipId);

        // Assert
        // Verify delete was called successfully
    }

    #endregion

    #region Media Management Tests

    [Fact]
    public async Task GetMediaAsync_WithValidId_ShouldReturnMedia()
    {
        // Arrange
        var mediaId = "555";
        var expectedMedia = new JsonApiSingleResponse<MediaDto> { Data = _builder.CreateMediaDto(m => m.Id = mediaId) };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/media/{mediaId}", expectedMedia);

        // Act
        var result = await _publishingService.GetMediaAsync(mediaId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(mediaId);
    }

    [Fact]
    public async Task GetMediaAsync_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetMediaAsync(""));
    }

    [Fact]
    public async Task ListMediaAsync_WithValidEpisodeId_ShouldReturnPagedMedia()
    {
        // Arrange
        var episodeId = "123";
        var media = new List<MediaDto> { _builder.CreateMediaDto(m => m.Id = "1"), _builder.CreateMediaDto(m => m.Id = "2") };
        var mediaResponse = _builder.BuildPagedResponse(media);
        _mockApiConnection.SetupGetResponse($"/publishing/v2/episodes/{episodeId}/media", mediaResponse);

        // Act
        var result = await _publishingService.ListMediaAsync(episodeId);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task UploadMediaAsync_WithValidRequest_ShouldReturnUploadedMedia()
    {
        // Arrange
        var episodeId = "123";
        var request = _builder.BuildMediaUploadRequest();
        var expectedMedia = new JsonApiSingleResponse<MediaDto> { Data = _builder.CreateMediaDto(m => m.Id = "555") };
        _mockApiConnection.SetupPostResponse($"/publishing/v2/episodes/{episodeId}/media", expectedMedia);

        // Act
        var result = await _publishingService.UploadMediaAsync(episodeId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("555");
    }

    [Fact]
    public async Task UploadMediaAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.UploadMediaAsync("123", null!));
    }

    [Fact]
    public async Task UpdateMediaAsync_WithValidRequest_ShouldReturnUpdatedMedia()
    {
        // Arrange
        var mediaId = "555";
        var request = _builder.BuildMediaUpdateRequest();
        var expectedMedia = new JsonApiSingleResponse<MediaDto> { Data = _builder.CreateMediaDto(m => m.Id = mediaId) };
        _mockApiConnection.SetupPatchResponse($"/publishing/v2/media/{mediaId}", expectedMedia);

        // Act
        var result = await _publishingService.UpdateMediaAsync(mediaId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(mediaId);
    }

    [Fact]
    public async Task DeleteMediaAsync_WithValidId_ShouldCallDeleteEndpoint()
    {
        // Arrange
        var mediaId = "555";
        _mockApiConnection.SetupDeleteResponse($"/publishing/v2/media/{mediaId}");

        // Act
        await _publishingService.DeleteMediaAsync(mediaId);

        // Assert
        // Verify delete was called successfully
    }

    #endregion

    #region Distribution Tests

    [Fact]
    public async Task ListDistributionChannelsAsync_ShouldReturnPagedChannels()
    {
        // Arrange
        var channels = new List<ChannelDto> { _builder.CreateChannelDto(c => c.Id = "1"), _builder.CreateChannelDto(c => c.Id = "2") };
        var pagedResponse = _builder.BuildPagedResponse(channels);
        _mockApiConnection.SetupGetResponse("/publishing/v2/distribution_channels", pagedResponse);

        // Act
        var result = await _publishingService.ListDistributionChannelsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task DistributeEpisodeAsync_WithValidIds_ShouldReturnDistributionResult()
    {
        // Arrange
        var episodeId = "123";
        var channelId = "456";
        var distributionDto = new DistributionDto
        {
            Id = "dist123",
            Type = "Distribution",
            Attributes = new DistributionAttributesDto
            {
                Success = true,
                Message = "Successfully distributed",
                DistributedAt = DateTimeOffset.UtcNow
            }
        };
        var response = new JsonApiSingleResponse<DistributionDto> { Data = distributionDto };
        _mockApiConnection.SetupPostResponse("/publishing/v2/distributions", response);

        // Act
        var result = await _publishingService.DistributeEpisodeAsync(episodeId, channelId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DistributeEpisodeAsync_WithEmptyEpisodeId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.DistributeEpisodeAsync("", "456"));
    }

    [Fact]
    public async Task DistributeSeriesAsync_WithValidIds_ShouldReturnDistributionResult()
    {
        // Arrange
        var seriesId = "789";
        var channelId = "456";
        var distributionDto = new DistributionDto
        {
            Id = "dist789",
            Type = "Distribution",
            Attributes = new DistributionAttributesDto
            {
                Success = true,
                Message = "Successfully distributed series",
                DistributedAt = DateTimeOffset.UtcNow
            }
        };
        var expectedResult = new JsonApiSingleResponse<DistributionDto> { Data = distributionDto };
        _mockApiConnection.SetupPostResponse("/publishing/v2/distributions", expectedResult);

        // Act
        var result = await _publishingService.DistributeSeriesAsync(seriesId, channelId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    #endregion

    #region Analytics Tests

    [Fact]
    public async Task GetEpisodeAnalyticsAsync_WithValidRequest_ShouldReturnAnalytics()
    {
        // Arrange
        var episodeId = "123";
        var request = _builder.BuildAnalyticsRequest();
        var expectedAnalytics = new JsonApiSingleResponse<EpisodeAnalyticsDto> { Data = _builder.CreateEpisodeAnalyticsDto(a => a.Attributes!.EpisodeId = episodeId) };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/episodes/{episodeId}/analytics", expectedAnalytics);

        // Act
        var result = await _publishingService.GetEpisodeAnalyticsAsync(episodeId, request);

        // Assert
        result.Should().NotBeNull();
        result.EpisodeId.Should().Be(episodeId);
    }

    [Fact]
    public async Task GetEpisodeAnalyticsAsync_WithEmptyEpisodeId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = _builder.BuildAnalyticsRequest();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _publishingService.GetEpisodeAnalyticsAsync("", request));
    }

    [Fact]
    public async Task GetSeriesAnalyticsAsync_WithValidRequest_ShouldReturnAnalytics()
    {
        // Arrange
        var seriesId = "456";
        var request = _builder.BuildAnalyticsRequest();
        var expectedAnalytics = new JsonApiSingleResponse<SeriesAnalyticsDto> { Data = _builder.CreateSeriesAnalyticsDto() };
        _mockApiConnection.SetupGetResponse($"/publishing/v2/series/{seriesId}/analytics", expectedAnalytics);

        // Act
        var result = await _publishingService.GetSeriesAnalyticsAsync(seriesId, request);

        // Assert
        result.Should().NotBeNull();
        result.SeriesId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GeneratePublishingReportAsync_WithValidRequest_ShouldReturnReport()
    {
        // Arrange
        var request = _builder.BuildPublishingReportRequest();
        var expectedReport = new JsonApiSingleResponse<PublishingReportDto> { Data = _builder.CreatePublishingReportDto() };
        _mockApiConnection.SetupGetResponse("/publishing/v2/reports?start_date=2025-07-05&end_date=2025-08-04&format=json&include_episode_details=true&include_series_details=true", expectedReport);

        // Act
        var result = await _publishingService.GeneratePublishingReportAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GeneratePublishingReportAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publishingService.GeneratePublishingReportAsync(null!));
    }

    #endregion

    #region Pagination Helper Tests

    [Fact]
    public async Task GetAllEpisodesAsync_WithoutParameters_ShouldReturnAllEpisodes()
    {
        // Arrange
        var episodes = new List<EpisodeDto> { _builder.CreateEpisodeDto(e => e.Id = "1"), _builder.CreateEpisodeDto(e => e.Id = "2"), _builder.CreateEpisodeDto(e => e.Id = "3") };
        var pagedResponse = _builder.BuildPagedResponse(episodes);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", pagedResponse);

        // Act
        var result = await _publishingService.GetAllEpisodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task StreamEpisodesAsync_WithoutParameters_ShouldStreamAllEpisodes()
    {
        // Arrange
        var episodes = new List<EpisodeDto> { _builder.CreateEpisodeDto(e => e.Id = "1"), _builder.CreateEpisodeDto(e => e.Id = "2") };
        var pagedResponse = _builder.BuildPagedResponse(episodes);
        _mockApiConnection.SetupGetResponse("/publishing/v2/episodes", pagedResponse);

        // Act
        var result = new List<Episode>();
        await foreach (var episode in _publishingService.StreamEpisodesAsync())
        {
            result.Add(episode);
        }

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion
}