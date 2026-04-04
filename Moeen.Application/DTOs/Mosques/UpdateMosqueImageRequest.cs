namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Request DTO for updating the logo or images of a mosque.
/// </summary>
public class UpdateMosqueImageRequest
{
    /// <summary>Gets or sets the URL of the mosque logo/main image.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Gets or sets additional image URLs associated with the mosque.</summary>
    public IList<string> ImageUrls { get; set; } = [];
}
