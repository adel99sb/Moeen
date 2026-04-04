namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Request DTO for updating the geographic location and address of a mosque.
/// </summary>
public class UpdateMosqueLocationRequest
{
    /// <summary>Gets or sets the city.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets the district/neighborhood.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets the full street address.</summary>
    public string? Address { get; set; }

    /// <summary>Gets or sets the geographic latitude.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the geographic longitude.</summary>
    public double? Longitude { get; set; }
}
