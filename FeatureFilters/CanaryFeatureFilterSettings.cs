namespace FeatureFilters;

public class CanaryFeatureFilterSettings
{
    /// <summary>
    /// A value between 0 and 100 specifying the chance that a feature configured to use the <see cref="CanaryFeatureFilter"/> should be enabled.
    /// </summary>
    public double Percentage { get; set; } = -1;
}