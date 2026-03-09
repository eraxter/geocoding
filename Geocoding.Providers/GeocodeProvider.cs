using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Geocoding.Providers;

public enum GeocodeType
{
    Forward,
    Reverse
}

public abstract class GeocodeProvider
{
    public abstract string Name { get; }

    public abstract bool Enabled { get; }

    public abstract int RequestTimeout { get; }

    public abstract Uri ForwardGeocode(LocationInfo location);

    public abstract Uri ReverseGeocode(LocationInfo location);

    public abstract List<LocationInfo> ParseResults(string responseText);

    public async Task<List<LocationInfo>> PerformGeocode(
        GeocodeType geocodeType,
        LocationInfo location
    )
    {
        List<LocationInfo> results = new();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        HttpClient client = new();
        Uri requestUri = geocodeType == GeocodeType.Forward
            ? ForwardGeocode(location)
            : ReverseGeocode(location);

        using (HttpResponseMessage response = await client.GetAsync(requestUri))
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            using (StreamReader reader = new(response.Content.ReadAsStream()))
            {
                results = ParseResults(reader.ReadToEnd());
            }
        }

        return results;
    }
}
