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
                                typeof(GeocodeProvider).IsAssignableFrom(type) &&
                                !type.IsAbstract && type.IsClass
                            )
                            .Select(type => Activator.CreateInstance(type) as GeocodeProvider)
                            .FirstOrDefault(provider => provider != null && provider.Enabled);
                    }
                }
            }

            return _provider ?? throw new Exception("No Provider Found");
        }
    }

    public GeocodingProvider() { }

    public override async Task<LocationResult> LookupByAddressAsync(LocationInfo location)
    {
        List<LocationInfo> results = await Provider.PerformGeocode(
            GeocodeType.Forward,
            location,
            Provider.RequestTimeout
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
            location,
            Provider.RequestTimeout
        );

        return new LocationResult(
            Provider.Name,
            results
        );
    }
}
