using Moq;

namespace Mockzy;

public class Mockzy<T> where T : class
{
    private readonly Mock<T> _mock;
    private readonly Dictionary<Type, object> _mocks;

    public Mockzy()
    {
        _mocks = new Dictionary<Type, object>();
        _mock = new Mock<T>();

        // Initialize mocks for dependencies
        InitializeMocks();
    }

    /// <summary>
    /// The mocked instance of the target class T.
    /// </summary>
    public T Object => _mock.Object;

    /// <summary>
    /// Access the mock of a specific dependency type.
    /// </summary>
    /// <typeparam name="TDependency">Type of the dependency.</typeparam>
    /// <returns>Mock of the dependency.</returns>
    public Mock<TDependency> GetMock<TDependency>() where TDependency : class
    {
        if (_mocks.TryGetValue(typeof(TDependency), out var mockObj) && mockObj is Mock<TDependency> mock)
        {
            return mock;
        }

        throw new InvalidOperationException($"No mock found for type {typeof(TDependency).Name}");
    }

    /// <summary>
    /// Initializes mocks for all constructor dependencies.
    /// </summary>
    private void InitializeMocks()
    {
        var constructor = typeof(T).GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructors found for type {typeof(T).Name}");
        }

        var parameters = constructor.GetParameters();

        foreach (var parameter in parameters)
        {
            var paramType = parameter.ParameterType;

            // Only mock interfaces or abstract classes
            if (paramType.IsInterface || paramType.IsAbstract)
            {
                var mockType = typeof(Mock<>).MakeGenericType(paramType);
                var mockInstance = Activator.CreateInstance(mockType);

                _mocks[paramType] = mockInstance!;
            }
            else
            {
                // For concrete classes, attempt to create a mock if possible
                // Moq can only mock virtual members
                var mockType = typeof(Mock<>).MakeGenericType(paramType);
                var mockInstance = Activator.CreateInstance(mockType);

                _mocks[paramType] = mockInstance!;
            }
        }

        // Now, set up the mock for T to return the mocked dependencies when its constructor is called
        // However, since Moq doesn't support constructor injection directly,
        // we need to use a different approach.

        // One approach is to instantiate T with the mocked dependencies and set up the mock to call base methods.
        // Another is to restrict Mockzy to classes that are virtual or interfaces.

        // For simplicity, let's assume T has virtual methods and use Moq to create a mock that inherits from T
        // and overrides its virtual methods.

        // Alternatively, Mockzy can provide the instantiated object with dependencies mocked.
        // This would involve resolving T with the mocked dependencies.

        // Let's implement the latter approach.
    }

    /// <summary>
    /// Creates an instance of T with all dependencies mocked.
    /// </summary>
    /// <returns>Instance of T with dependencies mocked.</returns>
    public T CreateInstanceWithMocks()
    {
        var constructor = typeof(T).GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructors found for type {typeof(T).Name}");
        }

        var parameters = constructor.GetParameters();
        var args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;

            if (_mocks.TryGetValue(paramType, out var mockObj))
            {
                // Return the mock's Object property
                var mockProperty = mockObj.GetType().GetProperty("Object");
                args[i] = mockProperty?.GetValue(mockObj)!;
            }
            else
            {
                // If not mocked, attempt to create a default instance or throw
                args[i] = Activator.CreateInstance(paramType) ?? throw new InvalidOperationException($"Cannot create instance for parameter {parameters[i].Name} of type {paramType.Name}");
            }
        }

        var instance = Activator.CreateInstance(typeof(T), args);

        return instance as T ?? throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}");
    }
}
