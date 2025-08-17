using InventoryService.Infrastructure;
using NetArchTest.Rules;
using Shared.Application;
using System.Reflection;

namespace ArchitectureTests;

public class InventoryServiceTests
{
    private const string Core = nameof(Core);
    private const string Application = nameof(Application);
    private const string Infrastructure = nameof(Infrastructure);
    private readonly Assembly CoreAssembly = typeof(AssemblyInfo).Assembly;
    private readonly Assembly ApplicationAssembly = typeof(InventoryService.Application.AssemblyInfo).Assembly;

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
}
