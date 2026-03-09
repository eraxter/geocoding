using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geocoding.Providers;

public class GeocodingProvider : ProviderBase
{
    private static object _lock = new();

    private static GeocodeProvider? _provider = null;

    public static GeocodeProvider Provider
    {
        get
        {
            if (_provider == null)
            {
                lock (_lock)
                {
                    if (_provider == null)
                    {
                        _provider = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(type =>
                                typeof(GeocodingProvider).IsAssignableFrom(type) &&
                                !type.IsAbstract && type.IsClass
                            )
                            .Select(type => Activator.CreateInstance(type) as GeocodeProvider)
                            .First(provider => provider != null && provider.Enabled);
                    }
                }
            }

            return _provider!;
        }
    }

    public GeocodingProvider() { }

    public override async Task<LocationResult> LookupByAddressAsync(LocationInfo location)
    {
        List<LocationInfo> results = await Provider.PerformGeocode(
            GeocodeType.Forward,
            location
        );

        return new LocationResult(
            Provider.Name,
            results
        );
    }

    public override async Task<LocationResult> LookupByCoordsAsync(LocationInfo location)
    {
        List<LocationInfo> results = await Provider.PerformGeocode(
            GeocodeType.Reverse,
            location
        );

        return new LocationResult(
            Provider.Name,
            results
        );
    }
}
