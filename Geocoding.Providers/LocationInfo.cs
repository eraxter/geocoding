namespace Geocoding.Providers;

public class LocationInfo
{
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State {  get; set; }

    public string? ZipCode { get; set; }

    public string? County { get; set; }

    public string? Country { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? QualityResponse { get; set; }

    public LocationInfo() { }

    public LocationInfo(
        string? address,
        string? city,
        string? state,
        string? zipCode,
        string? county,
        string? country,
        double? latitude,
        double? longitude,
        int? qualityResponse
    )
    {
        Address = address;
        City = city;
        State = state;
        ZipCode = zipCode;
        County = county;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
        QualityResponse = qualityResponse;
    }
}
