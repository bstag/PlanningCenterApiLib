using FluentAssertions;
using PlanningCenter.Api.Client.Mapping.Publishing;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Mapping;

public class PublishingMapperTests
{
    private readonly ExtendedTestDataBuilder _builder = new();

    #region Episode Mapping Tests

    [Fact]
    public void MapToDomain_ShouldMapEpisodeDto_WhenValidDtoProvided()
    {
        // Arrange
        var episodeDto = _builder.CreateEpisodeDto(e =>
        {
            e.Id = "episode123";
            e.Attributes.Title = "Test Episode";
            e.Attributes.Description = "Test Description";
            e.Attributes.LengthInSeconds = 1800;
            e.Attributes.IsPublished = true;
            e.Attributes.EpisodeNumber = 5;
            e.Attributes.SeasonNumber = 2;
            e.Relationships = new EpisodeRelationshipsDto
            {
                Series = new RelationshipData { Type = "Series", Id = "series123" }
            };
        });

        // Act
        var result = PublishingMapper.MapToDomain(episodeDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("episode123");
        result.Title.Should().Be("Test Episode");
        result.Description.Should().Be("Test Description");
        result.LengthInSeconds.Should().Be(1800);
        result.IsPublished.Should().BeTrue();
        result.EpisodeNumber.Should().Be(5);
        result.SeasonNumber.Should().Be(2);
        result.SeriesId.Should().Be("series123");
        result.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public void MapCreateRequestToJsonApi_ShouldMapEpisodeCreateRequest_WhenValidRequestProvided()
    {
        // Arrange
        var request = new EpisodeCreateRequest
        {
            Title = "New Episode",
            Description = "New Description",
            SeriesId = "series456",
            LengthInSeconds = 2400,
            EpisodeNumber = 3,
            SeasonNumber = 1,
            Tags = new List<string> { "tag1", "tag2" },
            Categories = new List<string> { "category1" }
        };

        // Act
        var result = PublishingMapper.MapCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Episode");
        result.Data.Attributes.Title.Should().Be("New Episode");
        result.Data.Attributes.Description.Should().Be("New Description");
        result.Data.Attributes.LengthInSeconds.Should().Be(2400);
        result.Data.Attributes.EpisodeNumber.Should().Be(3);
        result.Data.Attributes.SeasonNumber.Should().Be(1);
        result.Data.Attributes.Tags.Should().BeEquivalentTo(new[] { "tag1", "tag2" });
        result.Data.Attributes.Categories.Should().BeEquivalentTo(new[] { "category1" });
        result.Data.Relationships.Should().NotBeNull();
        result.Data.Relationships!.Series.Should().NotBeNull();
        result.Data.Relationships!.Series!.Id.Should().Be("series456");
        result.Data.Relationships!.Series!.Type.Should().Be("Series");
    }

    [Fact]
    public void MapCreateRequestToJsonApi_ShouldMapWithoutRelationships_WhenSeriesIdIsEmpty()
    {
        // Arrange
        var request = new EpisodeCreateRequest
        {
            Title = "New Episode",
            Description = "New Description"
        };

        // Act
        var result = PublishingMapper.MapCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Relationships.Should().BeNull();
    }

    [Fact]
    public void MapUpdateRequestToJsonApi_ShouldMapEpisodeUpdateRequest_WhenValidRequestProvided()
    {
        // Arrange
        var request = new EpisodeUpdateRequest
        {
            Title = "Updated Episode",
            Description = "Updated Description",
            IsPublished = true,
            EpisodeNumber = 7,
            Tags = new List<string> { "updated-tag" }
        };

        // Act
        var result = PublishingMapper.MapUpdateRequestToJsonApi("episode123", request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Episode");
        result.Data.Id.Should().Be("episode123");
        result.Data.Attributes.Title.Should().Be("Updated Episode");
        result.Data.Attributes.Description.Should().Be("Updated Description");
        result.Data.Attributes.IsPublished.Should().BeTrue();
        result.Data.Attributes.EpisodeNumber.Should().Be(7);
        result.Data.Attributes.Tags.Should().BeEquivalentTo(new[] { "updated-tag" });
    }

    #endregion

    #region Series Mapping Tests

    [Fact]
    public void MapToDomain_ShouldMapSeriesDto_WhenValidDtoProvided()
    {
        // Arrange
        var seriesDto = _builder.CreateSeriesDto(s =>
        {
            s.Id = "series123";
            s.Attributes.Title = "Test Series";
            s.Attributes.Description = "Test Description";
            s.Attributes.Summary = "Test Summary";
            s.Attributes.IsPublished = true;
            s.Attributes.SeriesType = "sermon";
            s.Attributes.Language = "en";
            s.Attributes.EpisodeCount = 10;
            s.Attributes.TotalViewCount = 5000;
            s.Attributes.IsExplicit = false;
        });

        // Act
        var result = PublishingMapper.MapToDomain(seriesDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("series123");
        result.Title.Should().Be("Test Series");
        result.Description.Should().Be("Test Description");
        result.Summary.Should().Be("Test Summary");
        result.IsPublished.Should().BeTrue();
        result.SeriesType.Should().Be("sermon");
        result.Language.Should().Be("en");
        result.EpisodeCount.Should().Be(10);
        result.TotalViewCount.Should().Be(5000);
        result.IsExplicit.Should().BeFalse();
        result.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public void MapCreateRequestToJsonApi_ShouldMapSeriesCreateRequest_WhenValidRequestProvided()
    {
        // Arrange
        var request = new SeriesCreateRequest
        {
            Title = "New Series",
            Description = "New Description",
            Summary = "New Summary",
            SeriesType = "podcast",
            Language = "es",
            Tags = new List<string> { "series-tag" },
            Categories = new List<string> { "series-category" },
            IsExplicit = true
        };

        // Act
        var result = PublishingMapper.MapCreateRequestToJsonApi(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Series");
        result.Data.Attributes.Title.Should().Be("New Series");
        result.Data.Attributes.Description.Should().Be("New Description");
        result.Data.Attributes.Summary.Should().Be("New Summary");
        result.Data.Attributes.SeriesType.Should().Be("podcast");
        result.Data.Attributes.Language.Should().Be("es");
        result.Data.Attributes.Tags.Should().BeEquivalentTo(new[] { "series-tag" });
        result.Data.Attributes.Categories.Should().BeEquivalentTo(new[] { "series-category" });
        result.Data.Attributes.IsExplicit.Should().BeTrue();
    }

    [Fact]
    public void MapUpdateRequestToJsonApi_ShouldMapSeriesUpdateRequest_WhenValidRequestProvided()
    {
        // Arrange
        var request = new SeriesUpdateRequest
        {
            Title = "Updated Series",
            Description = "Updated Description",
            IsPublished = true,
            SeriesType = "teaching"
        };

        // Act
        var result = PublishingMapper.MapUpdateRequestToJsonApi("series123", request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Series");
        result.Data.Id.Should().Be("series123");
        result.Data.Attributes.Title.Should().Be("Updated Series");
        result.Data.Attributes.Description.Should().Be("Updated Description");
        result.Data.Attributes.IsPublished.Should().BeTrue();
        result.Data.Attributes.SeriesType.Should().Be("teaching");
    }

    #endregion

    #region Speaker Mapping Tests

    [Fact]
    public void MapToDomain_ShouldMapSpeakerDto_WhenValidDtoProvided()
    {
        // Arrange
        var speakerDto = _builder.CreateSpeakerDto(s =>
        {
            s.Id = "speaker123";
            s.Attributes.FirstName = "John";
            s.Attributes.LastName = "Doe";
            s.Attributes.DisplayName = "Pastor John";
            s.Attributes.Title = "Senior Pastor";
            s.Attributes.Biography = "Experienced pastor";
            s.Attributes.Email = "john@example.com";
            s.Attributes.PhoneNumber = "555-123-4567";
            s.Attributes.WebsiteUrl = "https://example.com";
            s.Attributes.PhotoUrl = "https://example.com/photo.jpg";
            s.Attributes.Organization = "Test Church";
            s.Attributes.Location = "Test City";
            s.Attributes.Active = true;
        });

        // Act
        var result = PublishingMapper.MapToDomain(speakerDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("speaker123");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.DisplayName.Should().Be("Pastor John");
        result.Title.Should().Be("Senior Pastor");
        result.Biography.Should().Be("Experienced pastor");
        result.Email.Should().Be("john@example.com");
        result.PhoneNumber.Should().Be("555-123-4567");
        result.WebsiteUrl.Should().Be("https://example.com");
        result.PhotoUrl.Should().Be("https://example.com/photo.jpg");
        result.Organization.Should().Be("Test Church");
        result.Location.Should().Be("Test City");
        result.Active.Should().BeTrue();
        result.DataSource.Should().Be("Publishing");
    }

    #endregion

    #region Media Mapping Tests

    [Fact]
    public void MapToDomain_ShouldMapMediaDto_WhenValidDtoProvided()
    {
        // Arrange
        var mediaDto = _builder.CreateMediaDto(m =>
        {
            m.Id = "media123";
            m.Attributes.FileName = "test-audio.mp3";
            m.Attributes.ContentType = "audio/mpeg";
            m.Attributes.FileSizeInBytes = 1024000;
            m.Attributes.MediaType = "audio";
            m.Attributes.Quality = "high";
            m.Attributes.Url = "https://example.com/audio.mp3";
            m.Attributes.DownloadUrl = "https://example.com/download/audio.mp3";
            m.Attributes.IsPrimary = true;
        });

        // Act
        var result = PublishingMapper.MapToDomain(mediaDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("media123");
        result.FileName.Should().Be("test-audio.mp3");
        result.ContentType.Should().Be("audio/mpeg");
        result.FileSizeInBytes.Should().Be(1024000);
        result.MediaType.Should().Be("audio");
        result.Quality.Should().Be("high");
        result.FileUrl.Should().Be("https://example.com/audio.mp3");
        result.DownloadUrl.Should().Be("https://example.com/download/audio.mp3");
        result.IsPrimary.Should().BeTrue();
        result.DataSource.Should().Be("Publishing");
    }

    #endregion

    #region Speakership Mapping Tests

    [Fact]
    public void MapToDomain_ShouldMapSpeakershipDto_WhenValidDtoProvided()
    {
        // Arrange
        var speakershipDto = _builder.CreateSpeakershipDto(s =>
        {
            s.Id = "speakership123";
            s.Attributes.Role = "Primary Speaker";
            s.Attributes.Notes = "Main presenter";
            s.Attributes.Order = 1;
            s.Attributes.IsPrimary = true;
            s.Relationships = new SpeakershipRelationshipsDto
            {
                Episode = new RelationshipData { Type = "Episode", Id = "episode123" },
                Speaker = new RelationshipData { Type = "Speaker", Id = "speaker123" }
            };
        });

        // Act
        var result = PublishingMapper.MapToDomain(speakershipDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("speakership123");
        result.EpisodeId.Should().Be("episode123");
        result.SpeakerId.Should().Be("speaker123");
        result.Role.Should().Be("Primary Speaker");
        result.Notes.Should().Be("Main presenter");
        result.SortOrder.Should().Be(1);
        result.IsPrimary.Should().BeTrue();
        result.DataSource.Should().Be("Publishing");
    }

    [Fact]
    public void MapToDomain_ShouldHandleNullRelationships_WhenRelationshipsAreNull()
    {
        // Arrange
        var speakershipDto = _builder.CreateSpeakershipDto(s =>
        {
            s.Id = "speakership123";
            s.Relationships = null;
        });

        // Act
        var result = PublishingMapper.MapToDomain(speakershipDto);

        // Assert
        result.Should().NotBeNull();
        result.EpisodeId.Should().Be(string.Empty);
        result.SpeakerId.Should().Be(string.Empty);
    }

    [Fact]
    public void MapSpeakershipCreateToJsonApi_ShouldMapCorrectly_WhenValidParametersProvided()
    {
        // Act
        var result = PublishingMapper.MapSpeakershipCreateToJsonApi("episode123", "speaker456", "Guest Speaker");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Type.Should().Be("Speakership");
        result.Data.Attributes.Role.Should().Be("Guest Speaker");
        result.Data.Attributes.IsPrimary.Should().BeFalse();
        result.Data.Relationships.Episode.Type.Should().Be("Episode");
        result.Data.Relationships.Episode.Id.Should().Be("episode123");
        result.Data.Relationships.Speaker.Type.Should().Be("Speaker");
        result.Data.Relationships.Speaker.Id.Should().Be("speaker456");
    }

    [Fact]
    public void MapSpeakershipCreateToJsonApi_ShouldHandleNullRole_WhenRoleIsNotProvided()
    {
        // Act
        var result = PublishingMapper.MapSpeakershipCreateToJsonApi("episode123", "speaker456");

        // Assert
        result.Should().NotBeNull();
        result.Data.Attributes.Role.Should().BeNull();
        result.Data.Attributes.IsPrimary.Should().BeFalse();
    }

    #endregion
}