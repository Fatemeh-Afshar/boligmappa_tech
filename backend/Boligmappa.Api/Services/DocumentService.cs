using Boligmappa.Api.Models;
using Boligmappa.Api.Stores;

namespace Boligmappa.Api.Services;

/// <summary>
/// Business logic for documents. Owns the rules the store shouldn't care about:
/// assigning ids, stamping the upload time, and mapping request DTOs onto entities.
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly IDocumentStore _store;

    public DocumentService(IDocumentStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<Document>> GetForPropertyAsync(Guid propertyId)
        => _store.GetByPropertyAsync(propertyId);

    public Task<Document?> GetByIdAsync(Guid id)
        => _store.GetByIdAsync(id);

    public Task<Document> CreateAsync(CreateDocumentRequest request)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            PropertyId = request.PropertyId,
            Title = request.Title,
            DocumentType = request.DocumentType,
            UploadedBy = request.UploadedBy,
            // Server owns the timestamp so clients can't backdate uploads.
            UploadedAt = DateTimeOffset.UtcNow
        };

        return _store.AddAsync(document);
    }

    public async Task<Document?> UpdateAsync(Guid id, UpdateDocumentRequest request)
    {
        var existing = await _store.GetByIdAsync(id);
        if (existing is null)
            return null;

        // PropertyId and UploadedAt are intentionally preserved (immutable after creation).
        existing.Title = request.Title;
        existing.DocumentType = request.DocumentType;
        existing.UploadedBy = request.UploadedBy;

        await _store.UpdateAsync(existing);
        return existing;
    }

    public Task<bool> DeleteAsync(Guid id)
        => _store.DeleteAsync(id);
}
