namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Request DTO for searching and filtering mosques with pagination support.
/// </summary>
public class MosqueSearchRequest
{
    /// <summary>Gets or sets the free-text search term (matches name, city, district, or address).</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Gets or sets a city filter.</summary>
    public string? City { get; set; }

    /// <summary>Gets or sets a district filter.</summary>
    public string? District { get; set; }

    /// <summary>Gets or sets a filter for the active status. <c>null</c> means no filter.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Gets or sets a filter for the approval status. <c>null</c> means no filter.</summary>
    public bool? IsApproved { get; set; }

    /// <summary>Gets or sets the page number (1-based). Defaults to 1.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Gets or sets the page size. Defaults to 20.</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Gets or sets the field name to sort by. Defaults to "Name".</summary>
    public string? SortBy { get; set; } = "Name";

    /// <summary>Gets or sets a value indicating whether to sort in descending order.</summary>
    public bool SortDescending { get; set; }
}
