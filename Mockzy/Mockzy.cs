using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mockzy
{
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
            var constructor = typeof(T).GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

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

            // Note: For more complex scenarios, additional initialization logic might be required.
        }

        /// <summary>
        /// Creates an instance of T with all dependencies mocked.
        /// </summary>
        /// <returns>Instance of T with dependencies mocked.</returns>
        public T CreateInstanceWithMocks()
        {
            var constructor = typeof(T).GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

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
                    // Use reflection to get the 'Object' property with DeclaredOnly to avoid ambiguities
                    var objectProperty = mockObj.GetType().GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    if (objectProperty == null)
                    {
                        throw new InvalidOperationException($"No 'Object' property found on mock of type {paramType.Name}");
                    }

                    args[i] = objectProperty.GetValue(mockObj)!;
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
}
