namespace Mockzy.Tests;

public interface IServiceA
{
    void DoWork();
}

public interface IServiceB
{
    int Calculate(int a, int b);
}

// Concrete class dependency (with virtual members to allow mocking)
public class ConcreteService
{
    public virtual string GetData()
    {
        return "Real Data";
    }
}

// Concrete class dependency without virtual members (cannot be mocked by Moq)
public class NonMockableService
{
    public string FetchInfo()
    {
        return "Info";
    }
}

// Class with interface and concrete class dependencies
public class ClassWithDependencies
{
    public IServiceA ServiceA { get; }
    public IServiceB ServiceB { get; }
    public ConcreteService ConcreteService { get; }

    public ClassWithDependencies(IServiceA serviceA, IServiceB serviceB, ConcreteService concreteService)
    {
        ServiceA = serviceA;
        ServiceB = serviceB;
        ConcreteService = concreteService;
    }

    public string Process()
    {
        ServiceA.DoWork();
        var result = ServiceB.Calculate(5, 10);
        var data = ConcreteService.GetData();
        return $"Result: {result}, Data: {data}";
    }
}

// Class with no dependencies
public class ClassWithoutDependencies
{
    public string SayHello()
    {
        return "Hello, World!";
    }
}


public class ClassWithPrivateConstructor
{
    private ClassWithPrivateConstructor()
    {
    }
}

// Class with multiple constructors
public class ClassWithMultipleConstructors
{
    public IServiceA ServiceA { get; }
    public IServiceB ServiceB { get; }
    public ConcreteService ConcreteService { get; }

    // Parameterless constructor
    public ClassWithMultipleConstructors()
    {
    }

    // Constructor with one dependency
    public ClassWithMultipleConstructors(IServiceA serviceA)
    {
        ServiceA = serviceA;
    }

    // Constructor with multiple dependencies
    public ClassWithMultipleConstructors(IServiceA serviceA, IServiceB serviceB, ConcreteService concreteService)
    {
        ServiceA = serviceA;
        ServiceB = serviceB;
        ConcreteService = concreteService;
    }
}

// Class with abstract class dependency
public abstract class AbstractService
{
    public abstract void Execute();
}

public class ClassWithAbstractDependency
{
    public AbstractService AbstractService { get; }

    public ClassWithAbstractDependency(AbstractService abstractService)
    {
        AbstractService = abstractService;
    }

    public void Run()
    {
        AbstractService.Execute();
    }
}
