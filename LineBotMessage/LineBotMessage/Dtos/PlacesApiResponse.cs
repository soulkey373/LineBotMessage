using Newtonsoft.Json;
using System.Collections.Generic;

public class PlacesApiResponse
{
    [JsonProperty("results")]
    public List<Place> Results { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
}

public class Place
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("geometry")]
    public Geometry Geometry { get; set; }

    [JsonProperty("price_level")]
    public int Price_level { get; set; }
}

public class Geometry
{
    [JsonProperty("location")]
    public Location Location { get; set; }
}

public class Location
{
    [JsonProperty("lat")]
    public double Lat { get; set; }

    [JsonProperty("lng")]
    public double Lng { get; set; }
}