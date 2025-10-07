namespace Csdsa.Contracts.Interfaces;

public interface IFeatureToggleService
{
    Task<bool> IsEnabledAsync(string key);
}
