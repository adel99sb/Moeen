using Moeen.Application.DTOs.Mosques;

namespace Moeen.Application.Contracts;

/// <summary>
/// Defines the contract for mosque management operations (الجوامع).
/// </summary>
public interface IMosqueService
{
    // ──────────────────────────────────────────────
    // Core CRUD
    // ──────────────────────────────────────────────

    /// <summary>
    /// Creates a new mosque record.
    /// </summary>
    /// <param name="request">The data required to create the mosque.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The full details of the newly created mosque.</returns>
    Task<MosqueResponse> CreateAsync(CreateMosqueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the core details of an existing mosque.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to update.</param>
    /// <param name="request">The updated mosque data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated mosque details.</returns>
    Task<MosqueResponse> UpdateAsync(Guid id, UpdateMosqueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a mosque record by marking it as deleted without removing it from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the full details of a mosque by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// The mosque details, or <c>null</c> if no mosque with the given identifier exists.
    /// </returns>
    Task<MosqueResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for mosques using optional filters and returns a paginated result.
    /// </summary>
    /// <param name="request">Filtering, sorting, and pagination parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of mosques matching the search criteria.</returns>
    Task<PagedResult<MosqueListItem>> SearchAsync(MosqueSearchRequest request, CancellationToken cancellationToken = default);

    // ──────────────────────────────────────────────
    // Status & Lifecycle
    // ──────────────────────────────────────────────

    /// <summary>
    /// Activates a mosque, making it visible and operational.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to activate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a mosque, hiding it from public listings without deleting it.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to deactivate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approves and publishes a mosque after administrative review.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to approve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes approval and unpublishes a mosque, removing it from public view.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque to unpublish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UnpublishAsync(Guid id, CancellationToken cancellationToken = default);

    // ──────────────────────────────────────────────
    // Location
    // ──────────────────────────────────────────────

    /// <summary>
    /// Updates the address and geographic coordinates of a mosque.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque.</param>
    /// <param name="request">The new location data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated mosque details.</returns>
    Task<MosqueResponse> UpdateLocationAsync(Guid id, UpdateMosqueLocationRequest request, CancellationToken cancellationToken = default);

    // ──────────────────────────────────────────────
    // Media
    // ──────────────────────────────────────────────

    /// <summary>
    /// Updates the logo and image gallery of a mosque.
    /// </summary>
    /// <param name="id">The unique identifier of the mosque.</param>
    /// <param name="request">The new image data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated mosque details.</returns>
    Task<MosqueResponse> UpdateImagesAsync(Guid id, UpdateMosqueImageRequest request, CancellationToken cancellationToken = default);

    // ──────────────────────────────────────────────
    // Validation Helpers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Checks whether a mosque with the given name already exists.
    /// </summary>
    /// <param name="name">The name to check for uniqueness.</param>
    /// <param name="excludeId">
    /// An optional mosque identifier to exclude from the check (useful during updates).
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the name is already taken; otherwise, <c>false</c>.</returns>
    Task<bool> IsNameTakenAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
