﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace FeatureFilters;

[FilterAlias(Alias)]
public class CanaryFilter : IFeatureFilter
{
    private const string Alias = "Canary";
    
    private readonly ILogger _logger;

    public CanaryFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CanaryFilter>();
    }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var settings = context.Parameters.Get<CanaryFilterSettings>();

        bool result = true;

        if (settings.Percentage < 0)
        {
            _logger.LogWarning($"The '{Alias}' feature filter does not have a valid '{nameof(settings.Percentage)}' value for feature '{context.FeatureName}'");

            result = false;
        }

        if (result)
        {
            result = (RandomGenerator.NextDouble() * 100) < settings.Percentage;
        }

        return Task.FromResult(result);
    }
}