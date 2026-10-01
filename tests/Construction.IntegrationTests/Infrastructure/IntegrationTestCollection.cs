using Xunit;

namespace Construction.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationTestGroup
    : ICollectionFixture<IntegrationTestFixture>
{
    public const string Name = "PostgreSQL integration";
}
