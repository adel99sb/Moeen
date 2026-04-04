namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Lightweight DTO representing a mosque in list/search results.
/// </summary>
public class MosqueListItem
{
    /// <summary>Gets or sets the unique identifier of the mosque.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the mosque name (Arabic).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional English name of the mosque.</summary>
    public string? NameEn { get; set; }

    /// <summary>Gets or sets the city where the mosque is located.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets the district/neighborhood.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets the URL of the mosque logo/main image.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Gets or sets a value indicating whether the mosque record is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>Gets or sets a value indicating whether the mosque has been approved/published.</summary>
    public bool IsApproved { get; set; }
}
