using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for PeopleService contact information operations.
/// Tests address, email, and phone number management against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class PeopleServiceContactInfoIntegrationTests : PeopleServiceIntegrationTestBase
{
    public PeopleServiceContactInfoIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task AddressManagement_ShouldPerformFullLifecycle()
    {
        // Arrange - Create a test person
        Person testPerson = null!;
        try
        {
            testPerson = await CreateTestPersonAsync();
            
            // Arrange - Create address request
            var createRequest = new AddressCreateRequest
            {
                Street = "123 Test Street",
                City = "Test City",
                State = "TS",
                Zip = "12345",
                Location = "Home",
                IsPrimary = true
            };

            // Act - Add address
            var createdAddress = await _peopleService.AddAddressAsync(testPerson.Id, createRequest);

            // Assert - Add address
            createdAddress.Should().NotBeNull();
            createdAddress.Id.Should().NotBeNullOrEmpty();
            createdAddress.Street.Should().Be(createRequest.Street);
            createdAddress.City.Should().Be(createRequest.City);
            createdAddress.State.Should().Be(createRequest.State);
            createdAddress.Zip.Should().Be(createRequest.Zip);
            createdAddress.Location.Should().Be(createRequest.Location);
            createdAddress.Primary.Should().BeTrue();

            // Act - Update address
            var updateRequest = new AddressUpdateRequest
            {
                Street = "456 Updated Street",
                City = "Updated City"
            };

            var updatedAddress = await _peopleService.UpdateAddressAsync(testPerson.Id, createdAddress.Id, updateRequest);

            // Assert - Update address
            updatedAddress.Should().NotBeNull();
            updatedAddress.Id.Should().Be(createdAddress.Id);
            updatedAddress.Street.Should().Be(updateRequest.Street);
            updatedAddress.City.Should().Be(updateRequest.City);
            updatedAddress.State.Should().Be(createRequest.State); // Unchanged
            updatedAddress.Zip.Should().Be(createRequest.Zip); // Unchanged

            // Act - Delete address
            await _peopleService.DeleteAddressAsync(testPerson.Id, createdAddress.Id);

            // Note: There's no direct way to verify address deletion in the API
            // A real test would need to retrieve the person and check their addresses
        }
        finally
        {
            // Cleanup
            if (testPerson != null)
            {
                await CleanupTestPersonAsync(testPerson.Id);
            }
        }
    }

    [Fact]
    public async Task EmailManagement_ShouldPerformFullLifecycle()
    {
        // Arrange - Create a test person
        Person testPerson = null!;
        try
        {
            testPerson = await CreateTestPersonAsync();
            
            // Arrange - Create email request
            var createRequest = new EmailCreateRequest
            {
                Address = $"test.email.{_random.Next(10000, 99999)}@example.com",
                Location = "Home",
                IsPrimary = true
            };

            // Act - Add email
            var createdEmail = await _peopleService.AddEmailAsync(testPerson.Id, createRequest);

            // Assert - Add email
            createdEmail.Should().NotBeNull();
            createdEmail.Id.Should().NotBeNullOrEmpty();
            createdEmail.Address.Should().Be(createRequest.Address);
            createdEmail.Location.Should().Be(createRequest.Location);
            createdEmail.Primary.Should().BeTrue();

            // Act - Update email
            var updateRequest = new EmailUpdateRequest
            {
                Address = $"updated.email.{_random.Next(10000, 99999)}@example.com",
                Location = "Work"
            };

            var updatedEmail = await _peopleService.UpdateEmailAsync(testPerson.Id, createdEmail.Id, updateRequest);

            // Assert - Update email
            updatedEmail.Should().NotBeNull();
            updatedEmail.Id.Should().Be(createdEmail.Id);
            updatedEmail.Address.Should().Be(updateRequest.Address);
            updatedEmail.Location.Should().Be(updateRequest.Location);

            // Act - Delete email
            await _peopleService.DeleteEmailAsync(testPerson.Id, createdEmail.Id);

            // Note: There's no direct way to verify email deletion in the API
            // A real test would need to retrieve the person and check their emails
        }
        finally
        {
            // Cleanup
            if (testPerson != null)
            {
                await CleanupTestPersonAsync(testPerson.Id);
            }
        }
    }

    [Fact]
    public async Task PhoneNumberManagement_ShouldPerformFullLifecycle()
    {
        // Arrange - Create a test person
        Person testPerson = null!;
        try
        {
            testPerson = await CreateTestPersonAsync();
            
            // Arrange - Create phone number request
            var createRequest = new PhoneNumberCreateRequest
            {
                Number = $"555-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                Location = "Mobile",
                IsPrimary = true
            };

            // Act - Add phone number
            var createdPhoneNumber = await _peopleService.AddPhoneNumberAsync(testPerson.Id, createRequest);

            // Assert - Add phone number
            createdPhoneNumber.Should().NotBeNull();
            createdPhoneNumber.Id.Should().NotBeNullOrEmpty();
            createdPhoneNumber.Number.Should().Be(createRequest.Number);
            createdPhoneNumber.Location.Should().Be(createRequest.Location);
            createdPhoneNumber.Primary.Should().BeTrue();

            // Act - Update phone number
            var updateRequest = new PhoneNumberUpdateRequest
            {
                Number = $"555-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                Location = "Home"
            };

            var updatedPhoneNumber = await _peopleService.UpdatePhoneNumberAsync(testPerson.Id, createdPhoneNumber.Id, updateRequest);

            // Assert - Update phone number
            updatedPhoneNumber.Should().NotBeNull();
            updatedPhoneNumber.Id.Should().Be(createdPhoneNumber.Id);
            updatedPhoneNumber.Number.Should().Be(updateRequest.Number);
            updatedPhoneNumber.Location.Should().Be(updateRequest.Location);

            // Act - Delete phone number
            await _peopleService.DeletePhoneNumberAsync(testPerson.Id, createdPhoneNumber.Id);

            // Note: There's no direct way to verify phone number deletion in the API
            // A real test would need to retrieve the person and check their phone numbers
        }
        finally
        {
            // Cleanup
            if (testPerson != null)
            {
                await CleanupTestPersonAsync(testPerson.Id);
            }
        }
    }
}
