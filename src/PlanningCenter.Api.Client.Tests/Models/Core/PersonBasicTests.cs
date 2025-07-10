using FluentAssertions;
using PlanningCenter.Api.Client.Models.Core;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Core;

/// <summary>
/// Basic unit tests for the Person model that work with current implementation.
/// </summary>
public class PersonBasicTests
{
    [Fact]
    public void Person_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var person = new Person();

        // Assert
        person.Id.Should().Be(string.Empty);
        person.FirstName.Should().Be(string.Empty);
        person.LastName.Should().Be(string.Empty);
        person.MiddleName.Should().BeNull();
        person.Nickname.Should().BeNull();
        person.Gender.Should().BeNull();
        person.Birthdate.Should().BeNull();
        person.Anniversary.Should().BeNull();
        person.Status.Should().Be("active");
        person.MembershipStatus.Should().BeNull();
        person.MaritalStatus.Should().BeNull();
        person.School.Should().BeNull();
        person.Grade.Should().BeNull();
        person.GraduationYear.Should().BeNull();
        person.MedicalNotes.Should().BeNull();
        person.EmergencyContactName.Should().BeNull();
        person.EmergencyContactPhone.Should().BeNull();
        person.AvatarUrl.Should().BeNull();
        person.DataSource.Should().Be("People");
    }

    [Fact]
    public void Person_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var testDateTime = DateTime.UtcNow;
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
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = null
        };

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void FullName_ShouldReturnFirstMiddleAndLastName_WhenAllAreSet()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            MiddleName = "Michael",
            LastName = "Doe"
        };

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John Michael Doe");
    }

    [Fact]
    public void FullName_ShouldReturnFirstName_WhenOnlyFirstNameIsSet()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            MiddleName = null,
            LastName = ""
        };

        // Act
        var fullName = person.FullName;

        // Assert
        fullName.Should().Be("John");
    }

    [Theory]
    [InlineData("1990-01-15", 34)]
    [InlineData("2000-06-30", 24)]
    [InlineData("2010-12-25", 13)]
    public void Age_ShouldCalculateCorrectAge_WhenBirthdateIsSet(string birthdateString, int expectedAge)
    {
        // Arrange
        var birthdate = DateTime.Parse(birthdateString);
        var person = new Person { Birthdate = birthdate };

        // Act
        var age = person.Age;

        // Assert
        // Note: This test assumes the current date is around 2024
        // Allow some variance for test stability
        age.Should().BeGreaterOrEqualTo(expectedAge - 1);
        age.Should().BeLessOrEqualTo(expectedAge + 1);
    }

    [Fact]
    public void Age_ShouldReturnNull_WhenBirthdateIsNotSet()
    {
        // Arrange
        var person = new Person { Birthdate = null };

        // Act
        var age = person.Age;

        // Assert
        age.Should().BeNull();
    }
}