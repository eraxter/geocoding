using System.Collections.Generic;

namespace Geocoding.Providers;

public class LocationResult
{
    public string? Provider {  get; set; }

    public List<LocationInfo>? Locations { get; set; }

    public LocationResult()
    {
        Locations = new List<LocationInfo>();
    }

    public LocationResult(
        string? provider,
        List<LocationInfo>? locations
    )
    {
        Provider = provider;
        Locations = locations;
    }
}
