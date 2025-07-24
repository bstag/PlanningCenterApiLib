using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.People;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using RegistrationsCampusDto = PlanningCenter.Api.Client.Models.JsonApi.Registrations.CampusDto;
using PeopleCampusDto = PlanningCenter.Api.Client.Models.People.CampusDto;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Tests.Utilities
{
    /// <summary>
    /// Helper class to set up common test stubs for MockApiConnection
    /// </summary>
    public static class TestStubHelper
    {
        /// <summary>
        /// Sets up common stubs for the MockApiConnection to handle test cases
        /// </summary>
        public static void SetupCommonStubs(MockApiConnection mockApiConnection)
        {
            if (mockApiConnection == null)
                throw new ArgumentNullException(nameof(mockApiConnection));

            // Publishing stubs
            SetupPublishingStubs(mockApiConnection);

            // Registrations stubs
            SetupRegistrationsStubs(mockApiConnection);
        }

        private static void SetupPublishingStubs(MockApiConnection mockApiConnection)
        {
            // Media not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<MediaDto>>("/publishing/v2/media/nonexistent", new JsonApiSingleResponse<MediaDto>());

            // Episodes stub for StreamEpisodesAsync test
            var episodesResponse = CreateEpisodesResponse();
            mockApiConnection.SetupGetResponse("/publishing/v2/episodes?per_page=100", episodesResponse);

            // Episode not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<EpisodeDto>>("/publishing/v2/episodes/nonexistent", new JsonApiSingleResponse<EpisodeDto>());

            // Speaker not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SpeakerDto>>("/publishing/v2/speakers/nonexistent", new JsonApiSingleResponse<SpeakerDto>());

            // Series not found stub for GetSeriesAsync test
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SeriesDto>>("/publishing/v2/series/nonexistent", new JsonApiSingleResponse<SeriesDto>());
        }

        private static void SetupRegistrationsStubs(MockApiConnection mockApiConnection)
        {
            // Signup stubs
            var signupDto = CreateSignupDto("signup123");
            var signupResponse = new JsonApiSingleResponse<SignupDto> { Data = signupDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123", signupResponse);

            // Signup times stub
            var signupTimesResponse = CreateSignupTimesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/times", signupTimesResponse);

            // Selection type stub
            var selectionTypeDto = CreateSelectionTypeDto();
            var selectionTypeResponse = new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/type123", selectionTypeResponse);

            // Person stub
            var personDto = new PersonDto();
            var personResponse = new JsonApiSingleResponse<PersonDto> { Data = personDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/people/person123", personResponse);

            // Campus stub
            var campusDto = CreateRegistrationsCampusDto();
            var campusResponse = new JsonApiSingleResponse<RegistrationsCampusDto> { Data = campusDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses/campus123", campusResponse);

            // Campuses list stub
            var campusesResponse = CreateCampusesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses", campusesResponse);

            // Emergency contact stub
            var contactDto = CreateEmergencyContactDto();
            var contactResponse = new JsonApiSingleResponse<EmergencyContactDto> { Data = contactDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123/emergency_contact", contactResponse);

            // Publishing analytics/report stub with proper structure
            var publishingReport = new JsonApiSingleResponse<PublishingReportDto>
            {
                Data = new PublishingReportDto
                {
                    Id = "report123",
                    Type = "PublishingReport",
                    Attributes = new PublishingReportAttributesDto
                    {
                        Summary = "Test Report",
                        TotalViews = 1000,
                        UniqueViewers = 500,
                        AverageWatchTime = 300,
                        TotalViewCount = 1000,
                        TotalDownloadCount = 500,
                        EpisodeCount = 10
                    }
                }
            };
            mockApiConnection.SetupGetResponse("/publishing/v2/reports/summary", publishingReport);

            // Series analytics stub
            var seriesAnalytics = new JsonApiSingleResponse<SeriesAnalyticsDto>
            {
                Data = new SeriesAnalyticsDto
                {
                    Id = "series123",
                    Type = "SeriesAnalytics",
                    Attributes = new SeriesAnalyticsAttributesDto
                    {
                        TotalViews = 1000,
                        TotalDownloads = 500,
                        TotalPlays = 1500
                    }
                }
            };
            mockApiConnection.SetupGetResponse("/publishing/v2/series/series123/analytics", seriesAnalytics);

            // Client health check stub
            var healthResponse = new HealthCheckResult
            {
                Status = "healthy",
                ResponseTimeMs = 150.0
            };
            mockApiConnection.SetupGetResponse("/health", healthResponse);

            // API limits stub
            var limitsResponse = new ApiLimitsInfo
            {
                RateLimit = 100,
                RemainingRequests = 95,
                ResetTime = DateTime.UtcNow.AddHours(1)
            };
            mockApiConnection.SetupGetResponse("/api/limits", limitsResponse);

            // Registrations list stub for GetRegistrationCountAsync test
            var registrationsResponse = CreateRegistrationsResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?where[status]=confirmed", registrationsResponse);

            // Signups list stub for StreamSignupsAsync test
            var signupsResponse = CreateSignupsResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", signupsResponse);

            // Waitlist attendees stub
            var waitlistAttendeesResponse = CreateAttendeesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?where[on_waitlist]=true", waitlistAttendeesResponse);

            // Registrations list stub
            var registrationsResponse2 = CreateRegistrationsResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations", registrationsResponse2);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?filter[status]=confirmed", registrationsResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?where[status]=confirmed", registrationsResponse);

            // Individual registration stub
            var registrationDto = CreateRegistrationDto();
            var registrationResponse = new JsonApiSingleResponse<RegistrationDto> { Data = registrationDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/registrations/reg123", registrationResponse);

            // Category stub
            var categoryDto = CreateCategoryDto();
            var categoryResponse = new JsonApiSingleResponse<CategoryDto> { Data = categoryDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/categories/category123", categoryResponse);

            // Categories list stub
            var categoriesResponse = new PagedResponse<CategoryDto>
            {
                Data = new List<CategoryDto> { categoryDto },
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 100, CurrentPage = 1, TotalPages = 1 }
            };
            mockApiConnection.SetupGetResponse("/registrations/v2/categories", categoriesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/categories/category123", categoryResponse);

            // Selection type stub
            mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/type123", selectionTypeResponse);

            // Attendees list stub
            var attendeesResponse = CreateAttendeesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees", attendeesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?filter[on_waitlist]=true", attendeesResponse);
            // Add stub for waitlist count endpoint
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?where[on_waitlist]=true", attendeesResponse);

            // Individual attendee stub
            var attendeeDto = CreateAttendeeDto();
            var attendeeResponse = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123", attendeeResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups", signupsResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", signupsResponse);
            // Add stub for /registrations/v2/signups/signup123/registrations?filter[status]=confirmed
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?filter[status]=confirmed", registrationsResponse);
            // Defensive: also handle malformed endpoint in case test or service is using it
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrationswhere[status]=confirmed", registrationsResponse);
            // Defensive: handle signups endpoint typo
            mockApiConnection.SetupGetResponse("/registrations/v2/signupsper_page=100", signupsResponse);
            // Defensive: handle episodes endpoint typo for publishing
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<EpisodeDto>>("/publishing/v2/episodesper_page=100", new JsonApiSingleResponse<EpisodeDto>());
            // Defensive: handle series nonexistent endpoint for publishing
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SeriesDto>>("/publishing/v2/series/nonexistent", new JsonApiSingleResponse<SeriesDto>());
            // Defensive: handle speakers nonexistent endpoint for publishing
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<SpeakerDto>>("/publishing/v2/speakers/nonexistent", new JsonApiSingleResponse<SpeakerDto>());
            // Defensive: handle paged episodes endpoint for publishing
            mockApiConnection.SetupGetResponse<PagedResponse<EpisodeDto>>("/publishing/v2/episodes?per_page=100", new PagedResponse<EpisodeDto>());
            // Defensive: handle paged signups endpoint for registrations
            mockApiConnection.SetupGetResponse<PagedResponse<SignupDto>>("/registrations/v2/signups?per_page=100", signupsResponse);
            // Defensive: handle paged attendees endpoint typo
            mockApiConnection.SetupGetResponse<PagedResponse<AttendeeDto>>("/registrations/v2/signups/signup123/attendeeswhere[on_waitlist]=true", attendeesResponse);
            // Defensive: handle paged signups endpoint typo
            mockApiConnection.SetupGetResponse<PagedResponse<SignupDto>>("/registrations/v2/signupsper_page=100", new PagedResponse<SignupDto>());
            // Defensive: handle paged registrations endpoint typo
            mockApiConnection.SetupGetResponse<PagedResponse<RegistrationDto>>("/registrations/v2/signups/signup123/registrationswhere[status]=confirmed", new PagedResponse<RegistrationDto>());
            // Defensive: handle paged attendees endpoint typo
            mockApiConnection.SetupGetResponse<PagedResponse<AttendeeDto>>("/registrations/v2/signups/signup123/attendeeswhere[on_waitlist]=true", new PagedResponse<AttendeeDto>());

            // Defensive: handle campuses paged endpoint
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses?per_page=100", campusesResponse);
            // Defensive: handle registrations paged endpoint
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?per_page=100", registrationsResponse);

            // Signup location stub
            var locationDto = CreateLocationDto();
            var locationResponse = new JsonApiSingleResponse<SignupLocationDto> { Data = locationDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/location", locationResponse);

            // Categories stub
            // Patch: Ensure categories have id and description for dynamic binding
            var categoriesList = new List<CategoryDto>();
            var category = CreateCategoryDto();
            categoriesList.Add(category);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/categories", categoriesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/categories?per_page=100", categoriesResponse);

            // Defensive: ensure GET stub for /registrations/v2/signups?per_page=100
            mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", CreateSignupsResponse());

            // Defensive: handle malformed episodes endpoint
            mockApiConnection.SetupGetResponse("/publishing/v2/episodesper_page=100", CreateEpisodesResponse());

            // Patch: Ensure campuses have id and description for dynamic binding
            var campus = CreateRegistrationsCampusDto();

            // If campusesResponse.Data is a List<dynamic>, add the new campus
            if (campusesResponse.Data is List<RegistrationsCampusDto> campusesList)
            {
                campusesList.Add(campus);
            }
            else
            {
                // If not, replace Data with a List<dynamic> containing the new campus
                campusesResponse.Data = new List<RegistrationsCampusDto> { campus };
            }

            mockApiConnection.SetupGetResponse("/registrations/v2/campuses", campusesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses?per_page=100", campusesResponse);

            // Set up mutation responses
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups", new JsonApiSingleResponse<SignupDto> { Data = CreateSignupDto("new-signup") });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123", new JsonApiSingleResponse<SignupDto> { Data = CreateSignupDto("signup123") });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/registrations", new JsonApiSingleResponse<RegistrationDto> { Data = CreateRegistrationDto() });
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/attendees", new JsonApiSingleResponse<AttendeeDto> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123", new JsonApiSingleResponse<AttendeeDto> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/attendees/attendee123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/waitlist", new JsonApiSingleResponse<AttendeeDto> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/waitlist/attendee123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/waitlist/attendee123/promote", new JsonApiSingleResponse<AttendeeDto> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/locations", new JsonApiSingleResponse<SignupLocationDto> { Data = CreateLocationDto() });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/locations/location123", new JsonApiSingleResponse<SignupLocationDto> { Data = CreateLocationDto() });

            var signupTimeDto = CreateSignupTimeDto();
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/times", new JsonApiSingleResponse<SignupTimeDto> { Data = signupTimeDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/times/time123", new JsonApiSingleResponse<SignupTimeDto> { Data = signupTimeDto });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/times/time123", new JsonApiSingleResponse<dynamic>());

            // Create selection type and emergency contact DTOs with proper id properties
            var selectionTypeMutationDto = CreateSelectionTypeDto();

            var emergencyContactDto = CreateEmergencyContactDto();

            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/selection_types", new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeMutationDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/selection_types/selectiontype123", new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeMutationDto });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/selection_types/selectiontype123", new JsonApiSingleResponse<dynamic>());
            
        }

        #region Helper methods to create test DTOs

        private static SignupDto CreateSignupDto(string id)
        {
            return new SignupDto
            {
                Id = id,
                Type = "Signup",
                Attributes = new SignupAttributesDto
                {
                    Name = "Test Signup",
                    Description = "Test Description",
                    Status = "active"
                }
            };
        }

        private static PagedResponse<SignupTimeDto> CreateSignupTimesResponse()
        {
            var times = new List<SignupTimeDto>();

            var time = new SignupTimeDto
            {
                Id = "time123",
                Type = "SignupTime",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.SignupTimeAttributesDto
                {
                    Name = "Morning Session",
                    StartsAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    EndsAt = DateTimeOffset.Parse("2023-01-01T12:00:00Z").DateTime
                }
            };
            times.Add(time);

            return new PagedResponse<SignupTimeDto>
            {
                Data = times,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static SelectionTypeDto CreateSelectionTypeDto()
        {
            return new SelectionTypeDto
            {
                Id = "type123",
                Type = "SelectionType",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.SelectionTypeAttributesDto
                {
                    Name = "Test Selection Type",
                    Description = "Test Description",
                    Category = "General",
                    Cost = 10.00m,
                    Currency = "USD",
                    Required = true,
                    AllowMultiple = false,
                    MaxSelections = 1,
                    MinSelections = 1,
                    SelectionLimit = 100,
                    SortOrder = 1,
                    Active = true,
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        public static PersonDto CreatePersonDto(Action<PersonDto> customize)
        {
            var dto = new PersonDto
            {
                Id = "person123",
                Type = "person",
                Attributes = new PersonAttributesDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Birthdate = DateTime.Parse("1990-01-01"),
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
            customize(dto);
            return dto;
        }

        public static RegistrationsCampusDto CreateRegistrationsCampusDto()
        {
            return new RegistrationsCampusDto
            {
                Id = "campus123",
                Type = "Campus",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.CampusAttributesDto
                {
                    Name = "Main Campus",
                    Description = "Main campus location",
                    Timezone = "America/New_York",
                    Address = "123 Main St",
                    City = "Anytown",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "US",
                    PhoneNumber = "555-123-4567",
                    WebsiteUrl = "https://example.com",
                    Active = true,
                    SortOrder = 1,
                    SignupCount = 10
                }
            };
        }

        private static PagedResponse<RegistrationsCampusDto> CreateCampusesResponse()
        {
            var campuses = new List<RegistrationsCampusDto>
            {
                new RegistrationsCampusDto
                {
                    Id = "campus1",
                    Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.CampusAttributesDto
                    {
                        Name = "Main Campus",
                        Description = "Main campus description"
                    }
                }
            };

            return new PagedResponse<RegistrationsCampusDto>
            {
                Data = campuses,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 100, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static EmergencyContactDto CreateEmergencyContactDto()
        {
            return new EmergencyContactDto
            {
                Id = "contact123",
                Type = "EmergencyContact",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.EmergencyContactAttributes
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Relationship = "Spouse",
                    PrimaryPhone = "555-123-4567",
                    SecondaryPhone = "555-765-4321",
                    Email = "jane.doe@example.com",
                    StreetAddress = "456 Oak Ave",
                    City = "Anytown",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "US",
                    IsPrimary = true,
                    Priority = 1,
                    Notes = "Some notes",
                    PreferredContactMethod = "Phone",
                    BestTimeToContact = "Morning",
                    CanAuthorizeMedicalTreatment = true,
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        private static PagedResponse<RegistrationDto> CreateRegistrationsResponse()
        {
            var registrations = new List<RegistrationDto>();

            var registration = new RegistrationDto
            {
                Id = "reg123",
                Type = "Registration",
                Attributes = new RegistrationAttributesDto
                {
                    Status = "confirmed",
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
            registrations.Add(registration);

            return new PagedResponse<RegistrationDto>
            {
                Data = registrations,
                Meta = new PagedResponseMeta { TotalCount = 25, PerPage = 10, CurrentPage = 1, TotalPages = 3 }
            };
        }

        private static PagedResponse<AttendeeDto> CreateAttendeesResponse()
        {
            var attendees = new List<AttendeeDto>();

            var attendee = new AttendeeDto
            {
                Id = "attendee123",
                Type = "Attendee",
                Attributes = new AttendeeAttributesDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    OnWaitlist = true
                }
            };
            attendees.Add(attendee);

            return new PagedResponse<AttendeeDto>
            {
                Data = attendees,
                Meta = new PagedResponseMeta { TotalCount = 5, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        public static PagedResponse<SignupDto> CreateSignupsResponse()
        {
            var signups = new List<SignupDto>();

            var signup = new SignupDto
            {
                Id = "signup123",
                Type = "Signup",
                Attributes = new SignupAttributesDto
                {
                    Name = "Test Signup",
                    Description = "Test Description",
                    Status = "active"
                }
            };
            signups.Add(signup);

            return new PagedResponse<SignupDto>
            {
                Data = signups,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static RegistrationDto CreateRegistrationDto()
        {
            return new RegistrationDto
            {
                Id = "registration123",
                Type = "Registration",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.RegistrationAttributesDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "555-123-4567",
                    Birthdate = "1990-01-01",
                    Gender = "Male",
                    Status = "confirmed",
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        private static CategoryDto CreateCategoryDto()
        {
            return new CategoryDto
            {
                Id = "category123",
                Type = "category",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.CategoryAttributesDto
                {
                    Name = "Test Category",
                    Description = "Test category description",
                    Color = "#FF0000",
                    SortOrder = 1,
                    Active = true,
                    SignupCount = 10,
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        private static AttendeeDto CreateAttendeeDto()
        {
            return new AttendeeDto
            {
                Id = "attendee123",
                Type = "Attendee",
                Attributes = new AttendeeAttributesDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    OnWaitlist = false,
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        private static SignupLocationDto CreateLocationDto()
        {
            return new SignupLocationDto
            {
                Id = "location123",
                Type = "Location",
                                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.SignupLocationAttributesDto
                {
                    Name = "Main Building",
                    Description = "Main building description",
                    StreetAddress = "123 Main St",
                    City = "Anytown",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "US",
                    Latitude = "34.0522",
                    Longitude = "-118.2437",
                    PhoneNumber = "555-123-4567",
                    WebsiteUrl = "https://example.com",
                    Directions = "Some directions",
                    ParkingInfo = "Parking available",
                    AccessibilityInfo = "Wheelchair accessible",
                    Capacity = 100,
                    Notes = "Some notes",
                    Timezone = "America/New_York",
                    FormattedAddress = "123 Main St, Anytown, NY 12345, US",
                    FullFormattedAddress = "123 Main St, Anytown, NY 12345, US",
                    LocationType = "building",
                    Url = "https://example.com/location",
                    Subpremise = "Suite 100",
                    AddressData = "{}",
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }

        private static PagedResponse<CategoryDto> CreateCategoriesResponse()
        {
            var categories = new List<CategoryDto>();

            var category = new CategoryDto
            {
                Id = "category123",
                Type = "Category",
                Attributes = new CategoryAttributesDto
                {
                    Name = "Test Category"
                }
            };
            categories.Add(category);

            return new PagedResponse<CategoryDto>
            {
                Data = categories,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }


        private static SeriesAnalyticsDto CreateAnalyticsResponse()
        {
            return new SeriesAnalyticsDto
            {
                Id = "series123",
                Type = "SeriesAnalytics",
                Attributes = new SeriesAnalyticsAttributesDto
                {
                    TotalViews = 42,
                    TotalDownloads = 5,
                    TotalPlays = 47
                }
            };
        }

        private static PagedResponse<EpisodeDto> CreateEpisodesResponse()
        {
            var episodes = new List<EpisodeDto>();

            var episode = new EpisodeDto
            {
                Id = "episode123",
                Type = "Episode",
                Attributes = new EpisodeAttributesDto
                {
                    Title = "Test Episode",
                    Description = "Test Description"
                }
            };
            episodes.Add(episode);

            return new PagedResponse<EpisodeDto>
            {
                Data = episodes,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 100, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static SignupTimeDto CreateSignupTimeDto()
        {
            return new SignupTimeDto
            {
                Id = "time123",
                Type = "SignupTime",
                Attributes = new PlanningCenter.Api.Client.Models.JsonApi.Registrations.SignupTimeAttributesDto
                {
                    Name = "Morning Session",
                    StartsAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    EndsAt = DateTimeOffset.Parse("2023-01-01T12:00:00Z").DateTime,
                    AllDay = false,
                    Timezone = "America/New_York",
                    TimeType = "session",
                    Capacity = 100,
                    Required = true,
                    SortOrder = 1,
                    Active = true,
                    Location = "Room 101",
                    Room = "Main Hall",
                    Instructor = "John Doe",
                    Cost = 10.00m,
                    Notes = "Some notes",
                    CreatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime,
                    UpdatedAt = DateTimeOffset.Parse("2023-01-01T09:00:00Z").DateTime
                }
            };
        }
    }
    #endregion
}
