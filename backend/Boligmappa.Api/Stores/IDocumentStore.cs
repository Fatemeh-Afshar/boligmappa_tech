using Boligmappa.Api.Models;

namespace Boligmappa.Api.Stores;

/// <summary>
/// Persistence abstraction for documents. Backed by an in-memory store here,
/// but the interface keeps the service layer free of storage concerns so a real
/// database could be swapped in later without touching business logic.
/// </summary>
public interface IDocumentStore
{
    Task<IEnumerable<Document>> GetByPropertyAsync(Guid propertyId);
    Task<Document?> GetByIdAsync(Guid id);
    Task<Document> AddAsync(Document document);
    Task<bool> UpdateAsync(Document document);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Document>> GetDocumentsNewerThan(Guid propertyId, DateTimeOffset? since);
}
