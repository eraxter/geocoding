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

        Console.WriteLine("\n\npress any key to continue...");
        Console.ReadKey();
    }

    static async Task TestGeocode()
    {
        GeocodeType geocodeType = GeocodeType.Forward;
        GeocodingProvider provider = new();
        LocationInfo location = new();

        Console.WriteLine("Geocode Type?");
        var types = Enum.GetValues<GeocodeType>();
        var locationProps = location.GetType().GetProperties();

        for (int i = 0; i < types.Length; i++)
        {
            Console.WriteLine(string.Format("{0}. {1}", i + 1, types[i]));
        }

        if (
            int.TryParse(Console.ReadLine(), out int type) &&
            (type - 1) < types.Length
        )
        {
            geocodeType = types[type - 1];
        }
        else
        {
            throw new Exception("Invalid Geocode Type!");
        }

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
            if (
                forwardProps.Contains(prop.Name) == false &&
                reverseProps.Contains(prop.Name) == false
            )
            {
                continue;
            }

            if (
                geocodeType == GeocodeType.Forward &&
                forwardProps.Contains(prop.Name) == false
            )
            {
                continue;
            }

            if (
                geocodeType == GeocodeType.Reverse &&
                reverseProps.Contains(prop.Name) == false
            )
            {
                continue;
            }

            Console.Write(string.Format("{0}: ", prop.Name));
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

        LocationResult result = geocodeType == GeocodeType.Forward
            ? await provider.LookupByAddressAsync(location)
            : await provider.LookupByCoordsAsync(location);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string output = JsonSerializer.Serialize(result, options);

        Console.WriteLine(output);
    }
}
