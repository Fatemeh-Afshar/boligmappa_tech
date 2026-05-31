using Boligmappa.Api.Models;

namespace Boligmappa.Api.Services;

public interface IDocumentService
{
    Task<IEnumerable<Document>> GetForPropertyAsync(Guid propertyId);
    Task<Document?> GetByIdAsync(Guid id);
    Task<Document> CreateAsync(CreateDocumentRequest request);
    Task<Document?> UpdateAsync(Guid id, UpdateDocumentRequest request);
    Task<bool> DeleteAsync(Guid id);
}
