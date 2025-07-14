using System;
using AutoFixture;
using PlanningCenter.Api.Client.Models.People;

namespace PlanningCenter.Api.Client.Tests.Utilities
{
    /// <summary>
    /// Lightweight test data factory for building DTO instances used in unit tests.
    /// Only a subset of helpers are implemented for initial scaffolding; extend as needed.
    /// </summary>
    public class TestDataBuilder
    {
        private readonly Fixture _fixture = new();

        public PersonDto CreatePersonDto(Action<PersonDto>? customize = null)
        {
            var dto = new PersonDto
            {
                Id = _fixture.Create<string>(),
                Attributes = new PersonAttributesDto
                {
                    FirstName = _fixture.Create<string>(),
                    LastName = _fixture.Create<string>(),
                    Status = "active",
                    Gender = "male",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow
                }
            };
            customize?.Invoke(dto);
            return dto;
        }
    }
}
