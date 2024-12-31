using FluentAssertions;

namespace Mockzy.Tests;

public class MockzyTests
{

    [Fact]
    public void Mockzy_Should_Create_Mocks_For_Interface_Dependencies()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithDependencies>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        instance.ServiceA.Should().NotBeNull();
        instance.ServiceB.Should().NotBeNull();
        instance.ConcreteService.Should().NotBeNull();

        // Verify that the mocks are accessible via GetMock<TDependency>()
        var mockServiceA = mockzy.GetMock<IServiceA>();
        var mockServiceB = mockzy.GetMock<IServiceB>();
        var mockConcreteService = mockzy.GetMock<ConcreteService>();

        mockServiceA.Should().NotBeNull();
        mockServiceB.Should().NotBeNull();
        mockConcreteService.Should().NotBeNull();
    }

    [Fact]
    public void Mockzy_Should_Create_Mock_For_Abstract_Class_Dependencies()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithAbstractDependency>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        instance.AbstractService.Should().NotBeNull();

        // Verify that the mock is accessible via GetMock<TDependency>()
        var mockAbstractService = mockzy.GetMock<AbstractService>();
        mockAbstractService.Should().NotBeNull();
    }


    [Fact]
    public void Mockzy_Should_Handle_Concrete_Class_Dependencies_With_Virtual_Members()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithDependencies>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        instance.ConcreteService.Should().NotBeNull();

        // Verify that the mock is accessible via GetMock<TDependency>()
        var mockConcreteService = mockzy.GetMock<ConcreteService>();
        mockConcreteService.Should().NotBeNull();

        // Setup mock behavior
        mockConcreteService.Setup(cs => cs.GetData()).Returns("Mocked Data");

        // Act
        var data = instance.ConcreteService.GetData();

        // Assert
        data.Should().Be("Mocked Data");
    }

    [Fact]
    public void Mockzy_Should_Handle_Classes_With_No_Dependencies()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithoutDependencies>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        // Since there are no dependencies, no mocks should exist
        Action act = () => mockzy.GetMock<IServiceA>();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("No mock found for type IServiceA");
    }


    [Fact]
    public void Mockzy_Should_Select_Constructor_With_Most_Parameters()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithMultipleConstructors>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        instance.ServiceA.Should().NotBeNull();
        instance.ServiceB.Should().NotBeNull();
        instance.ConcreteService.Should().NotBeNull();

        // Verify that mocks are accessible
        var mockServiceA = mockzy.GetMock<IServiceA>();
        var mockServiceB = mockzy.GetMock<IServiceB>();
        var mockConcreteService = mockzy.GetMock<ConcreteService>();

        mockServiceA.Should().NotBeNull();
        mockServiceB.Should().NotBeNull();
        mockConcreteService.Should().NotBeNull();
    }


    [Fact]
    public void Mockzy_Should_Throw_Exception_When_No_Public_Constructors_Found()
    { 
        // Arrange
        // No specific arrangement needed as we're testing the instantiation

        // Act
        Action act = () => new Mockzy<ClassWithPrivateConstructor>();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("No public constructors found for type ClassWithPrivateConstructor");

    }



    [Fact]
    public void Mockzy_GetMock_Should_Throw_Exception_When_Mock_Not_Found()
    {
        // Arrange
        var mockzy = new Mockzy<ClassWithDependencies>();

        // Act
        Action act = () => mockzy.GetMock<NonMockableService>();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("No mock found for type NonMockableService");
    }

    [Fact]
    public void Mockzy_Should_Create_Mocks_For_Multiple_Interfaces()
    {
        // Arrange & Act
        var mockzy = new Mockzy<ClassWithDependencies>();
        var instance = mockzy.CreateInstanceWithMocks();

        // Assert
        instance.Should().NotBeNull();
        instance.ServiceA.Should().NotBeNull();
        instance.ServiceB.Should().NotBeNull();

        // Verify that the mocks are accessible via GetMock<TDependency>()
        var mockServiceA = mockzy.GetMock<IServiceA>();
        var mockServiceB = mockzy.GetMock<IServiceB>();

        mockServiceA.Should().NotBeNull();
        mockServiceB.Should().NotBeNull();
    }

}