namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Request DTO for updating an existing mosque.
/// </summary>
public class UpdateMosqueRequest
{
    /// <summary>Gets or sets the updated mosque name (Arabic).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the updated optional English name of the mosque.</summary>
    public string? NameEn { get; set; }

    /// <summary>Gets or sets the updated description of the mosque.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the updated city.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets the updated district/neighborhood.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets the updated full street address.</summary>
    public string? Address { get; set; }

    /// <summary>Gets or sets the updated contact phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Gets or sets the updated contact email address.</summary>
    public string? Email { get; set; }

    /// <summary>Gets or sets the updated mosque capacity.</summary>
    public int? Capacity { get; set; }
}
