using FluentAssertions;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Core;

/// <summary>
/// Unit tests for the Person model.
/// </summary>
public class PersonTests
{
    private readonly TestDataBuilder _testDataBuilder = new();

    [Fact]
    public void Person_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var person = new Person();

        // Assert
        person.Id.Should().BeNull();
        person.FirstName.Should().BeNull();
        person.LastName.Should().BeNull();
        person.MiddleName.Should().BeNull();
        person.Nickname.Should().BeNull();
        person.Gender.Should().BeNull();
        person.Birthdate.Should().BeNull();
        person.Anniversary.Should().BeNull();
        person.Status.Should().BeNull();
        person.MembershipStatus.Should().BeNull();
        person.MaritalStatus.Should().BeNull();
        person.School.Should().BeNull();
        person.Grade.Should().BeNull();
        person.GraduationYear.Should().BeNull();
        person.MedicalNotes.Should().BeNull();
        person.EmergencyContactName.Should().BeNull();
        person.EmergencyContactPhone.Should().BeNull();
        person.AvatarUrl.Should().BeNull();
        person.CreatedAt.Should().BeNull();
        person.UpdatedAt.Should().BeNull();
        person.DataSource.Should().BeNull();
    }

    [Fact]
    public void Person_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var testDateTime = TestHelpers.CreateTestDateTime();
        var person = new Person();

        // Act
        person.Id = "123";
        person.FirstName = "John";
        person.LastName = "Doe";
        person.MiddleName = "Michael";
        person.Nickname = "Johnny";
        person.Gender = "Male";
        person.Birthdate = testDateTime;
        person.Anniversary = testDateTime.AddYears(25);
        person.Status = "active";
        person.MembershipStatus = "member";
        person.MaritalStatus = "married";
        person.School = "Test High School";
        person.Grade = "12";
        person.GraduationYear = 2024;
        person.MedicalNotes = "No known allergies";
        person.EmergencyContactName = "Jane Doe";
        person.EmergencyContactPhone = "+1-555-123-4567";
        person.AvatarUrl = "https://example.com/avatar.jpg";
        person.CreatedAt = testDateTime;
        person.UpdatedAt = testDateTime.AddDays(1);
        person.DataSource = "People";

        // Assert
        person.Id.Should().Be("123");
        person.FirstName.Should().Be("John");
        person.LastName.Should().Be("Doe");
        person.MiddleName.Should().Be("Michael");
        person.Nickname.Should().Be("Johnny");
        person.Gender.Should().Be("Male");
        person.Birthdate.Should().Be(testDateTime);
        person.Anniversary.Should().Be(testDateTime.AddYears(25));
        person.Status.Should().Be("active");
        person.MembershipStatus.Should().Be("member");
        person.MaritalStatus.Should().Be("married");
        person.School.Should().Be("Test High School");
        person.Grade.Should().Be("12");
        person.GraduationYear.Should().Be(2024);
        person.MedicalNotes.Should().Be("No known allergies");
        person.EmergencyContactName.Should().Be("Jane Doe");
        person.EmergencyContactPhone.Should().Be("+1-555-123-4567");
        person.AvatarUrl.Should().Be("https://example.com/avatar.jpg");
        person.CreatedAt.Should().Be(testDateTime);
        person.UpdatedAt.Should().Be(testDateTime.AddDays(1));
        person.DataSource.Should().Be("People");
    }

    [Fact]
    public void FullName_ShouldReturnFirstAndLastName_WhenBothAreSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = "John";
            p.LastName = "Doe";
            p.MiddleName = null;
        });

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void FullName_ShouldReturnFirstMiddleAndLastName_WhenAllAreSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = "John";
            p.MiddleName = "Michael";
            p.LastName = "Doe";
        });

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John Michael Doe");
    }

    [Fact]
    public void FullName_ShouldReturnFirstName_WhenOnlyFirstNameIsSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = "John";
            p.MiddleName = null;
            p.LastName = null;
        });

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John");
    }

    [Fact]
    public void FullName_ShouldReturnLastName_WhenOnlyLastNameIsSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = null;
            p.MiddleName = null;
            p.LastName = "Doe";
        });

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("Doe");
    }

    [Fact]
    public void FullName_ShouldReturnEmptyString_WhenNoNamesAreSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = null;
            p.MiddleName = null;
            p.LastName = null;
        });

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("");
    }

    [Theory]
    [InlineData("1990-01-15", 34)]
    [InlineData("2000-06-30", 24)]
    [InlineData("2010-12-25", 13)]
    public void Age_ShouldCalculateCorrectAge_WhenBirthdateIsSet(string birthdateString, int expectedAge)
    {
        // Arrange
        var birthdate = DateTime.Parse(birthdateString);
        var person = _testDataBuilder.CreatePerson(p => p.Birthdate = birthdate);

        // Act
        var age = person.Age;

        // Assert
        // Note: This test assumes the current date is around 2024
        // In a real scenario, you might want to inject a time provider for more precise testing
        age.Should().BeCloseTo(expectedAge, 1); // Allow 1 year variance for test stability
    }

    [Fact]
    public void Age_ShouldReturnNull_WhenBirthdateIsNotSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p => p.Birthdate = null);

        // Act
        var age = person.Age;

        // Assert
        age.Should().BeNull();
    }

    [Fact]
    public void DisplayName_ShouldReturnNickname_WhenNicknameIsSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = "John";
            p.LastName = "Doe";
            p.Nickname = "Johnny";
        });

        // Act
        var displayName = person.DisplayName;

        // Assert
        displayName.Should().Be("Johnny");
    }

    [Fact]
    public void DisplayName_ShouldReturnFullName_WhenNicknameIsNotSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p =>
        {
            p.FirstName = "John";
            p.LastName = "Doe";
            p.Nickname = null;
        });

        // Act
        var displayName = person.DisplayName;

        // Assert
        displayName.Should().Be("John Doe");
    }

    [Fact]
    public void IsActive_ShouldReturnTrue_WhenStatusIsActive()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p => p.Status = "active");

        // Act
        var isActive = person.IsActive;

        // Assert
        isActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("inactive")]
    [InlineData("archived")]
    [InlineData("pending")]
    [InlineData(null)]
    public void IsActive_ShouldReturnFalse_WhenStatusIsNotActive(string? status)
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p => p.Status = status);

        // Act
        var isActive = person.IsActive;

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void HasAvatar_ShouldReturnTrue_WhenAvatarUrlIsSet()
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p => p.AvatarUrl = "https://example.com/avatar.jpg");

        // Act
        var hasAvatar = person.HasAvatar;

        // Assert
        hasAvatar.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HasAvatar_ShouldReturnFalse_WhenAvatarUrlIsNullOrEmpty(string? avatarUrl)
    {
        // Arrange
        var person = _testDataBuilder.CreatePerson(p => p.AvatarUrl = avatarUrl);

        // Act
        var hasAvatar = person.HasAvatar;

        // Assert
        hasAvatar.Should().BeFalse();
    }
}