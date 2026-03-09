using System.Threading.Tasks;

namespace Geocoding.Providers;

public interface IAddressProvider
{
    Task<LocationResult> LookupByAddressAsync(LocationInfo location);

    Task<LocationResult> LookupByCoordsAsync(LocationInfo location);
}

public abstract class ProviderBase : IAddressProvider
{
    public abstract Task<LocationResult> LookupByAddressAsync(LocationInfo location);

    public abstract Task<LocationResult> LookupByCoordsAsync(LocationInfo location);
}
