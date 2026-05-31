using System.Collections.Concurrent;
using Boligmappa.Api.Models;

namespace Boligmappa.Api.Stores;

/// <summary>
/// Thread-safe in-memory document store. Seeded with a little sample data so the
/// frontend has something to show on first load. Registered as a singleton so the
/// data survives across requests for the lifetime of the process.
/// </summary>
public class InMemoryDocumentStore : IDocumentStore
{
    // A known property id so the seeded data is easy to query from the UI/Swagger.
    public static readonly Guid SamplePropertyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly ConcurrentDictionary<Guid, Document> _documents = new();

    public InMemoryDocumentStore()
    {
        Seed();
    }

    public Task<IEnumerable<Document>> GetByPropertyAsync(Guid propertyId)
    {
        var matches = _documents.Values
            .Where(d => d.PropertyId == propertyId)
            .OrderByDescending(d => d.UploadedAt)
            .ToList();

        return Task.FromResult<IEnumerable<Document>>(matches);
    }

    public Task<Document?> GetByIdAsync(Guid id)
    {
        _documents.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }

    public Task<Document> AddAsync(Document document)
    {
        _documents[document.Id] = document;
        return Task.FromResult(document);
    }

    public Task<bool> UpdateAsync(Document document)
    {
        // Only replace if it already exists; never create via update.
        if (!_documents.ContainsKey(document.Id))
            return Task.FromResult(false);

        _documents[document.Id] = document;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_documents.TryRemove(id, out _));
    }

    /// <summary>
    /// Returns documents for a property uploaded strictly after <paramref name="since"/>.
    /// </summary>
    /// <remarks>
    /// Provided as start code. Two bugs were fixed from the original:
    ///   1. The comparison used `&gt;=` ("newer than or equal to"), which contradicts the
    ///      method name and would re-include a document whose timestamp exactly equals
    ///      `since` (e.g. on incremental sync). Changed to strictly greater-than `&gt;`.
    ///   2. `_documents` is a dictionary here, so we iterate `_documents.Values`.
    /// </remarks>
    public async Task<IEnumerable<Document>> GetDocumentsNewerThan(Guid propertyId, DateTimeOffset? since)
    {
        if (since == null)
            return Enumerable.Empty<Document>();

        return await Task.FromResult(
            _documents.Values
                .Where(d => d.PropertyId == propertyId && d.UploadedAt > since)
                .ToList()
        );
    }

    private void Seed()
    {
        var seedData = new[]
        {
            new Document
            {
                Id = Guid.NewGuid(),
                PropertyId = SamplePropertyId,
                Title = "Ferdigattest hovedhus",
                DocumentType = DocumentType.BuildingPermit,
                UploadedAt = DateTimeOffset.UtcNow.AddDays(-30),
                UploadedBy = "Kommunen"
            },
            new Document
            {
                Id = Guid.NewGuid(),
                PropertyId = SamplePropertyId,
                Title = "Garanti varmepumpe",
                DocumentType = DocumentType.Warranty,
                UploadedAt = DateTimeOffset.UtcNow.AddDays(-7),
                UploadedBy = "VVS AS"
            }
        };

        foreach (var doc in seedData)
            _documents[doc.Id] = doc;
    }
}
