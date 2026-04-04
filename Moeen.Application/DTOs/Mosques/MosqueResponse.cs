namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Response DTO representing the full details of a mosque.
/// </summary>
public class MosqueResponse
{
    /// <summary>Gets or sets the unique identifier of the mosque.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the mosque name (Arabic).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional English name of the mosque.</summary>
    public string? NameEn { get; set; }

    /// <summary>Gets or sets the description of the mosque.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the city where the mosque is located.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets the district/neighborhood.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets the full street address.</summary>
    public string? Address { get; set; }

    /// <summary>Gets or sets the geographic latitude.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the geographic longitude.</summary>
    public double? Longitude { get; set; }

    /// <summary>Gets or sets the contact phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Gets or sets the contact email address.</summary>
    public string? Email { get; set; }

    /// <summary>Gets or sets the mosque capacity.</summary>
    public int? Capacity { get; set; }

    /// <summary>Gets or sets the URL of the mosque logo/main image.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Gets or sets additional image URLs associated with the mosque.</summary>
    public IReadOnlyList<string> ImageUrls { get; set; } = [];

    /// <summary>Gets or sets a value indicating whether the mosque record is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>Gets or sets a value indicating whether the mosque has been approved/published.</summary>
    public bool IsApproved { get; set; }

    /// <summary>Gets or sets the UTC date and time when the record was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC date and time when the record was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }
}
