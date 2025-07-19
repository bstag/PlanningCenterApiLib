using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PlanningCenter.Api.Client.Models.Registrations;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;

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
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<dynamic>>("/publishing/v2/episodes/nonexistent", new JsonApiSingleResponse<dynamic>());
            
            // Speaker not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<dynamic>>("/publishing/v2/speakers/nonexistent", new JsonApiSingleResponse<dynamic>());
            
            // Series not found stub for GetSeriesAsync test
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<dynamic>>("/publishing/v2/series/nonexistent", new JsonApiSingleResponse<dynamic>());
            
            // Speaker not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<dynamic>>("/publishing/v2/speakers/nonexistent", new JsonApiSingleResponse<dynamic>());
            
            // Media not found stub
            mockApiConnection.SetupGetResponse<JsonApiSingleResponse<dynamic>>("/publishing/v2/media/nonexistent", new JsonApiSingleResponse<dynamic>());
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
            dynamic selectionTypeDto = new ExpandoObject();
            selectionTypeDto.id = "type123";
            selectionTypeDto.type = "SelectionType";
            
            dynamic selectionTypeAttributes = new ExpandoObject();
            selectionTypeAttributes.name = "Test Selection Type";
            selectionTypeAttributes.description = "Test Description";
            
            selectionTypeDto.attributes = selectionTypeAttributes;
            
            var selectionTypeResponse = new JsonApiSingleResponse<dynamic> { Data = selectionTypeDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/type123", selectionTypeResponse);
            
            // Person stub
            var personDto = CreatePersonDto();
            var personResponse = new JsonApiSingleResponse<dynamic> { Data = personDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/people/person123", personResponse);
            
            // Campus stub
            var campusDto = CreateCampusDto();
            var campusResponse = new JsonApiSingleResponse<dynamic> { Data = campusDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses/campus123", campusResponse);
            
            // Campuses list stub
            var campusesResponse = CreateCampusesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses", campusesResponse);
            
            // Emergency contact stub
            dynamic contactDto = new ExpandoObject();
            contactDto.id = "contact123";
            contactDto.type = "EmergencyContact";
            dynamic contactAttributes = new ExpandoObject();
            contactAttributes.name = "Jane Doe";
            contactAttributes.relationship = "Spouse";
            contactAttributes.phone = "555-123-4567";
            
            contactDto.attributes = contactAttributes;
            
            var contactResponse = new JsonApiSingleResponse<dynamic> { Data = contactDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123/emergency_contact", contactResponse);

            // Publishing analytics/report stub with proper structure
            dynamic publishingReport = new ExpandoObject();
            publishingReport.data = new ExpandoObject();
            publishingReport.data.id = "report123";
            publishingReport.data.type = "PublishingReport";
            publishingReport.data.attributes = new ExpandoObject();
            publishingReport.data.attributes.summary = "Test Report";
            publishingReport.data.attributes.total_views = 1000;
            publishingReport.data.attributes.unique_viewers = 500;
            publishingReport.data.attributes.average_watch_time = 300;
            publishingReport.data.attributes.total_view_count = 1000;
            publishingReport.data.attributes.total_download_count = 500;
            publishingReport.data.attributes.episode_count = 10;
            mockApiConnection.SetupGetResponse("/publishing/v2/reports/summary", publishingReport);
            
            // Series analytics stub
            mockApiConnection.SetupGetResponse("/publishing/v2/series/series123/analytics", publishingReport);
            
            // Client health check stub
            dynamic healthResponse = new ExpandoObject();
            healthResponse.status = "healthy";
            healthResponse.response_time_ms = 150.0;
            mockApiConnection.SetupGetResponse("/health", healthResponse);
            
            // API limits stub
            dynamic limitsResponse = new ExpandoObject();
            limitsResponse.rate_limit = 1000;
            limitsResponse.remaining = 995;
            limitsResponse.reset_time = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
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
            var registrationResponse = new JsonApiSingleResponse<dynamic> { Data = registrationDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/registrations/reg123", registrationResponse);
            
            // Category stub
            var categoryDto = CreateCategoryDto();
            var categoryResponse = new JsonApiSingleResponse<dynamic> { Data = categoryDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/categories/category123", categoryResponse);
            
            // Categories list stub
            var categoriesResponse = new PagedResponse<dynamic>
            {
                Data = new List<dynamic> { categoryDto },
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 100, CurrentPage = 1, TotalPages = 1 }
            };
            mockApiConnection.SetupGetResponse("/registrations/v2/categories", categoriesResponse);
            
            // Selection type stub
           mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/selectiontype123", selectionTypeResponse);
            
            // Attendees list stub
            var attendeesResponse = CreateAttendeesResponse();
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees", attendeesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?filter[on_waitlist]=true", attendeesResponse);
            
            // Individual attendee stub
            var attendeeDto = CreateAttendeeDto();
            var attendeeResponse = new JsonApiSingleResponse<dynamic> { Data = attendeeDto };
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
            mockApiConnection.SetupGetResponse<object>("/publishing/v2/episodesper_page=100", null);
            // Defensive: handle series nonexistent endpoint for publishing
            mockApiConnection.SetupGetResponse<object>("/publishing/v2/series/nonexistent", null);
            // Defensive: handle speakers nonexistent endpoint for publishing
            mockApiConnection.SetupGetResponse<object>("/publishing/v2/speakers/nonexistent", null);
            // Defensive: handle paged episodes endpoint for publishing
            mockApiConnection.SetupGetResponse<object>("/publishing/v2/episodes?per_page=100", null);
            // Defensive: handle paged signups endpoint for registrations
            mockApiConnection.SetupGetResponse<object>("/registrations/v2/signups?per_page=100", signupsResponse);
            // Defensive: handle paged attendees endpoint typo
            mockApiConnection.SetupGetResponse<object>("/registrations/v2/signups/signup123/attendeeswhere[on_waitlist]=true", attendeesResponse);
            // Defensive: handle paged signups endpoint typo
            mockApiConnection.SetupGetResponse<object>("/registrations/v2/signupsper_page=100", null);
            // Defensive: handle paged registrations endpoint typo
            mockApiConnection.SetupGetResponse<object>("/registrations/v2/signups/signup123/registrationswhere[status]=confirmed", null);
            // Defensive: handle paged attendees endpoint typo
            mockApiConnection.SetupGetResponse<object>("/registrations/v2/signups/signup123/attendeeswhere[on_waitlist]=true", null);

            // Defensive: handle campuses paged endpoint
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses?per_page=100", campusesResponse);
            // Defensive: handle registrations paged endpoint
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?per_page=100", registrationsResponse);
            
            // Signup location stub
            var locationDto = CreateLocationDto();
            var locationResponse = new JsonApiSingleResponse<dynamic> { Data = locationDto };
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/location", locationResponse);
            
            // Categories stub
            // Patch: Ensure categories have id and description for dynamic binding
            var categoriesList = new List<dynamic>();
            dynamic category = new ExpandoObject();
            category.id = "category123";
            category.type = "Category";
            dynamic catAttrs = new ExpandoObject();
            catAttrs.name = "Test Category";
            catAttrs.description = "Test Description";
            category.attributes = catAttrs;
            categoriesList.Add(category);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/categories", categoriesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/categories?per_page=100", categoriesResponse);

            // Defensive: ensure GET stub for /registrations/v2/signups?per_page=100
            mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", CreateSignupsResponse());
            
            // Defensive: handle malformed episodes endpoint
            mockApiConnection.SetupGetResponse("/publishing/v2/episodesper_page=100", CreateEpisodesResponse());

            // Patch: Ensure campuses have id and description for dynamic binding
            dynamic campus = new ExpandoObject();
            campus.id = "campus123";
            campus.type = "Campus";
            dynamic campusAttrs = new ExpandoObject();
            campusAttrs.name = "Test Campus";
            campusAttrs.description = "Test Description";
            campus.attributes = campusAttrs;

            // If campusesResponse.Data is a List<dynamic>, add the new campus
            if (campusesResponse.Data is List<dynamic> campusesList)
            {
                campusesList.Add(campus);
            }
            else
            {
                // If not, replace Data with a List<dynamic> containing the new campus
                campusesResponse.Data = new List<dynamic> { campus };
            }

            mockApiConnection.SetupGetResponse("/registrations/v2/campuses", campusesResponse);
            mockApiConnection.SetupGetResponse("/registrations/v2/campuses?per_page=100", campusesResponse);
            
            // Set up mutation responses
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups", new JsonApiSingleResponse<dynamic> { Data = CreateSignupDto("new-signup") });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123", new JsonApiSingleResponse<dynamic> { Data = CreateSignupDto("signup123") });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/registrations", new JsonApiSingleResponse<dynamic> { Data = CreateRegistrationDto() });
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/attendees", new JsonApiSingleResponse<dynamic> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123", new JsonApiSingleResponse<dynamic> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/attendees/attendee123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/waitlist", new JsonApiSingleResponse<dynamic> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/waitlist/attendee123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/waitlist/attendee123/promote", new JsonApiSingleResponse<dynamic> { Data = CreateAttendeeDto() });
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/locations", new JsonApiSingleResponse<dynamic> { Data = CreateLocationDto() });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/locations/location123", new JsonApiSingleResponse<dynamic> { Data = CreateLocationDto() });

            dynamic signupTimeDto = new ExpandoObject();
            signupTimeDto.id = "time123";
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/times", new JsonApiSingleResponse<dynamic> { Data = signupTimeDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/times/time123", new JsonApiSingleResponse<dynamic> { Data = signupTimeDto });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/times/time123", new JsonApiSingleResponse<dynamic>());
            
            // Create selection type and emergency contact DTOs with proper id properties
            dynamic selectionTypeMutationDto = new ExpandoObject();
            selectionTypeMutationDto.id = "selectiontype123";
            selectionTypeMutationDto.type = "SelectionType";
            
            dynamic emergencyContactDto = new ExpandoObject();
            emergencyContactDto.id = "newcontact123";
            emergencyContactDto.type = "EmergencyContact";
            
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/selection_types", new JsonApiSingleResponse<dynamic> { Data = selectionTypeMutationDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/selection_types/selectiontype123", new JsonApiSingleResponse<dynamic> { Data = selectionTypeMutationDto });
            mockApiConnection.SetupMutationResponse("DELETE", "/registrations/v2/signups/signup123/selection_types/selectiontype123", new JsonApiSingleResponse<dynamic>());
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/attendees/attendee123/emergency_contact", new JsonApiSingleResponse<dynamic> { Data = emergencyContactDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123/emergency_contact", new JsonApiSingleResponse<dynamic> { Data = emergencyContactDto });

            categoryDto.id = "category123";
            mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/categories", new JsonApiSingleResponse<dynamic> { Data = categoryDto });
            mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/categories/category123", new JsonApiSingleResponse<dynamic> { Data = categoryDto });
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

        private static PagedResponse<dynamic> CreateSignupTimesResponse()
        {
            var times = new List<dynamic>();
            
            dynamic time = new ExpandoObject();
            time.id = "time123";
            time.type = "SignupTime";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Morning Session";
            attributes.starts_at = "2023-01-01T09:00:00Z";
            attributes.ends_at = "2023-01-01T12:00:00Z";
            
            time.attributes = attributes;
            times.Add(time);
            
            return new PagedResponse<dynamic>
            {
                Data = times,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static dynamic CreateSelectionTypeDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "type123";
            dto.type = "SelectionType";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Test Selection Type";
            attributes.description = "Test Description";
            
            dto.attributes = attributes;
            return dto;
        }

        private static dynamic CreatePersonDto()
        {
            dynamic person = new ExpandoObject();
            person.id = "person123";
            person.type = "person";
            person.attributes = new ExpandoObject();
            person.attributes.first_name = "John";
            person.attributes.last_name = "Doe";
            person.attributes.email = "john.doe@example.com";
            person.attributes.phone = "555-1234";
            person.attributes.birthdate = "1990-01-01";
            person.attributes.timezone = "America/New_York";
            person.attributes.created_at = "2023-01-01T09:00:00Z";
            person.attributes.updated_at = "2023-01-01T09:00:00Z";
            return person;
        }

        private static dynamic CreateCampusDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "campus123";
            dto.type = "Campus";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Main Campus";
            attributes.description = "Main campus location";
            attributes.timezone = "America/New_York";
            attributes.address = "123 Main St";
            attributes.city = "Anytown";
            attributes.state = "NY";
            attributes.postal_code = "12345";
            attributes.country = "US";
            attributes.phone_number = "555-123-4567";
            attributes.website_url = "https://example.com";
            attributes.active = true;
            attributes.sort_order = 1;
            attributes.signup_count = 10;
            
            dto.attributes = attributes;
            return dto;
        }

        private static PagedResponse<dynamic> CreateCampusesResponse()
        {
            dynamic response = new ExpandoObject();
            response.data = new List<dynamic>
            {
                new ExpandoObject()
            };
            response.data[0].id = "campus1";
            response.data[0].attributes = new ExpandoObject();
            response.data[0].attributes.name = "Main Campus";
            response.data[0].attributes.description = "Main campus description";
            return response;
        }

        private static dynamic CreateEmergencyContactDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "contact123";
            dto.type = "EmergencyContact";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Jane Doe";
            attributes.relationship = "Spouse";
            attributes.phone = "555-123-4567";
            
            dto.attributes = attributes;
            return dto;
        }

        private static PagedResponse<dynamic> CreateRegistrationsResponse()
        {
            var registrations = new List<dynamic>();
            
            dynamic registration = new ExpandoObject();
            registration.id = "reg123";
            registration.type = "Registration";
            
            dynamic attributes = new ExpandoObject();
            attributes.status = "confirmed";
            attributes.created_at = "2023-01-01T09:00:00Z";
            
            registration.attributes = attributes;
            registrations.Add(registration);
            
            return new PagedResponse<dynamic>
            {
                Data = registrations,
                Meta = new PagedResponseMeta { TotalCount = 25, PerPage = 10, CurrentPage = 1, TotalPages = 3 }
            };
        }

        private static PagedResponse<dynamic> CreateAttendeesResponse()
        {
            var attendees = new List<dynamic>();
            
            dynamic attendee = new ExpandoObject();
            attendee.id = "attendee123";
            attendee.type = "Attendee";
            
            dynamic attributes = new ExpandoObject();
            attributes.first_name = "John";
            attributes.last_name = "Doe";
            attributes.on_waitlist = true;
            
            attendee.attributes = attributes;
            attendees.Add(attendee);
            
            return new PagedResponse<dynamic>
            {
                Data = attendees,
                Meta = new PagedResponseMeta { TotalCount = 5, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static PagedResponse<dynamic> CreateSignupsResponse()
        {
            var signups = new List<dynamic>();
            
            dynamic signup = new ExpandoObject();
            signup.id = "signup123";
            signup.type = "Signup";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Test Signup";
            attributes.description = "Test Description";
            attributes.status = "active";
            
            signup.attributes = attributes;
            signups.Add(signup);
            
            return new PagedResponse<dynamic>
            {
                Data = signups,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        private static dynamic CreateRegistrationDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "registration123";
            dto.type = "Registration";
            
            dynamic attributes = new ExpandoObject();
            attributes.first_name = "John";
            attributes.last_name = "Doe";
            attributes.email = "john.doe@example.com";
            attributes.phone = "555-123-4567";
            attributes.birthdate = "1990-01-01";
            attributes.gender = "Male";
            attributes.status = "confirmed";
            attributes.created_at = "2023-01-01T09:00:00Z";
            attributes.updated_at = "2023-01-01T09:00:00Z";
            
            dto.attributes = attributes;
            return dto;
        }
        
        private static dynamic CreateCategoryDto()
        {
            dynamic category = new ExpandoObject();
            category.id = "category123";
            category.type = "category";
            category.attributes = new ExpandoObject();
            category.attributes.name = "Test Category";
            category.attributes.description = "Test category description";
            category.attributes.color = "#FF0000";
            category.attributes.sort_order = 1;
            category.attributes.active = true;
            category.attributes.signup_count = 10;
            category.attributes.created_at = "2023-01-01T09:00:00Z";
            category.attributes.updated_at = "2023-01-01T09:00:00Z";
            return category;
        }
        
        private static dynamic CreateAttendeeDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "attendee123";
            dto.type = "Attendee";
            
            dynamic attributes = new ExpandoObject();
            attributes.first_name = "John";
            attributes.last_name = "Doe";
            attributes.on_waitlist = false;
            attributes.created_at = "2023-01-01T09:00:00Z";
            
            dto.attributes = attributes;
            return dto;
        }
        
        private static dynamic CreateLocationDto()
        {
            dynamic dto = new ExpandoObject();
            dto.id = "location123";
            dto.type = "Location";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Main Building";
            attributes.address = "123 Main St";
            
            dto.attributes = attributes;
            return dto;
        }
        
        private static PagedResponse<dynamic> CreateCategoriesResponse()
        {
            var categories = new List<dynamic>();
            
            dynamic category = new ExpandoObject();
            category.id = "category123";
            category.type = "Category";
            
            dynamic attributes = new ExpandoObject();
            attributes.name = "Test Category";
            
            category.attributes = attributes;
            categories.Add(category);
            
            return new PagedResponse<dynamic>
            {
                Data = categories,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 10, CurrentPage = 1, TotalPages = 1 }
            };
        }

        
        private static dynamic CreateAnalyticsResponse()
        {
            dynamic analytics = new ExpandoObject();
            analytics.data = new ExpandoObject();
            analytics.data.views = 42;
            analytics.data.downloads = 5;
            return analytics;
        }
        
        private static PagedResponse<dynamic> CreateEpisodesResponse()
        {
            var episodes = new List<dynamic>();
            
            dynamic episode = new ExpandoObject();
            episode.id = "episode123";
            episode.type = "Episode";
            
            dynamic attributes = new ExpandoObject();
            attributes.title = "Test Episode";
            attributes.description = "Test Description";
            
            episode.attributes = attributes;
            episodes.Add(episode);
            
            return new PagedResponse<dynamic>
            {
                Data = episodes,
                Meta = new PagedResponseMeta { TotalCount = 1, PerPage = 100, CurrentPage = 1, TotalPages = 1 }
            };
        }
    }
    #endregion
}
