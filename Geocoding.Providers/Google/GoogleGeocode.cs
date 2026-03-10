using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geocoding.Providers.Google;

public class GoogleGeocode
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("results")]
    public List<Result>? Results { get; set; }
}

public enum MatchQuality : int
{
    Address = 0,
    AddressInterpolated = 0,
    AddressInterpolatedOffset = 0,
    CentroidStreet = 0,
    CentroidZip4 = 1,
    CentroidZip2 = 1,
    CentroidZip = 1,
    CentroidCity = 1,
    CentroidCounty = 2,
    CentroidState = 2,
    CentroidCountry = 2,
    CentroidUnknown = 2,
    Unknown = 99
}

public class LocationPrecisionLevels
{
    public const string ADDRESS = "ROOFTOP";
    public const string INTERPOLATED = "RANGE_INTERPOLATED";
    public const string CENTROID = "GEOMETRIC_CENTER";
    public const string APPROXIMATE = "APPROXIMATE";
}

public class AddressComponentTypes
{
    public const string STREET_ADDRESS = "street_address";
    public const string ROUTE = "route";
    public const string INTERSECTION = "intersection";
    public const string POLITICAL = "political";
    public const string COUNTRY = "country";
    public const string ADMINISTRATIVE_AREA_LEVEL_1 = "administrative_area_level_1";
    public const string ADMINISTRATIVE_AREA_LEVEL_2 = "administrative_area_level_2";
    public const string ADMINISTRATIVE_AREA_LEVEL_3 = "administrative_area_level_3";
    public const string COLLOQUIAL_AREA = "colloquial_area";
    public const string LOCALITY = "locality";
    public const string SUBLOCALITY = "sublocality";
    public const string NEIGHBORHOOD = "neighborhood";
    public const string PREMISE = "premise";
    public const string SUBPREMISE = "subpremise";
    public const string POSTAL_CODE = "postal_code";
    public const string NATURAL_FEATURE = "natural_feature";
    public const string AIRPORT = "airport";
    public const string PARK = "park";
    public const string POINT_OF_INTEREST = "point_of_interest";
    public const string POST_BOX = "post_box";
    public const string STREET_NUMBER = "street_number";
    public const string FLOOR = "floor";
    public const string ROOM = "room";
}

public class AddressComponent
{
    [JsonPropertyName("long_name")]
    public string? LongName { get; set; }

    [JsonPropertyName("short_name")]
    public string? ShortName { get; set; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }
}

public class Location
{
    [JsonPropertyName("lat")]
    public double? Latitude { get; set; }

    [JsonPropertyName("lng")]
    public double? Longitude { get; set; }
}

public class Viewport
{
    [JsonPropertyName("northeast")]
    public Location? Northeast { get; set; }

    [JsonPropertyName("southwest")]
    public Location? Southwest { get; set; }
}

public class Geometry
{
    [JsonPropertyName("location_type")]
    public string? LocationType { get; set; }

    [JsonPropertyName("location")]
    public Location? Location { get; set; }

    [JsonPropertyName("viewport")]
    public Viewport? Viewport { get; set; }
}

public class Result
{
    [JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; set; }

    [JsonPropertyName("geometry")]
    public Geometry? Geometry { get; set; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }

    [JsonPropertyName("address_components")]
    public List<AddressComponent>? AddressComponents { get; set; }

    public string? Address
    {
        get
        {
            AddressComponent? streetNumber = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.STREET_NUMBER)
            );

            AddressComponent? route = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.ROUTE)
            );

            return string.Format(
                "{0} {1}",
                streetNumber?.ShortName,
                route?.ShortName
            ).Trim();
        }
    }

    public string? City
    {
        get
        {
            AddressComponent? city = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.LOCALITY)
            );

            return city?.ShortName;
        }
    }

    public string? State
    {
        get
        {
            AddressComponent? state = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.ADMINISTRATIVE_AREA_LEVEL_1)
            );

            return state?.ShortName;
        }
    }

    public string? ZipCode
    {
        get
        {
            AddressComponent? zipCode = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.POSTAL_CODE)
            );

            return zipCode?.ShortName;
        }
    }

    public string? County
    {
        get
        {
            AddressComponent? county = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.ADMINISTRATIVE_AREA_LEVEL_2)
            );

            return county?.ShortName;
        }
    }

    public string? Country
    {
        get
        {
            AddressComponent? country = AddressComponents!.SingleOrDefault(x =>
                x.Types!.Contains(AddressComponentTypes.COUNTRY)
            );

            return country?.ShortName;
        }
    }

    public int? QualityResponse
    {
        get
        {
            int precision = (int)MatchQuality.Unknown;

            switch (Geometry!.LocationType)
            {
                case LocationPrecisionLevels.ADDRESS:
                    precision = (int)MatchQuality.Address;
                    break;
                case LocationPrecisionLevels.INTERPOLATED:
                    precision = (int)MatchQuality.AddressInterpolated;
                    break;
                case LocationPrecisionLevels.CENTROID:
                case LocationPrecisionLevels.APPROXIMATE:
                    if (Types!.Count > 0)
                    {
                        switch (Types[0])
                        {
                            case AddressComponentTypes.ROUTE:
                                precision = (int)MatchQuality.AddressInterpolated;
                                break;
                            case AddressComponentTypes.POSTAL_CODE:
                                precision = (int)MatchQuality.CentroidZip;
                                break;
                            case AddressComponentTypes.LOCALITY:
                                precision = (int)MatchQuality.CentroidCity;
                                break;
                            case AddressComponentTypes.ADMINISTRATIVE_AREA_LEVEL_2:
                                precision = (int)MatchQuality.CentroidCounty;
                                break;
                            case AddressComponentTypes.ADMINISTRATIVE_AREA_LEVEL_1:
                                precision = (int)MatchQuality.CentroidState;
                                break;
                            case AddressComponentTypes.COUNTRY:
                                precision = (int)MatchQuality.CentroidCountry;
                                break;
                            default:
                                precision = (int)MatchQuality.CentroidUnknown;
                                break;
                        }
                    }
                    break;
            }

            return precision;
        }
    }
}
