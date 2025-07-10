using AutoFixture;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// Builder for creating test data objects with realistic values.
/// </summary>
public class TestDataBuilder
{
    private readonly IFixture _fixture;

    public TestDataBuilder()
    {
        _fixture = TestHelpers.CreateFixture();
    }

    /// <summary>
    /// Creates a test Person with realistic data.
    /// </summary>
    public Person CreatePerson(Action<Person>? customize = null)
    {
        var person = new Person
        {
            Id = _fixture.Create<string>(),
            FirstName = _fixture.Create<string>(),
            LastName = _fixture.Create<string>(),
            MiddleName = _fixture.Create<string>(),
            Nickname = _fixture.Create<string>(),
            Gender = "Male",
            Birthdate = DateTime.UtcNow.AddYears(-30),
            Anniversary = DateTime.UtcNow.AddYears(-5),
            Status = "active",
            MembershipStatus = "member",
            MaritalStatus = "married",
            School = _fixture.Create<string>(),
            Grade = "12",
            GraduationYear = DateTime.UtcNow.Year + 1,
            MedicalNotes = _fixture.Create<string>(),
            EmergencyContactName = _fixture.Create<string>(),
            EmergencyContactPhone = "+1-555-123-4567",
            AvatarUrl = "https://example.com/avatar.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            DataSource = "People"
        };

        customize?.Invoke(person);
        return person;
    }

    /// <summary>
    /// Creates a test PersonDto with realistic data.
    /// </summary>
    public PersonDto CreatePersonDto(Action<PersonDto>? customize = null)
    {
        var dto = new PersonDto
        {
            Type = "Person",
            Id = _fixture.Create<string>(),
            Attributes = new PersonAttributesDto
            {
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                MiddleName = _fixture.Create<string>(),
                Nickname = _fixture.Create<string>(),
                Gender = "Female",
                Birthdate = DateTime.UtcNow.AddYears(-25),
                Anniversary = DateTime.UtcNow.AddYears(-3),
                Status = "active",
                MembershipStatus = "member",
                MaritalStatus = "single",
                School = _fixture.Create<string>(),
                Grade = "10",
                GraduationYear = DateTime.UtcNow.Year + 2,
                MedicalNotes = _fixture.Create<string>(),
                EmergencyContactName = _fixture.Create<string>(),
                EmergencyContactPhone = "+1-555-987-6543",
                AvatarUrl = "https://example.com/avatar2.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        customize?.Invoke(dto);
        return dto;
    }

    /// <summary>
    /// Creates a test Address with realistic data.
    /// </summary>
    public Address CreateAddress(Action<Address>? customize = null)
    {
        var address = new Address
        {
            Id = _fixture.Create<string>(),
            Street = "123 Main Street",
            City = "Anytown",
            State = "CA",
            Zip = "12345",
            Location = "Home",
            Primary = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            DataSource = "People"
        };

        customize?.Invoke(address);
        return address;
    }

    /// <summary>
    /// Creates a test Email with realistic data.
    /// </summary>
    public Email CreateEmail(Action<Email>? customize = null)
    {
        var email = new Email
        {
            Id = _fixture.Create<string>(),
            Address = "test@example.com",
            Location = "Home",
            Primary = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            DataSource = "People"
        };

        customize?.Invoke(email);
        return email;
    }

    /// <summary>
    /// Creates a test PhoneNumber with realistic data.
    /// </summary>
    public PhoneNumber CreatePhoneNumber(Action<PhoneNumber>? customize = null)
    {
        var phone = new PhoneNumber
        {
            Id = _fixture.Create<string>(),
            Number = "+1-555-123-4567",
            Location = "Mobile",
            Primary = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            DataSource = "People"
        };

        customize?.Invoke(phone);
        return phone;
    }

    /// <summary>
    /// Creates a test PagedResponse with the specified data.
    /// </summary>
    public PagedResponse<T> CreatePagedResponse<T>(
        IList<T> data,
        int currentPage = 1,
        int totalPages = 1,
        int totalCount = 0,
        int perPage = 25)
    {
        totalCount = totalCount == 0 ? data.Count : totalCount;

        return new PagedResponse<T>
        {
            Data = data.ToList(),
            Meta = new PagedResponseMeta
            {
                TotalCount = totalCount,
                Count = data.Count,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                PerPage = perPage,
                CanOrderBy = new[] { "created_at", "updated_at", "first_name", "last_name" },
                CanQueryBy = new[] { "status", "first_name", "last_name", "email" },
                CanInclude = new[] { "addresses", "emails", "phone_numbers" },
                CanFilter = new[] { "status", "membership_status" },
                Parent = new Dictionary<string, object>
                {
                    ["type"] = "Organization",
                    ["id"] = "1"
                }
            },
            Links = new PagedResponseLinks
            {
                Self = $"https://api.planningcenteronline.com/people/v2/people?page={currentPage}&per_page={perPage}",
                Next = currentPage < totalPages ? 
                    $"https://api.planningcenteronline.com/people/v2/people?page={currentPage + 1}&per_page={perPage}" : 
                    null,
                Prev = currentPage > 1 ? 
                    $"https://api.planningcenteronline.com/people/v2/people?page={currentPage - 1}&per_page={perPage}" : 
                    null
            }
        };
    }

    /// <summary>
    /// Creates multiple test persons.
    /// </summary>
    public List<Person> CreatePersons(int count, Action<Person, int>? customize = null)
    {
        var persons = new List<Person>();
        for (int i = 0; i < count; i++)
        {
            var person = CreatePerson();
            customize?.Invoke(person, i);
            persons.Add(person);
        }
        return persons;
    }

    /// <summary>
    /// Creates multiple test PersonDtos.
    /// </summary>
    public List<PersonDto> CreatePersonDtos(int count, Action<PersonDto, int>? customize = null)
    {
        var dtos = new List<PersonDto>();
        for (int i = 0; i < count; i++)
        {
            var dto = CreatePersonDto();
            customize?.Invoke(dto, i);
            dtos.Add(dto);
        }
        return dtos;
    }

    /// <summary>
    /// Creates a test QueryParameters object.
    /// </summary>
    public QueryParameters CreateQueryParameters(Action<QueryParameters>? customize = null)
    {
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object>
            {
                ["status"] = "active"
            },
            Include = new[] { "addresses", "emails" },
            Order = "created_at",
            PerPage = 25,
            Offset = 0
        };

        customize?.Invoke(parameters);
        return parameters;
    }

    /// <summary>
    /// Creates a test PaginationOptions object.
    /// </summary>
    public PaginationOptions CreatePaginationOptions(Action<PaginationOptions>? customize = null)
    {
        var options = new PaginationOptions
        {
            MaxItems = 100,
            PageSize = 25,
            MaxConcurrentRequests = 3
        };

        customize?.Invoke(options);
        return options;
    }
}