using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureFilters.Tests;

public class CanaryFilterTests
{
    [Theory]
    [InlineData(0, 0, 0)] // 0%
    [InlineData(1, 0.008, 0.02)] // 1% ±5%
    [InlineData(10.2, 0.07, 0.13)] // 10.2% ±5%
    [InlineData(25.8, 0.24, 0.28)] // 25.8% ±5%
    [InlineData(50, 0.45, 0.55)] // 50% ±5%
    [InlineData(55.5, 0.50, 0.60)] // 55.5% ±5%
    [InlineData(65.5, 0.60, 0.70)] // 65.5% ±5%
    [InlineData(80, 0.75, 0.85)] // 80% ±5%
    [InlineData(100, 1, 1)] // 100%
    public async Task IsEnabledAsync_ShouldReturnTrue_WhenFeatureIsEnabledAndAtSpecifiedPercentage(double percentage, double lowerBound, double upperBound)
    {
        // Arrange
        const string feature = "feature";

        const int iterations = 10000;

        var inMemorySettings = new Dictionary<string, string?>
            {
                { $"FeatureManagement:{feature}:EnabledFor:0:Name", "Canary" },
                { $"FeatureManagement:{feature}:EnabledFor:0:Parameters:Percentage", percentage.ToString() }
            };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddFeatureManagement()
            .AddFeatureFilter<CanaryFilter>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        int enabledCount = 0;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            if (await featureManager.IsEnabledAsync(feature))
            {
                enabledCount++;
            }
        }

        // Assert
        var actualPercentage = (double)enabledCount / iterations;
        actualPercentage.Should().BeInRange(lowerBound, upperBound);
    }

    [Fact]
    public async Task IsEnabledAsync_ShouldReturnFalse_WhenFeatureIsDisabled()
    {
        const string feature = "feature";

        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { $"FeatureManagement:{feature}", "false" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
            .AddFeatureFilter<CanaryFilter>();

        var serviceProvider = services.BuildServiceProvider();

        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var result = await featureManager.IsEnabledAsync(feature);

        // Assert
        result.Should().BeFalse();
    }
}