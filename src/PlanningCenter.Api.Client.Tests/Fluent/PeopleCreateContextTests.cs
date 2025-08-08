using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for PeopleCreateContext.
/// Tests the fluent creation API functionality.
/// </summary>
public class PeopleCreateContextTests
{
    private readonly Mock<IPeopleService> _mockPeopleService;
    private readonly PersonCreateRequest _personRequest;
    private readonly PeopleCreateContext _createContext;
    private readonly TestDataBuilder _builder = new();

    public PeopleCreateContextTests()
    {
        _mockPeopleService = new Mock<IPeopleService>();
        _personRequest = new PersonCreateRequest { FirstName = "John", LastName = "Doe" };
        _createContext = new PeopleCreateContext(_mockPeopleService.Object, _personRequest);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPeopleServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PeopleCreateContext(null!, _personRequest));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPersonRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PeopleCreateContext(_mockPeopleService.Object, null!));
    }

    #endregion

    #region Fluent Method Tests

    [Fact]
    public void WithAddress_ShouldReturnSameContext_WhenRequestIsProvided()
    {
        // Arrange
        var addressRequest = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345"
        };

        // Act
        var result = _createContext.WithAddress(addressRequest);

        // Assert
        result.Should().BeSameAs(_createContext);
    }

    [Fact]
    public void WithAddress_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _createContext.WithAddress(null!));
    }

    [Fact]
    public void WithEmail_ShouldReturnSameContext_WhenRequestIsProvided()
    {
        // Arrange
        var emailRequest = new EmailCreateRequest
        {
            Address = "john.doe@example.com",
            Location = "Home",
            Primary = true
        };

        // Act
        var result = _createContext.WithEmail(emailRequest);

        // Assert
        result.Should().BeSameAs(_createContext);
    }

    [Fact]
    public void WithEmail_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _createContext.WithEmail(null!));
    }

    [Fact]
    public void WithPhoneNumber_ShouldReturnSameContext_WhenRequestIsProvided()
    {
        // Arrange
        var phoneRequest = new PhoneNumberCreateRequest
        {
            Number = "555-123-4567",
            Location = "Mobile",
            Primary = true
        };

        // Act
        var result = _createContext.WithPhoneNumber(phoneRequest);

        // Assert
        result.Should().BeSameAs(_createContext);
    }

    [Fact]
    public void WithPhoneNumber_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _createContext.WithPhoneNumber(null!));
    }

    #endregion

    #region Execution Tests

    [Fact]
    public async Task ExecuteAsync_ShouldCreatePersonOnly_WhenNoRelatedDataIsAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);

        // Act
        var result = await _createContext.ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreatePersonAndAddress_WhenAddressIsAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        var addressRequest = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345"
        };
        var expectedAddress = new Address
        {
            Id = "addr123",
            Street = addressRequest.Street,
            City = addressRequest.City,
            State = addressRequest.State,
            Zip = addressRequest.Zip
        };

        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);
        _mockPeopleService.Setup(s => s.AddAddressAsync(expectedPerson.Id, addressRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAddress);

        // Act
        var result = await _createContext
            .WithAddress(addressRequest)
            .ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddAddressAsync(expectedPerson.Id, addressRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreatePersonAndEmail_WhenEmailIsAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        var emailRequest = new EmailCreateRequest
        {
            Address = "john.doe@example.com",
            Location = "Home",
            Primary = true
        };
        var expectedEmail = new Email
        {
            Id = "email123",
            Address = emailRequest.Address,
            Location = emailRequest.Location,
            Primary = emailRequest.Primary
        };

        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);
        _mockPeopleService.Setup(s => s.AddEmailAsync(expectedPerson.Id, emailRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEmail);

        // Act
        var result = await _createContext
            .WithEmail(emailRequest)
            .ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddEmailAsync(expectedPerson.Id, emailRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreatePersonAndPhoneNumber_WhenPhoneNumberIsAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        var phoneRequest = new PhoneNumberCreateRequest
        {
            Number = "555-123-4567",
            Location = "Mobile",
            Primary = true
        };
        var expectedPhone = _builder.BuildPhoneNumber("phone123");

        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);
        _mockPeopleService.Setup(s => s.AddPhoneNumberAsync(expectedPerson.Id, phoneRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPhone);

        // Act
        var result = await _createContext
            .WithPhoneNumber(phoneRequest)
            .ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddPhoneNumberAsync(expectedPerson.Id, phoneRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreatePersonAndAllRelatedData_WhenMultipleItemsAreAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        
        var addressRequest = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345"
        };
        
        var emailRequest = new EmailCreateRequest
        {
            Address = "john.doe@example.com",
            Location = "Home",
            Primary = true
        };
        
        var phoneRequest = new PhoneNumberCreateRequest
        {
            Number = "555-123-4567",
            Location = "Mobile",
            Primary = true
        };

        var expectedAddress = new Address { Id = "addr123", Street = addressRequest.Street };
        var expectedEmail = new Email { Id = "email123", Address = emailRequest.Address };
        var expectedPhone = _builder.BuildPhoneNumber("phone123");

        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);
        _mockPeopleService.Setup(s => s.AddAddressAsync(expectedPerson.Id, addressRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAddress);
        _mockPeopleService.Setup(s => s.AddEmailAsync(expectedPerson.Id, emailRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEmail);
        _mockPeopleService.Setup(s => s.AddPhoneNumberAsync(expectedPerson.Id, phoneRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPhone);

        // Act
        var result = await _createContext
            .WithAddress(addressRequest)
            .WithEmail(emailRequest)
            .WithPhoneNumber(phoneRequest)
            .ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddAddressAsync(expectedPerson.Id, addressRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddEmailAsync(expectedPerson.Id, emailRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddPhoneNumberAsync(expectedPerson.Id, phoneRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateMultipleAddresses_WhenMultipleAddressesAreAdded()
    {
        // Arrange
        var expectedPerson = _builder.BuildPerson("123");
        
        var homeAddress = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345",
            Location = "Home"
        };
        
        var workAddress = new AddressCreateRequest
        {
            Street = "456 Business Ave",
            City = "Worktown",
            State = "CA",
            Zip = "54321",
            Location = "Work"
        };

        var expectedHomeAddress = new Address { Id = "addr1", Street = homeAddress.Street };
        var expectedWorkAddress = new Address { Id = "addr2", Street = workAddress.Street };

        _mockPeopleService.Setup(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);
        _mockPeopleService.Setup(s => s.AddAddressAsync(expectedPerson.Id, homeAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedHomeAddress);
        _mockPeopleService.Setup(s => s.AddAddressAsync(expectedPerson.Id, workAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWorkAddress);

        // Act
        var result = await _createContext
            .WithAddress(homeAddress)
            .WithAddress(workAddress)
            .ExecuteAsync();

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.CreateAsync(_personRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddAddressAsync(expectedPerson.Id, homeAddress, It.IsAny<CancellationToken>()), Times.Once);
        _mockPeopleService.Verify(s => s.AddAddressAsync(expectedPerson.Id, workAddress, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Arrange
        var addressRequest = new AddressCreateRequest { Street = "123 Main St", City = "Test", State = "CA", Zip = "12345" };
        var emailRequest = new EmailCreateRequest { Address = "test@example.com" };
        var phoneRequest = new PhoneNumberCreateRequest { Number = "555-123-4567" };

        // Act & Assert (should not throw)
        var result = _createContext
            .WithAddress(addressRequest)
            .WithEmail(emailRequest)
            .WithPhoneNumber(phoneRequest)
            .WithAddress(addressRequest) // Can add multiple of the same type
            .WithEmail(emailRequest);

        result.Should().BeSameAs(_createContext);
    }

    #endregion
}