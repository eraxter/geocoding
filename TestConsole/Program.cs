using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Geocoding.Providers;

namespace TestConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        await TestGeocode();
    }

    static async Task TestGeocode()
    {
        GeocodeType geocodeType = GeocodeType.Forward;
        GeocodingProvider provider = new();
        LocationInfo location = new();

        var types = Enum.GetValues<GeocodeType>();
        var locationProps = location.GetType().GetProperties();

        for (var i = 0; i < types.Length; i++)
        {
            Console.WriteLine(
                string.Format("{0}. {1}", i + 1, types[i])
            );
        }

        Console.Write(
            string.Format("Geocode Type ({0}-{1}) ? ", 1, types.Length)
        );

        if (
            int.TryParse(Console.ReadLine(), out int type)
            && (type - 1) < types.Length
        )
        {
            geocodeType = types[type - 1];
        }
        else
        {
            throw new Exception("Invalid Geocode Type!");
        }

        Console.WriteLine();

        List<string> forwardProps = [
            "Address",
            "City",
            "State",
            "ZipCode",
            "County",
            "Country"
        ];

        List<string> reverseProps = [
            "Latitude",
            "Longitude"
        ];

        foreach (var prop in locationProps)
        {
            bool isForward = geocodeType == GeocodeType.Forward
                && forwardProps.Contains(prop.Name);

            bool isReverse = geocodeType == GeocodeType.Reverse
                && reverseProps.Contains(prop.Name);

            if (isForward || isReverse)
            {
                Console.Write(string.Format("{0} ? ", prop.Name));
                string? value = Console.ReadLine();

                if (value != null)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(location, value);
                    }
                    else if (prop.PropertyType == typeof(double?))
                    {
                        prop.SetValue(location, Convert.ToDouble(value));
                    }
                }
            }
        }

        LocationResult result = geocodeType == GeocodeType.Forward
            ? await provider.LookupByAddressAsync(location)
            : await provider.LookupByCoordsAsync(location);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string output = JsonSerializer.Serialize(result, options);

        Console.WriteLine(string.Format("\nResponse:\n{0}", output));
    }
}
