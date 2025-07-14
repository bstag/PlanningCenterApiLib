using System;
using System.Collections.Generic;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Tests.Utilities
{
    /// <summary>
    /// JSON:API response wrapper for collection resources.
    /// </summary>
    public class JsonApiCollectionResponse<T>
    {
        public List<T>? Data { get; set; }
        public PagedResponseMeta? Meta { get; set; }
        public PagedResponseLinks? Links { get; set; }
    }

    /// <summary>
    /// Helper class to build test data for unit tests.
    /// </summary>
    public class TestDataBuilder
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Creates a test person with random data.
        /// </summary>
        public Person BuildPerson(string? id = null)
        {
            return new Person
            {
                Id = id ?? _random.Next(1000, 9999).ToString(),
                FirstName = $"Test{_random.Next(100)}",
                LastName = $"Person{_random.Next(100)}",
                // Note: Person in Core namespace doesn't have Email property
                Birthdate = DateTime.Now.AddYears(-30).AddDays(_random.Next(365)),
                CreatedAt = DateTime.Now.AddDays(-_random.Next(100)),
                UpdatedAt = DateTime.Now.AddDays(-_random.Next(10))
            };
        }

        /// <summary>
        /// Creates a test person DTO with random data.
        /// </summary>
        public PersonDto BuildPersonDto(string? id = null)
        {
            id ??= _random.Next(1000, 9999).ToString();
            
            return new PersonDto
            {
                Id = id,
                Type = "Person",
                Attributes = new PersonAttributesDto
                {
                    FirstName = $"Test{_random.Next(100)}",
                    LastName = $"Person{_random.Next(100)}",
                    // Note: PersonAttributesDto doesn't have Email property
                    Birthdate = DateTime.Now.AddYears(-30).AddDays(_random.Next(365)),
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(100)),
                    UpdatedAt = DateTime.Now.AddDays(-_random.Next(10))
                }
            };
        }
        
        /// <summary>
        /// Creates a test person DTO with random data and allows customization.
        /// </summary>
        public PersonDto CreatePersonDto(Action<PersonDto> customize)
        {
            var dto = BuildPersonDto();
            customize(dto);
            return dto;
        }

        /// <summary>
        /// Creates a test phone number with random data.
        /// </summary>
        public PhoneNumber BuildPhoneNumber(string? id = null)
        {
            return new PhoneNumber
            {
                Id = id ?? _random.Next(1000, 9999).ToString(),
                Number = $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                Location = "Mobile",
                Primary = true,
                CreatedAt = DateTime.Now.AddDays(-_random.Next(100)),
                UpdatedAt = DateTime.Now.AddDays(-_random.Next(10))
            };
        }

        /// <summary>
        /// Creates a test phone number DTO with random data.
        /// </summary>
        public PhoneNumberDto BuildPhoneNumberDto(string? id = null)
        {
            id ??= _random.Next(1000, 9999).ToString();
            
            return new PhoneNumberDto
            {
                Id = id,
                Type = "PhoneNumber",
                Attributes = new PhoneNumberAttributesDto
                {
                    Number = $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                    Location = "Mobile",
                    Primary = true,
                    Carrier = true,
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(100)),
                    UpdatedAt = DateTime.Now.AddDays(-_random.Next(10))
                }
            };
        }
        
        /// <summary>
        /// Creates a test phone number DTO with random data and allows customization.
        /// </summary>
        public PhoneNumberDto CreatePhoneNumberDto(Action<PhoneNumberDto> customize)
        {
            var dto = BuildPhoneNumberDto();
            customize(dto);
            return dto;
        }

        /// <summary>
        /// Creates a test JSON API single response with a person DTO.
        /// </summary>
        public JsonApiSingleResponse<PersonDto> BuildPersonResponse(string? id = null)
        {
            return new JsonApiSingleResponse<PersonDto>
            {
                Data = BuildPersonDto(id)
            };
        }

        /// <summary>
        /// Creates a test JSON API single response with a phone number DTO.
        /// </summary>
        public JsonApiSingleResponse<PhoneNumberDto> BuildPhoneNumberResponse(string? id = null)
        {
            return new JsonApiSingleResponse<PhoneNumberDto>
            {
                Data = BuildPhoneNumberDto(id)
            };
        }

        /// <summary>
        /// Creates a test JSON API collection response with multiple person DTOs.
        /// </summary>
        public JsonApiCollectionResponse<PersonDto> BuildPersonCollectionResponse(int count = 3)
        {
            var people = new List<PersonDto>();
            for (int i = 0; i < count; i++)
            {
                people.Add(BuildPersonDto());
            }

            return new JsonApiCollectionResponse<PersonDto>
            {
                Data = people,
                Meta = new PagedResponseMeta { Count = count, TotalCount = count },
                Links = new PagedResponseLinks { Self = "/people/v2/people" }
            };
        }

        /// <summary>
        /// Creates a test JSON API collection response with multiple phone number DTOs.
        /// </summary>
        public JsonApiCollectionResponse<PhoneNumberDto> BuildPhoneNumberCollectionResponse(int count = 3)
        {
            var phoneNumbers = new List<PhoneNumberDto>();
            for (int i = 0; i < count; i++)
            {
                phoneNumbers.Add(BuildPhoneNumberDto());
            }

            return new JsonApiCollectionResponse<PhoneNumberDto>
            {
                Data = phoneNumbers,
                Meta = new PagedResponseMeta { Count = count, TotalCount = count },
                Links = new PagedResponseLinks { Self = "/people/v2/people/1/phone_numbers" }
            };
        }
    }
}