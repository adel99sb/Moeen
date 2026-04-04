namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Request DTO for creating a new mosque.
/// </summary>
public class CreateMosqueRequest
{
    /// <summary>Gets or sets the mosque name (Arabic).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional English name of the mosque.</summary>
    public string? NameEn { get; set; }

    /// <summary>Gets or sets a brief description of the mosque.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the city where the mosque is located.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets the district/neighborhood within the city.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets the full street address.</summary>
    public string? Address { get; set; }

    /// <summary>Gets or sets the geographic latitude of the mosque.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the geographic longitude of the mosque.</summary>
    public double? Longitude { get; set; }

    /// <summary>Gets or sets the contact phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Gets or sets the contact email address.</summary>
    public string? Email { get; set; }

    /// <summary>Gets or sets the mosque capacity (number of worshippers).</summary>
    public int? Capacity { get; set; }

    /// <summary>Gets or sets the URL of the mosque logo/main image.</summary>
    public string? LogoUrl { get; set; }
}
