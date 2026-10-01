using System.Reflection;
using Construction.Api.Controllers.V1;
using Construction.Application.Reports.GetProjectSummary;
using Construction.Domain.Projects;
using Construction.Infrastructure.Persistence;
using Xunit;

namespace Construction.ArchitectureTests;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_DoesNotReferenceOuterLayers()
    {
        IReadOnlySet<string> references =
            GetReferences(typeof(Project).Assembly);

        Assert.DoesNotContain(
            "Construction.Application",
            references);
        Assert.DoesNotContain(
            "Construction.Infrastructure",
            references);
        Assert.DoesNotContain(
            "Construction.Api",
            references);
    }

    [Fact]
    public void Application_DoesNotReferenceInfrastructureOrApi()
    {
        IReadOnlySet<string> references =
            GetReferences(
                typeof(GetProjectSummaryQueryHandler).Assembly);

        Assert.DoesNotContain(
            "Construction.Infrastructure",
            references);
        Assert.DoesNotContain(
            "Construction.Api",
            references);
    }

    [Fact]
    public void Infrastructure_DoesNotReferenceApi()
    {
        IReadOnlySet<string> references =
            GetReferences(
                typeof(ApplicationDbContext).Assembly);

        Assert.DoesNotContain(
            "Construction.Api",
            references);
    }

    [Fact]
    public void ApiControllers_AreSealed()
    {
        Type[] controllers =
            typeof(ReportsController).Assembly
                .GetTypes()
                .Where(type =>
                    type.Namespace?.Contains(
                        ".Controllers.",
                        StringComparison.Ordinal) == true
                    && type.Name.EndsWith(
                        "Controller",
                        StringComparison.Ordinal))
                .ToArray();

        Assert.NotEmpty(controllers);

        foreach (Type controller in controllers)
        {
            Assert.True(
                controller.IsSealed,
                $"{controller.FullName} must be sealed.");
        }
    }

    private static HashSet<string> GetReferences(
        Assembly assembly) =>
        assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
}
