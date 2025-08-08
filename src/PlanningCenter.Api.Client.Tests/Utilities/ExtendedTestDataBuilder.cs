using System;
using System.Collections.Generic;
using AutoFixture;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.JsonApi.Webhooks;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PeopleModels = PlanningCenter.Api.Client.Models.People;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// Extended test data factory for building DTO instances for Phase 3 modules.
/// Follows the same patterns as the original TestDataBuilder.
/// </summary>
public class ExtendedTestDataBuilder
{
    private readonly Fixture _fixture = new();
    
    /// <summary>
    /// Ensures a string is at least the specified minimum length by padding with zeros if needed.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="minLength">The minimum required length</param>
    /// <returns>A string of at least the minimum length</returns>
    private string EnsureMinLength(string input, int minLength)
    {
        if (input.Length >= minLength)
        {
            return input.Substring(0, minLength);
        }
        
        return input.PadRight(minLength, '0');
    }

    // Services Module Test Data
    public PlanDto CreatePlanDto(Action<PlanDto>? customize = null)
    {
        var dto = new PlanDto
        {
            Id = _fixture.Create<string>(),
            Type = "Plan",
            Attributes = new PlanAttributes
            {
                Title = _fixture.Create<string>(),
                Dates = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"),
                SortDate = DateTime.UtcNow.AddDays(7),
                IsPublic = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SongDto CreateSongDto(Action<SongDto>? customize = null)
    {
        var dto = new SongDto
        {
            Id = _fixture.Create<string>(),
            Type = "Song",
            Attributes = new SongAttributes
            {
                Title = _fixture.Create<string>(),
                Author = _fixture.Create<string>(),
                Copyright = "Copyright 2024 Test Music",
                CcliNumber = _fixture.Create<string>(),
                Hidden = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public ServiceTypeDto CreateServiceTypeDto(Action<ServiceTypeDto>? customize = null)
    {
        var dto = new ServiceTypeDto
        {
            Id = _fixture.Create<string>(),
            Type = "ServiceType",
            Attributes = new ServiceTypeAttributes
            {
                Name = _fixture.Create<string>(),
                Sequence = _fixture.Create<int>() % 10,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public ItemDto CreateItemDto(Action<ItemDto>? customize = null)
    {
        var dto = new ItemDto
        {
            Id = _fixture.Create<string>(),
            Type = "Item",
            Attributes = new ItemAttributes
            {
                Title = _fixture.Create<string>(),
                Sequence = _fixture.Create<int>() % 20,
                ItemType = "song",
                Description = _fixture.Create<string>(),
                Length = 4,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Groups Module Test Data
    public GroupDto CreateGroupDto(Action<GroupDto>? customize = null)
    {
        var dto = new GroupDto
        {
            Id = _fixture.Create<string>(),
            Type = "Group",
            Attributes = new GroupAttributes
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>(),
                ContactEmail = _fixture.Create<string>() + "@example.com",
                MembershipsCount = _fixture.Create<int>() % 50,
                ChatEnabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public MembershipDto CreateMembershipDto(Action<MembershipDto>? customize = null)
    {
        var dto = new MembershipDto
        {
            Id = _fixture.Create<string>(),
            Type = "Membership",
            Attributes = new MembershipAttributes
            {
                Role = "Member",
                JoinedAt = DateTime.UtcNow.AddDays(-10),
                CanViewMembers = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public GroupTypeDto CreateGroupTypeDto(Action<GroupTypeDto>? customize = null)
    {
        var dto = new GroupTypeDto
        {
            Id = _fixture.Create<string>(),
            Type = "GroupType",
            Attributes = new GroupTypeAttributes
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>(),
                ChurchCenterVisible = true,
                Position = _fixture.Create<int>() % 10,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Check-Ins Module Test Data
    public CheckInDto CreateCheckInDto(Action<CheckInDto>? customize = null)
    {
        var dto = new CheckInDto
        {
            Id = _fixture.Create<string>(),
            Type = "CheckIn",
            Attributes = new CheckInAttributes
            {
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                Kind = "regular",
                Number = _fixture.Create<int>().ToString(),
                SecurityCode = EnsureMinLength(_fixture.Create<int>().ToString(), 3),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public Models.JsonApi.CheckIns.EventDto CreateCheckInEventDto(Action<Models.JsonApi.CheckIns.EventDto>? customize = null)
    {
        var dto = new Models.JsonApi.CheckIns.EventDto
        {
            Id = _fixture.Create<string>(),
            Type = "Event",
            Attributes = new Models.JsonApi.CheckIns.EventAttributes
            {
                Name = _fixture.Create<string>(),
                Frequency = "weekly",
                EnableServicesIntegration = true,
                Archived = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Calendar Module Test Data
    public Models.JsonApi.Calendar.EventDto CreateCalendarEventDto(Action<Models.JsonApi.Calendar.EventDto>? customize = null)
    {
        var dto = new Models.JsonApi.Calendar.EventDto
        {
            Id = _fixture.Create<string>(),
            Type = "Event",
            Attributes = new Models.JsonApi.Calendar.EventAttributes
            {
                Name = _fixture.Create<string>(),
                Summary = _fixture.Create<string>(),
                AllDayEvent = false,
                StartsAt = DateTime.UtcNow.AddDays(7),
                EndsAt = DateTime.UtcNow.AddDays(7).AddHours(2),
                VisibleInChurchCenter = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public ResourceDto CreateResourceDto(Action<ResourceDto>? customize = null)
    {
        var dto = new ResourceDto
        {
            Id = _fixture.Create<string>(),
            Type = "Resource",
            Attributes = new ResourceAttributes
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>(),
                Kind = "Equipment",
                Expires = false,
                Quantity = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Giving Module Test Data
    public DonationDto CreateDonationDto(Action<DonationDto>? customize = null)
    {
        var dto = new DonationDto
        {
            Id = _fixture.Create<string>(),
            Type = "Donation",
            Attributes = new DonationAttributesDto
            {
                AmountCents = _fixture.Create<int>() % 100000 + 1000, // $10 to $1000
                AmountCurrency = "USD",
                PaymentMethod = "Credit Card",
                PaymentLast4 = "1234", // Updated property name
                PaymentBrand = "Visa",
                FeeCents = 30,
                ReceivedAt = DateTime.UtcNow.AddDays(-1),
                Refunded = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public FundDto CreateFundDto(Action<FundDto>? customize = null)
    {
        var dto = new FundDto
        {
            Id = _fixture.Create<string>(),
            Type = "Fund",
            Attributes = new FundAttributesDto
            {
                Name = _fixture.Create<string>() + " Fund",
                Description = _fixture.Create<string>(),
                Code = _fixture.Create<string>().Substring(0, 5).ToUpper(),
                Visibility = true,
                Default = false,
                Color = "#FF5733",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public BatchDto CreateBatchDto(Action<BatchDto>? customize = null)
    {
        var dto = new BatchDto
        {
            Id = _fixture.Create<string>(),
            Type = "Batch",
            Attributes = new BatchAttributesDto
            {
                Description = _fixture.Create<string>(),
                Status = "open",
                CommittedAt = null,
                TotalCents = _fixture.Create<int>() % 1000000,
                TotalCount = _fixture.Create<int>() % 100,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public PledgeDto CreatePledgeDto(Action<PledgeDto>? customize = null)
    {
        var dto = new PledgeDto
        {
            Id = _fixture.Create<string>(),
            Type = "Pledge",
            Attributes = new PledgeAttributesDto
            {
                AmountCents = _fixture.Create<int>() % 1000000 + 10000, // $100 to $10,000
                AmountCurrency = "USD",
                DonatedAmountCents = 0,
                JointGiverAmountCents = null,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public RecurringDonationDto CreateRecurringDonationDto(Action<RecurringDonationDto>? customize = null)
    {
        var dto = new RecurringDonationDto
        {
            Id = _fixture.Create<string>(),
            Type = "RecurringDonation",
            Attributes = new RecurringDonationAttributesDto
            {
                AmountCents = _fixture.Create<int>() % 50000 + 1000, // $10 to $500
                AmountCurrency = "USD",
                Status = "active",
                Schedule = "monthly",
                NextOccurrence = DateTime.UtcNow.AddDays(30),
                LastDonationReceivedAt = DateTime.UtcNow.AddDays(-30),
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public RefundDto CreateRefundDto(Action<RefundDto>? customize = null)
    {
        var dto = new RefundDto
        {
            Id = _fixture.Create<string>(),
            Type = "Refund",
            Attributes = new RefundAttributesDto
            {
                AmountCents = _fixture.Create<int>() % 10000 + 500,
                AmountCurrency = "USD",
                FeeCents = 30,
                Reason = "Requested by donor",
                RefundedAt = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public PaymentSourceDto CreatePaymentSourceDto(Action<PaymentSourceDto>? customize = null)
    {
        var dto = new PaymentSourceDto
        {
            Id = _fixture.Create<string>(),
            Type = "PaymentSource",
            Attributes = new PaymentSourceAttributesDto
            {
                Name = "Primary Card",
                PaymentMethodType = "credit_card",
                PaymentLast4 = "1234", // Updated property name
                PaymentBrand = "Visa",
                ExpirationMonth = 12,
                ExpirationYear = DateTime.UtcNow.Year + 2,
                Verified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Collection builders for Giving module
    public PagedResponse<DonationDto> BuildDonationCollectionResponse(int count = 3)
    {
        var donations = new List<DonationDto>();
        for (int i = 0; i < count; i++)
        {
            donations.Add(CreateDonationDto());
        }

        return new PagedResponse<DonationDto>
        {
            Data = donations,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/giving/v2/donations" }
        };
    }

    public PagedResponse<FundDto> BuildFundCollectionResponse(int count = 3)
    {
        var funds = new List<FundDto>();
        for (int i = 0; i < count; i++)
        {
            funds.Add(CreateFundDto());
        }

        return new PagedResponse<FundDto>
        {
            Data = funds,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/giving/v2/funds" }
        };
    }

    public PagedResponse<BatchDto> BuildBatchCollectionResponse(int count = 3)
    {
        var batches = new List<BatchDto>();
        for (int i = 0; i < count; i++)
        {
            batches.Add(CreateBatchDto());
        }

        return new PagedResponse<BatchDto>
        {
            Data = batches,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/giving/v2/batches" }
        };
    }

    public PagedResponse<PledgeDto> BuildPledgeCollectionResponse(int count = 3)
    {
        var pledges = new List<PledgeDto>();
        for (int i = 0; i < count; i++)
        {
            pledges.Add(CreatePledgeDto());
        }

        return new PagedResponse<PledgeDto>
        {
            Data = pledges,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/giving/v2/pledges" }
        };
    }

    public PagedResponse<RecurringDonationDto> BuildRecurringDonationCollectionResponse(int count = 3)
    {
        var recurringDonations = new List<RecurringDonationDto>();
        for (int i = 0; i < count; i++)
        {
            recurringDonations.Add(CreateRecurringDonationDto());
        }

        return new PagedResponse<RecurringDonationDto>
        {
            Data = recurringDonations,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/giving/v2/recurring_donations" }
        };
    }

    // Publishing Module Test Data
    public EpisodeDto CreateEpisodeDto(Action<EpisodeDto>? customize = null)
    {
        var dto = new EpisodeDto
        {
            Id = _fixture.Create<string>(),
            Type = "Episode",
            Attributes = new EpisodeAttributesDto
            {
                Title = _fixture.Create<string>() + " Episode",
                Description = _fixture.Create<string>(),
                PublishedAt = DateTime.UtcNow.AddDays(-1),
                LengthInSeconds = _fixture.Create<int>() % 3600 + 600, // 10 minutes to 1 hour
                VideoUrl = "https://example.com/video.mp4",
                AudioUrl = "https://example.com/audio.mp3",
                ArtworkUrl = "https://example.com/artwork.jpg",
                Notes = _fixture.Create<string>(),
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SeriesDto CreateSeriesDto(Action<SeriesDto>? customize = null)
    {
        var dto = new SeriesDto
        {
            Id = _fixture.Create<string>(),
            Type = "Series",
            Attributes = new SeriesAttributesDto
            {
                Title = _fixture.Create<string>() + " Series",
                Description = _fixture.Create<string>(),
                ArtworkUrl = "https://example.com/series-artwork.jpg",
                PublishedAt = DateTime.UtcNow.AddDays(-30),
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SpeakerDto CreateSpeakerDto(Action<SpeakerDto>? customize = null)
    {
        var dto = new SpeakerDto
        {
            Id = _fixture.Create<string>(),
            Type = "Speaker",
            Attributes = new SpeakerAttributesDto
            {
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                DisplayName = _fixture.Create<string>(),
                Title = "Pastor",
                Biography = _fixture.Create<string>(),
                Email = _fixture.Create<string>() + "@example.com",
                PhoneNumber = "555-123-4567",
                WebsiteUrl = "https://example.com",
                PhotoUrl = "https://example.com/photo.jpg",
                Organization = "Test Church",
                Location = "Test City",
                Active = true,
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public MediaDto CreateMediaDto(Action<MediaDto>? customize = null)
    {
        var dto = new MediaDto
        {
            Id = _fixture.Create<string>(),
            Type = "Media",
            Attributes = new MediaAttributesDto
            {
                FileName = _fixture.Create<string>() + ".mp3",
                ContentType = "audio/mpeg",
                FileSizeInBytes = _fixture.Create<int>() % 10000000 + 1000000, // 1MB to 10MB
                MediaType = "audio",
                Quality = "high",
                Url = "https://example.com/media.mp3",
                DownloadUrl = "https://example.com/download/media.mp3",
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SpeakershipDto CreateSpeakershipDto(Action<SpeakershipDto>? customize = null)
    {
        var dto = new SpeakershipDto
        {
            Id = _fixture.Create<string>(),
            Type = "Speakership",
            Attributes = new SpeakershipAttributesDto
            {
                Role = "Primary Speaker",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Collection builders for Publishing module
    public PagedResponse<EpisodeDto> BuildEpisodeCollectionResponse(int count = 3)
    {
        var episodes = new List<EpisodeDto>();
        for (int i = 0; i < count; i++)
        {
            episodes.Add(CreateEpisodeDto());
        }

        return new PagedResponse<EpisodeDto>
        {
            Data = episodes,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/publishing/v2/episodes" }
        };
    }

    public PagedResponse<SeriesDto> BuildSeriesCollectionResponse(int count = 3)
    {
        var series = new List<SeriesDto>();
        for (int i = 0; i < count; i++)
        {
            series.Add(CreateSeriesDto());
        }

        return new PagedResponse<SeriesDto>
        {
            Data = series,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/publishing/v2/series" }
        };
    }

    public PagedResponse<SpeakerDto> BuildSpeakerCollectionResponse(int count = 3)
    {
        var speakers = new List<SpeakerDto>();
        for (int i = 0; i < count; i++)
        {
            speakers.Add(CreateSpeakerDto());
        }

        return new PagedResponse<SpeakerDto>
        {
            Data = speakers,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/publishing/v2/speakers" }
        };
    }

    public PagedResponse<MediaDto> BuildMediaCollectionResponse(int count = 3)
    {
        var media = new List<MediaDto>();
        for (int i = 0; i < count; i++)
        {
            media.Add(CreateMediaDto());
        }

        return new PagedResponse<MediaDto>
        {
            Data = media,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/publishing/v2/media" }
        };
    }

    public PagedResponse<SpeakershipDto> BuildSpeakershipCollectionResponse(int count = 3)
    {
        var speakerships = new List<SpeakershipDto>();
        for (int i = 0; i < count; i++)
        {
            speakerships.Add(CreateSpeakershipDto());
        }

        return new PagedResponse<SpeakershipDto>
        {
            Data = speakerships,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/publishing/v2/speakerships" }
        };
    }

    // Webhooks Module Test Data
    public WebhookSubscriptionDto CreateWebhookSubscriptionDto(Action<WebhookSubscriptionDto>? customize = null)
    {
        var dto = new WebhookSubscriptionDto
        {
            Id = _fixture.Create<string>(),
            Type = "WebhookSubscription",
            Attributes = new WebhookSubscriptionAttributesDto
            {
                Url = "https://example.com/webhook",
                Secret = _fixture.Create<string>(),
                Active = true,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow,
                LastDeliveryAt = DateTime.UtcNow.AddHours(-1),
                LastDeliveryStatus = "delivered"
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public AvailableEventDto CreateAvailableEventDto(Action<AvailableEventDto>? customize = null)
    {
        var dto = new AvailableEventDto
        {
            Id = _fixture.Create<string>(),
            Type = "AvailableEvent",
            Attributes = new AvailableEventAttributesDto
            {
                Name = _fixture.Create<string>() + ".created",
                Description = _fixture.Create<string>(),
                Module = "people",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public WebhookEventDto CreateWebhookEventDto(Action<WebhookEventDto>? customize = null)
    {
        var dto = new WebhookEventDto
        {
            Id = _fixture.Create<string>(),
            Type = "Event",
            Attributes = new WebhookEventAttributesDto
            {
                EventType = "person.created",
                DeliveryStatus = "delivered",
                DeliveredAt = DateTime.UtcNow.AddMinutes(-30),
                ResponseCode = 200,
                ResponseTimeMs = 150.5,
                Payload = "{\"data\":{\"type\":\"Person\",\"id\":\"123\"}}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Collection builders for Webhooks module
    public PagedResponse<WebhookSubscriptionDto> BuildWebhookSubscriptionCollectionResponse(int count = 3)
    {
        var subscriptions = new List<WebhookSubscriptionDto>();
        for (int i = 0; i < count; i++)
        {
            subscriptions.Add(CreateWebhookSubscriptionDto());
        }

        return new PagedResponse<WebhookSubscriptionDto>
        {
            Data = subscriptions,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/webhooks/v2/subscriptions" }
        };
    }

    public PagedResponse<AvailableEventDto> BuildAvailableEventCollectionResponse(int count = 3)
    {
        var events = new List<AvailableEventDto>();
        for (int i = 0; i < count; i++)
        {
            events.Add(CreateAvailableEventDto());
        }

        return new PagedResponse<AvailableEventDto>
        {
            Data = events,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/webhooks/v2/available_events" }
        };
    }

    public PagedResponse<WebhookEventDto> BuildWebhookEventCollectionResponse(int count = 3)
    {
        var events = new List<WebhookEventDto>();
        for (int i = 0; i < count; i++)
        {
            events.Add(CreateWebhookEventDto());
        }

        return new PagedResponse<WebhookEventDto>
        {
            Data = events,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/webhooks/v2/events" }
        };
    }

    // Domain model builders for Publishing module
    public Episode BuildEpisode(string? id = null)
    {
        return new Episode
        {
            Id = id ?? _fixture.Create<string>(),
            Title = _fixture.Create<string>() + " Episode",
            Description = _fixture.Create<string>(),
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            LengthInSeconds = _fixture.Create<int>() % 3600 + 600,
            VideoUrl = "https://example.com/video.mp4",
            AudioUrl = "https://example.com/audio.mp3",
            ArtworkUrl = "https://example.com/artwork.jpg",
            Notes = _fixture.Create<string>(),
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Series BuildSeries(string? id = null)
    {
        return new Series
        {
            Id = id ?? _fixture.Create<string>(),
            Title = _fixture.Create<string>() + " Series",
            Description = _fixture.Create<string>(),
            ArtworkUrl = "https://example.com/series-artwork.jpg",
            PublishedAt = DateTime.UtcNow.AddDays(-30),
            CreatedAt = DateTime.UtcNow.AddDays(-60),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Speaker BuildSpeaker(string? id = null)
    {
        return new Speaker
        {
            Id = id ?? _fixture.Create<string>(),
            FirstName = _fixture.Create<string>(),
            LastName = _fixture.Create<string>(),
            DisplayName = _fixture.Create<string>(),
            Title = "Pastor",
            Biography = _fixture.Create<string>(),
            Email = _fixture.Create<string>() + "@example.com",
            PhoneNumber = "555-123-4567",
            WebsiteUrl = "https://example.com",
            PhotoUrl = "https://example.com/photo.jpg",
            Organization = "Test Church",
            Location = "Test City",
            Active = true,
            CreatedAt = DateTime.UtcNow.AddDays(-90),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Media BuildMedia(string? id = null)
    {
        return new Media
        {
            Id = id ?? _fixture.Create<string>(),
            FileName = _fixture.Create<string>() + ".mp3",
            ContentType = "audio/mpeg",
            FileSizeInBytes = _fixture.Create<int>() % 10000000 + 1000000,
            MediaType = "audio",
            Quality = "high",
            FileUrl = "https://example.com/media.mp3",
            DownloadUrl = "https://example.com/download/media.mp3",
            IsPrimary = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Speakership BuildSpeakership(string? id = null)
    {
        return new Speakership
        {
            Id = id ?? _fixture.Create<string>(),
            Role = "Primary Speaker",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };
    }

    // Request builders for Publishing module
    public EpisodeCreateRequest BuildEpisodeCreateRequest()
    {
        return new EpisodeCreateRequest
        {
            Title = _fixture.Create<string>() + " Episode",
            Description = _fixture.Create<string>(),
            Notes = _fixture.Create<string>()
        };
    }

    public EpisodeUpdateRequest BuildEpisodeUpdateRequest()
    {
        return new EpisodeUpdateRequest
        {
            Title = _fixture.Create<string>() + " Updated Episode",
            Description = _fixture.Create<string>(),
            Notes = _fixture.Create<string>()
        };
    }

    public SeriesCreateRequest BuildSeriesCreateRequest()
    {
        return new SeriesCreateRequest
        {
            Title = _fixture.Create<string>() + " Series",
            Description = _fixture.Create<string>()
        };
    }

    public SeriesUpdateRequest BuildSeriesUpdateRequest()
    {
        return new SeriesUpdateRequest
        {
            Title = _fixture.Create<string>() + " Updated Series",
            Description = _fixture.Create<string>()
        };
    }

    public SpeakerCreateRequest BuildSpeakerCreateRequest()
    {
        return new SpeakerCreateRequest
        {
            FirstName = _fixture.Create<string>(),
            LastName = _fixture.Create<string>(),
            Email = _fixture.Create<string>() + "@example.com"
        };
    }

    public SpeakerUpdateRequest BuildSpeakerUpdateRequest()
    {
        return new SpeakerUpdateRequest
        {
            FirstName = _fixture.Create<string>(),
            LastName = _fixture.Create<string>(),
            Email = _fixture.Create<string>() + "@example.com"
        };
    }

    // Analytics builders
    public EpisodeAnalytics BuildEpisodeAnalytics(string episodeId)
    {
        return new EpisodeAnalytics
        {
            EpisodeId = episodeId,
            ViewCount = _fixture.Create<int>() % 10000,
            DownloadCount = _fixture.Create<int>() % 5000,
            AverageWatchTimeSeconds = _fixture.Create<double>() % 3600,
            PeriodStart = DateTime.UtcNow.AddDays(-30),
            PeriodEnd = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, object>()
        };
    }

    public SeriesAnalytics BuildSeriesAnalytics(string seriesId)
    {
        return new SeriesAnalytics
        {
            SeriesId = seriesId,
            TotalViewCount = _fixture.Create<int>() % 100000,
            TotalDownloadCount = _fixture.Create<int>() % 50000,
            EpisodeCount = _fixture.Create<int>() % 50 + 1,
            PeriodStart = DateTime.UtcNow.AddDays(-30),
            PeriodEnd = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, object>()
        };
    }

    // Generic paged response builder
    public PagedResponse<T> BuildPagedResponse<T>(IReadOnlyList<T> data)
    {
        return new PagedResponse<T>
        {
            Data = data,
            Meta = new PagedResponseMeta { Count = data.Count, TotalCount = data.Count },
            Links = new PagedResponseLinks { Self = "/api/endpoint" }
        };
    }

    // Distribution and reporting builders
    public DistributionChannel BuildDistributionChannel(string? id = null)
    {
        return new DistributionChannel
        {
            Id = id ?? _fixture.Create<string>(),
            Name = _fixture.Create<string>() + " Channel",
            Description = _fixture.Create<string>(),
            ChannelType = "podcast",
            Active = true,
            AutoDistribute = false,
            SupportedContentTypes = new List<string> { "audio", "video" },
            SupportedFormats = new List<string> { "mp3", "mp4" },
            Configuration = new Dictionary<string, object>(),
            Credentials = new Dictionary<string, string>(),
            Metadata = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public DistributionResult BuildDistributionResult()
    {
        return new DistributionResult
        {
            Success = true,
            Message = "Successfully distributed",
            DistributedAt = DateTime.UtcNow
        };
    }

    public PublishingReportRequest BuildPublishingReportRequest()
    {
        return new PublishingReportRequest
        {
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            IncludeEpisodeDetails = true,
            IncludeSeriesDetails = true,
            Format = "json"
        };
    }

    public PublishingReport BuildPublishingReport()
    {
        return new PublishingReport
        {
            TotalEpisodes = _fixture.Create<int>() % 100,
            TotalSeries = _fixture.Create<int>() % 20,
            TotalViews = _fixture.Create<int>() % 100000,
            TotalDownloads = _fixture.Create<int>() % 50000,
            GeneratedAt = DateTime.UtcNow,
            PeriodStart = DateTime.UtcNow.AddDays(-30),
            PeriodEnd = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, object>()
        };
    }

    public AnalyticsRequest BuildAnalyticsRequest()
    {
        return new AnalyticsRequest
        {
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };
    }

    public MediaUploadRequest BuildMediaUploadRequest()
    {
        return new MediaUploadRequest
        {
            FileName = "test-media.mp4",
            ContentType = "video/mp4",
            FileSizeInBytes = 1024000,
            MediaType = "video",
            Quality = "HD",
            IsPrimary = true
        };
    }

    public MediaUpdateRequest BuildMediaUpdateRequest()
    {
        return new MediaUpdateRequest
        {
            FileName = "updated-media.mp4",
            Quality = "HD",
            IsPrimary = true
        };
    }

    // Missing DTO creation methods
    public ChannelDto CreateChannelDto(Action<ChannelDto>? customize = null)
    {
        var dto = new ChannelDto
        {
            Id = _fixture.Create<string>(),
            Type = "Channel",
            Attributes = new ChannelAttributes
            {
                Name = _fixture.Create<string>() + " Channel",
                Description = _fixture.Create<string>(),
                Published = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public EpisodeAnalyticsDto CreateEpisodeAnalyticsDto(Action<EpisodeAnalyticsDto>? customize = null)
    {
        var dto = new EpisodeAnalyticsDto
        {
            Id = _fixture.Create<string>(),
            Type = "EpisodeAnalytics",
            Attributes = new EpisodeAnalyticsAttributesDto
            {
                EpisodeId = _fixture.Create<string>(),
                ViewCount = _fixture.Create<int>() % 1000 + 1,
                DownloadCount = _fixture.Create<int>() % 500 + 1,
                AverageWatchTimeSeconds = _fixture.Create<double>() % 3600 + 60,
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SeriesAnalyticsDto CreateSeriesAnalyticsDto(Action<SeriesAnalyticsDto>? customize = null)
    {
        var dto = new SeriesAnalyticsDto
        {
            Id = _fixture.Create<string>(),
            Type = "SeriesAnalytics",
            Attributes = new SeriesAnalyticsAttributesDto
            {
                TotalViews = _fixture.Create<int>() % 50000,
                TotalDownloads = _fixture.Create<int>() % 25000,
                TotalPlays = _fixture.Create<int>() % 30000
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public PublishingReportDto CreatePublishingReportDto(Action<PublishingReportDto>? customize = null)
    {
        var dto = new PublishingReportDto
        {
            Id = _fixture.Create<string>(),
            Type = "PublishingReport",
            Attributes = new PublishingReportAttributesDto
            {
                Summary = _fixture.Create<string>(),
                TotalViews = _fixture.Create<int>() % 100000,
                UniqueViewers = _fixture.Create<int>() % 50000,
                AverageWatchTime = _fixture.Create<int>() % 3600,
                TotalViewCount = _fixture.Create<int>() % 100000,
                TotalDownloadCount = _fixture.Create<int>() % 50000,
                EpisodeCount = _fixture.Create<int>() % 100 + 1
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    // Registrations Module Test Data
    public SignupDto CreateSignupDto(Action<SignupDto>? customize = null)
    {
        var dto = new SignupDto
        {
            Id = _fixture.Create<string>(),
            Type = "Signup",
            Attributes = new SignupAttributesDto
            {
                Name = _fixture.Create<string>() + " Event",
                Description = _fixture.Create<string>(),
                OpenAt = DateTime.UtcNow.AddDays(-7),
                CloseAt = DateTime.UtcNow.AddDays(6),
                RegistrationLimit = _fixture.Create<int>() % 100 + 10,
                RegistrationCount = _fixture.Create<int>() % 50,
                WaitlistEnabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public RegistrationDto CreateRegistrationDto(Action<RegistrationDto>? customize = null)
    {
        var dto = new RegistrationDto
        {
            Id = _fixture.Create<string>(),
            Type = "Registration",
            Attributes = new RegistrationAttributesDto
            {
                Status = "confirmed",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public AttendeeDto CreateAttendeeDto(Action<AttendeeDto>? customize = null)
    {
        var dto = new AttendeeDto
        {
            Id = _fixture.Create<string>(),
            Type = "Attendee",
            Attributes = new AttendeeAttributesDto
            {
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                Email = _fixture.Create<string>() + "@example.com",
                PhoneNumber = $"{_fixture.Create<int>() % 900 + 100}-{_fixture.Create<int>() % 900 + 100}-{_fixture.Create<int>() % 9000 + 1000}",
                Status = "confirmed",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public PeopleModels.PersonDto CreatePersonDto(Action<PeopleModels.PersonDto>? customize = null)
    {
        var dto = new PeopleModels.PersonDto
        {
            Id = _fixture.Create<string>(),
            Type = "Person",
            Attributes = new PeopleModels.PersonAttributesDto
            {
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                Birthdate = DateTime.UtcNow.AddYears(-25).AddDays(_fixture.Create<int>() % 365),
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public CampusDto CreateCampusDto(Action<CampusDto>? customize = null)
    {
        var dto = new CampusDto
        {
            Id = _fixture.Create<string>(),
            Type = "Campus",
            Attributes = new CampusAttributesDto
            {
                Name = _fixture.Create<string>() + " Campus",
                Description = _fixture.Create<string>(),
                Timezone = "America/New_York",
                Address = _fixture.Create<string>(),
                Street = _fixture.Create<string>(),
                City = _fixture.Create<string>(),
                State = _fixture.Create<string>(),
                PostalCode = _fixture.Create<string>(),
                Zip = _fixture.Create<string>(),
                Country = "US",
                PhoneNumber = $"{_fixture.Create<int>() % 900 + 100}-{_fixture.Create<int>() % 900 + 100}-{_fixture.Create<int>() % 9000 + 1000}",
                Active = true,
                SortOrder = _fixture.Create<int>() % 100,
                SignupCount = _fixture.Create<int>() % 50,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
                UpdatedAt = DateTimeOffset.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public SelectionTypeDto CreateSelectionTypeDto(Action<SelectionTypeDto>? customize = null)
    {
        var dto = new SelectionTypeDto
        {
            Id = _fixture.Create<string>(),
            Type = "SelectionType",
            Attributes = new SelectionTypeAttributesDto
            {
                Name = _fixture.Create<string>() + " Selection",
                Description = _fixture.Create<string>(),
                Required = false,
                AllowMultiple = false,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
                UpdatedAt = DateTimeOffset.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public CategoryDto CreateCategoryDto(Action<CategoryDto>? customize = null)
    {
        var dto = new CategoryDto
        {
            Id = _fixture.Create<string>(),
            Type = "Category",
            Attributes = new CategoryAttributesDto
            {
                Name = _fixture.Create<string>() + " Category",
                Description = _fixture.Create<string>(),
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize?.Invoke(dto);
        return dto;
    }

    public CountDto CreateCountDto(Action<CountDto>? customize = null)
    {
        var dto = new CountDto
        {
            Id = _fixture.Create<string>(),
            Type = "Count",
            Attributes = new CountAttributesDto
            {
                Count = _fixture.Create<int>() % 1000
            }
        };
        customize?.Invoke(dto);
        return dto;
    }
}