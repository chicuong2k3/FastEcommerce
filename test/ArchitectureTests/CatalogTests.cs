using NetArchTest.Rules;
using Shared.Application;
using Shared.Contracts;
using Shared.Core;
using System.Reflection;

namespace ArchitectureTests;

public class CatalogTests
{
    private const string Core = nameof(Core);
    private const string Application = nameof(Application);
    private const string Infrastructure = nameof(Infrastructure);
    private readonly Assembly CoreAssembly = typeof(Catalog.Core.AssemblyInfo).Assembly;
    private readonly Assembly ApplicationAssembly = typeof(Catalog.Application.AssemblyInfo).Assembly;
    private readonly Assembly ContractsAssembly = typeof(Catalog.Contracts.AssemblyInfo).Assembly;

    [Fact]
    public void Core_ShouldNot_HaveDependencyOnOthers()
    {
        var others = new[]
        {
            Application,
            Infrastructure
        };

        var result = Types.InAssembly(CoreAssembly)
                          .ShouldNot()
                          .HaveDependencyOnAll(others)
                          .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_ShouldNot_HaveDependencyOnOthers()
    {
        var others = new[]
        {
            Infrastructure
        };

        var result = Types.InAssembly(ApplicationAssembly)
                          .ShouldNot()
                          .HaveDependencyOnAll(others)
                          .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Handlers_Should_HaveDependencyOnDomain()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(Core)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Commands_Should_Have_CorrectSuffix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        string message = string.Empty;
        if (!result.IsSuccessful)
        {
            var violatingTypes = result.FailingTypes.Select(t => t.FullName).ToList();

            message = "The following commands do not follow the naming convention (should end with 'Command'):\n" +
                          string.Join("\n", violatingTypes);


        }

        Assert.True(result.IsSuccessful, message);
    }

    [Fact]
    public void CommandHandlers_Should_Have_CorrectSuffix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        string message = string.Empty;
        if (!result.IsSuccessful)
        {
            var violatingTypes = result.FailingTypes.Select(t => t.FullName).ToList();

            message = "The following command handlers do not follow the naming convention (should end with 'CommandHandler'):\n" +
                          string.Join("\n", violatingTypes);

        }

        Assert.True(result.IsSuccessful, message);

    }

    [Fact]
    public void Queries_Should_Have_CorrectSuffix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        string message = string.Empty;
        if (!result.IsSuccessful)
        {
            var violatingTypes = result.FailingTypes.Select(t => t.FullName).ToList();

            message = "The following queries do not follow the naming convention (should end with 'Query'):\n" +
                          string.Join("\n", violatingTypes);


        }

        Assert.True(result.IsSuccessful, message);
    }

    [Fact]
    public void QueryHandlers_Should_Have_CorrectSuffix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();
        string message = string.Empty;
        if (!result.IsSuccessful)
        {
            var violatingTypes = result.FailingTypes.Select(t => t.FullName).ToList();

            message = "The following query handlers do not follow the naming convention (should end with 'QueryHandler'):\n" +
                          string.Join("\n", violatingTypes);


        }

        Assert.True(result.IsSuccessful, message);
    }

    [Fact]
    public void DomainEvents_Should_Have_JsonConstructor_With_Id_And_OccurredOn()
    {
        var eventTypes = Types.InAssembly(CoreAssembly)
                              .That()
                              .Inherit(typeof(DomainEvent))
                              .GetTypes();

        var failingTypes = new List<string>();

        foreach (var type in eventTypes)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            bool hasValidJsonConstructor = constructors.Any(ctor =>
            {
                var jsonCtorAttr = ctor.GetCustomAttributes(typeof(System.Text.Json.Serialization.JsonConstructorAttribute), false).Any();
                if (!jsonCtorAttr)
                    return false;

                var parameters = ctor.GetParameters();
                if (parameters.Length < 2)
                    return false;

                var last = parameters[^2];
                var last2 = parameters[^1];

                return (last.ParameterType == typeof(Guid) && last2.ParameterType == typeof(DateTime))
                    || (last.ParameterType == typeof(DateTime) && last2.ParameterType == typeof(Guid));
            });

            if (!hasValidJsonConstructor)
            {
                failingTypes.Add(type.FullName!);
            }
        }

        var message = failingTypes.Any()
            ? "The following DomainEvent types do not have a [JsonConstructor] with Guid id and DateTime occurredOn:\n" +
              string.Join("\n", failingTypes)
            : string.Empty;

        Assert.True(failingTypes.Count == 0, message);
    }

    [Fact]
    public void IntegrationEvents_Should_Have_JsonConstructor_With_Id_And_OccurredOn()
    {
        var eventTypes = Types.InAssembly(ContractsAssembly)
            .That()
            .Inherit(typeof(IntegrationEvent))
            .GetTypes();

        var failingTypes = new List<string>();

        foreach (var type in eventTypes)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            bool hasValidJsonConstructor = constructors.Any(ctor =>
            {
                var hasAttr = ctor.GetCustomAttributes(typeof(System.Text.Json.Serialization.JsonConstructorAttribute), false).Any();
                if (!hasAttr) return false;

                var parameters = ctor.GetParameters();

                var hasId = parameters.Any(p => p.ParameterType == typeof(Guid));
                var hasOccurredOn = parameters.Any(p => p.ParameterType == typeof(DateTime));

                return hasId && hasOccurredOn;
            });

            if (!hasValidJsonConstructor)
            {
                failingTypes.Add(type.FullName!);
            }
        }

        var message = failingTypes.Any()
            ? "The following IntegrationEvent types do not have a [JsonConstructor] with Guid id and DateTime occurredOn:\n" +
              string.Join("\n", failingTypes)
            : string.Empty;

        Assert.True(failingTypes.Count == 0, message);
    }
}
