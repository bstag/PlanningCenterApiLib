using System;
using System.Collections.Generic;
using AutoFixture;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.JsonApi.Webhooks;

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
}