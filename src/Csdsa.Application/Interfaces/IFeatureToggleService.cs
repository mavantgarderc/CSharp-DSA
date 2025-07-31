namespace Csdsa.Application.Interfaces;

public interface IFeatureToggleService
{
    Task<bool> IsEnabledAsync(string key);
}
