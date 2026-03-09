using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Geocoding.Providers.Google;

public class Google : GeocodeProvider
{
    // Google URL vars
    private const string ADDRESS = "address";
    private const string SENSOR = "sensor";
    private const string LATLNG = "latlng";
    private const string KEY = "key";

    // Google status codes
    private const string STATUS_OK = "OK";
    private const string REQUEST_DENIED = "REQUEST_DENIED";

    public Google() { }

    public override string Name => "Google";

    public override bool Enabled => GoogleSettings.Default.Enabled;

    public override int RequestTimeout => GoogleSettings.Default.RequestTimeout;

    public override Uri ForwardGeocode(LocationInfo location)
    {
        StringBuilder sb = new();

        sb.Append(GoogleSettings.Default.ForwardGeocodeUrl);

        sb.AppendFormat(
            "?{0}={1}",
            KEY,
            GoogleSettings.Default.ApiKey
        );

        sb.AppendFormat(
            "&{0}={1}",
            SENSOR,
            GoogleSettings.Default.SensorEnabled
        );

        List<string?> address = new()
        {
            location.Address,
            location.City,
            location.State,
            location.ZipCode,
            location.County,
            location.Country
        };

        sb.AppendFormat(
            "&{0}={1}",
            ADDRESS,
            HttpUtility.UrlEncode(string.Join(",", address))
        );

        return new Uri(sb.ToString());
    }

    public override Uri ReverseGeocode(LocationInfo location)
    {
        StringBuilder sb = new();

        sb.Append(GoogleSettings.Default.ReverseGeocodeUrl);

        sb.AppendFormat(
            "?{0}={1}",
            KEY,
            GoogleSettings.Default.ApiKey
        );

        sb.AppendFormat(
            "&{0}={1}",
            SENSOR,
            GoogleSettings.Default.SensorEnabled
        );

        sb.AppendFormat(
            "&{0}={1}",
            LATLNG,
            string.Format("{0},{1}", location.Latitude, location.Longitude)
        );

        return new Uri(sb.ToString());
    }

    public override List<LocationInfo> ParseResults(string responseText)
    {
        List<LocationInfo> results = new();
        GoogleGeocode? response = JsonSerializer.Deserialize<GoogleGeocode?>(responseText);

        if (response?.Status != STATUS_OK)
        {
            throw new Exception(response?.ErrorMessage ?? response?.Status);
        }

        foreach (Result result in response?.Results)
        {
            results.Add(
                new LocationInfo(
                    result.Address,
                    result.City,
                    result.State,
                    result.ZipCode,
                    result.County,
                    result.Country,
                    result.Geometry?.Location?.Latitude,
                    result.Geometry?.Location?.Longitude,
                    result.QualityResponse
                )
            );
        }

        return results;
    }
}
