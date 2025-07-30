namespace Csdsa.Application.Common.Interfaces;

public interface IFeatureToggleService
{
    Task<bool> IsEnabledAsync(string key);
}
