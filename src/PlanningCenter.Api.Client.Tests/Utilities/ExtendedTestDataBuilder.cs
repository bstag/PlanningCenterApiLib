using System;
using AutoFixture;
using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;

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
}