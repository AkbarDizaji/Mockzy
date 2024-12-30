# Mockzy

**Mockzy** is a lightweight and intuitive .NET library designed to simplify the process of mocking dependencies in your unit tests. By leveraging the powerful [Moq](https://github.com/moq/moq4) framework, Mockzy automates the creation and management of mocks for your service classes, enabling you to focus on writing effective and maintainable tests.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Getting Started](#getting-started)
  - [Basic Usage](#basic-usage)
  - [Accessing and Configuring Mocks](#accessing-and-configuring-mocks)
  - [Creating Instances with Mocks](#creating-instances-with-mocks)
- [API Reference](#api-reference)
  - [Mockzy\<T> Class](#mockzyt-class)
    - [Constructor](#constructor)
    - [Properties](#properties)
    - [Methods](#methods)
- [Examples](#examples)
  - [Unit Test Example](#unit-test-example)
- [Best Practices](#best-practices)
- [Limitations](#limitations)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Automatic Mocking:** Automatically creates mocks for all constructor dependencies of the target class.
- **Easy Access:** Provides straightforward methods to access and configure specific dependency mocks.
- **Instance Creation:** Simplifies the creation of service instances with all dependencies mocked.
- **Integration with Moq:** Leverages the powerful features of the Moq framework for flexible and robust mocking capabilities.

## Installation

Mockzy can be easily integrated into your .NET projects via [NuGet](https://www.nuget.org/).

### Using the .NET CLI

`dotnet add package Mockzy`

### Using the Package Manager Console

`Install-Package Mockzy` 

## Getting Started

### Basic Usage

Suppose you have a service class `MyService` that depends on several interfaces:

    
    public class MyService(
        HttpClient httpClient,
        ICandidateClient candidateClient,
        ILookupClient lookupClient,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<ResumeParserService> logger)
    // Methods to be tested...}


### Accessing and Configuring Mocks

To create a mock instance of `MyService` with all its dependencies mocked, follow these steps:

1.  **Instantiate Mockzy:**

```
    using Mockzy;
    using Moq;
    var mockzy = new Mockzy<MyService>();
```

3.  **Configure Specific Dependency Mocks:**
  

```      
    // Configure the IConfiguration mock
    var mockConfiguration = mockzy.GetMock<IConfiguration>();
    mockConfiguration.Setup(cfg => cfg["parser:address"]).Returns("http://localhost:5000");
    
    // Configure the HttpClient mock
    var mockHttpClient = mockzy.GetMock<HttpClient>();
    mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                  .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                  {
                      Content = new StringContent("{\"error\":null}")
                  });
    
    // Configure other dependencies similarly...
```
### Creating Instances with Mocks

After configuring the necessary mocks, you can create an instance of `MyService` with all dependencies mocked:

`var MyService = mockzy.CreateInstanceWithMocks();` 

You can now use `MyService` in your tests, with all its dependencies behaving as configured in the mocks.

## API Reference

### Mockzy<T> Class

The `Mockzy<T>` class is the core of the Mockzy library, providing functionalities to automatically mock dependencies and create instances of the target class with these mocks injected.

#### Constructor

`public Mockzy()` 

-   **Description:** Initializes a new instance of the `Mockzy<T>` class. It automatically identifies and creates mocks for all constructor dependencies of the target class `T`.

#### Properties

-   **Object**
    
    
    `public T Object { get; }` 
    
    -   **Description:** Gets the mocked instance of the target class `T`.

#### Methods

-   **GetMock<TDependency>()**
    
    
    `public Mock<TDependency> GetMock<TDependency>() where TDependency : class` 
    
    -   **Description:** Retrieves the mock object for a specific dependency type `TDependency`.
    -   **Returns:** A `Mock<TDependency>` instance.
    -   **Exceptions:** Throws `InvalidOperationException` if no mock is found for the specified type.
-   **CreateInstanceWithMocks()**
    
    `public T CreateInstanceWithMocks()` 
    
    -   **Description:** Creates an instance of the target class `T` with all its constructor dependencies injected as mocked objects.
    -   **Returns:** An instance of `T` with mocked dependencies.
    -   **Exceptions:** Throws `InvalidOperationException` if instance creation fails due to missing mocks or instantiation issues.

## Examples

### Unit Test Example

Below is an example of how to use Mockzy in a unit test for the `MyService`:

```
using Mockzy;
using Moq;
using Xunit;

public class MyServiceTests
{
    [Fact]
    public void ParseResumeAsync_ShouldProcessResumeCorrectly()
    {
        // Arrange
        var mockzy = new Mockzy<MyService>();

        // Configure IConfiguration mock
        var mockConfiguration = mockzy.GetMock<IConfiguration>();
        mockConfiguration.Setup(cfg => cfg["parser:address"]).Returns("http://localhost:5000");

        // Configure HttpClient mock
        var mockHttpClient = mockzy.GetMock<HttpClient>();
        mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                      .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                      {
                          Content = new StringContent("{\"error\":null}")
                      });

        // Configure other dependencies as needed...

        var service = mockzy.CreateInstanceWithMocks();

        // Act
        // var result = await service.ParseResumeAsync(...);

        // Assert
        // Assert.NotNull(result);
        // mockHttpClient.Verify(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }
}
```

In this example:

1.  **Instantiate Mockzy:** Creates a new `Mockzy<MyService>` instance, automatically mocking all constructor dependencies.
2.  **Configure Mocks:** Sets up specific behaviors for the `IConfiguration` and `HttpClient` mocks.
3.  **Create Service Instance:** Generates an instance of `MyService` with all dependencies mocked.
4.  **Perform Assertions:** Uses assertions to verify the behavior of the service and interactions with the mocks.

## Best Practices

-   **Prefer Mocking Interfaces and Abstract Classes:** Mockzy automatically mocks interfaces and abstract classes. Ensure your dependencies are defined as interfaces or abstract classes to leverage this feature effectively.
    
-   **Configure Only Necessary Mocks:** Only configure the behavior of mocks that are relevant to the specific test scenario to keep tests clean and focused.
    
-   **Use `CreateInstanceWithMocks` Wisely:** Use this method to instantiate your service with all dependencies mocked. This ensures that your tests are isolated and do not rely on real implementations.
    
-   **Verify Mock Interactions:** Utilize Moq's verification features to ensure that your service interacts with its dependencies as expected.
    

## Limitations

-   **Concrete Class Mocking:** Mockzy attempts to mock all constructor dependencies, including concrete classes. However, Moq can only effectively mock interfaces and abstract classes. For concrete classes with virtual methods, mocking is possible, but it's recommended to depend on abstractions (interfaces or abstract classes) to facilitate easier testing.
    
-   **Complex Dependency Graphs:** While Mockzy handles simple dependency injection scenarios well, highly complex or deeply nested dependencies may require additional configuration or manual intervention.
    
-   **Constructor Selection:** Mockzy selects the constructor with the most parameters by default. If your class has multiple constructors, ensure that the chosen constructor is appropriate for your testing scenario.
    

## Contributing

Contributions are welcome! Whether it's reporting bugs, suggesting features, or submitting pull requests, your input helps improve Mockzy. Please follow these steps to contribute:

1.  **Fork the Repository:** Click the "Fork" button at the top-right corner of the repository page.
    
2.  **Clone Your Fork:**
    
    `git clone https://github.com/your-username/Mockzy.git` 
    
3.  **Create a New Branch:**
    
    `git checkout -b feature/YourFeatureName` 
    
4.  **Make Your Changes:** Implement your feature or bug fix.
    
5.  **Commit Your Changes:**
    
    
    `git commit -m "Add feature XYZ"` 
    
6.  **Push to Your Fork:**
     
    `git push origin feature/YourFeatureName` 
    
7.  **Open a Pull Request:** Navigate to the original repository and open a pull request with a description of your changes.
