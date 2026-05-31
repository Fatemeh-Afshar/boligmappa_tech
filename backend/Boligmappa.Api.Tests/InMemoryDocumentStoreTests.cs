using Boligmappa.Api.Models;
using Boligmappa.Api.Stores;
using Xunit;

namespace Boligmappa.Api.Tests;

public class InMemoryDocumentStoreTests
{
    private static Document NewDoc(Guid propertyId, DateTimeOffset uploadedAt, string title = "Doc")
        => new()
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            Title = title,
            DocumentType = DocumentType.Other,
            UploadedAt = uploadedAt,
            UploadedBy = "Tester"
        };

    [Fact]
    public async Task GetByPropertyAsync_ReturnsOnlyMatchingProperty_OrderedNewestFirst()
    {
        var store = new InMemoryDocumentStore();
        var propertyId = Guid.NewGuid();
        var other = Guid.NewGuid();

        var older = await store.AddAsync(NewDoc(propertyId, DateTimeOffset.UtcNow.AddDays(-2), "older"));
        var newer = await store.AddAsync(NewDoc(propertyId, DateTimeOffset.UtcNow.AddDays(-1), "newer"));
        await store.AddAsync(NewDoc(other, DateTimeOffset.UtcNow, "other property"));

        var result = (await store.GetByPropertyAsync(propertyId)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(newer.Id, result[0].Id); // newest first
        Assert.Equal(older.Id, result[1].Id);
    }

    [Fact]
    public async Task GetDocumentsNewerThan_ReturnsEmpty_WhenSinceIsNull()
    {
        var store = new InMemoryDocumentStore();
        var propertyId = Guid.NewGuid();
        await store.AddAsync(NewDoc(propertyId, DateTimeOffset.UtcNow));

        var result = await store.GetDocumentsNewerThan(propertyId, since: null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDocumentsNewerThan_ExcludesDocumentWithExactSinceTimestamp()
    {
        // Regression test for the start-code bug: ">=" wrongly re-included a document
        // whose timestamp exactly equals `since`. "Newer than" must be strictly ">".
        var store = new InMemoryDocumentStore();
        var propertyId = Guid.NewGuid();
        var since = DateTimeOffset.UtcNow;
        await store.AddAsync(NewDoc(propertyId, since, "exactly at boundary"));

        var result = await store.GetDocumentsNewerThan(propertyId, since);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDocumentsNewerThan_ReturnsOnlyStrictlyNewerDocsForProperty()
    {
        var store = new InMemoryDocumentStore();
        var propertyId = Guid.NewGuid();
        var other = Guid.NewGuid();
        var since = DateTimeOffset.UtcNow;

        await store.AddAsync(NewDoc(propertyId, since.AddMinutes(-1), "too old"));
        var fresh = await store.AddAsync(NewDoc(propertyId, since.AddMinutes(1), "fresh"));
        await store.AddAsync(NewDoc(other, since.AddMinutes(5), "newer but wrong property"));

        var result = (await store.GetDocumentsNewerThan(propertyId, since)).ToList();

        Assert.Single(result);
        Assert.Equal(fresh.Id, result[0].Id);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenDocumentMissing()
    {
        var store = new InMemoryDocumentStore();

        var updated = await store.UpdateAsync(NewDoc(Guid.NewGuid(), DateTimeOffset.UtcNow));

        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenDocumentMissing()
    {
        var store = new InMemoryDocumentStore();

        var deleted = await store.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingDocument()
    {
        var store = new InMemoryDocumentStore();
        var doc = await store.AddAsync(NewDoc(Guid.NewGuid(), DateTimeOffset.UtcNow));

        var deleted = await store.DeleteAsync(doc.Id);

        Assert.True(deleted);
        Assert.Null(await store.GetByIdAsync(doc.Id));
    }
}
