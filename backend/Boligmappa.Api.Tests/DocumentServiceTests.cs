using Boligmappa.Api.Models;
using Boligmappa.Api.Services;
using Boligmappa.Api.Stores;
using Xunit;

namespace Boligmappa.Api.Tests;

public class DocumentServiceTests
{
    private static DocumentService CreateService() => new(new InMemoryDocumentStore());

    private static CreateDocumentRequest CreateRequest(Guid propertyId) => new()
    {
        PropertyId = propertyId,
        Title = "Garanti",
        DocumentType = DocumentType.Warranty,
        UploadedBy = "VVS AS"
    };

    [Fact]
    public async Task CreateAsync_AssignsId_StampsUploadedAt_AndMapsFields()
    {
        var service = CreateService();
        var propertyId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        var created = await service.CreateAsync(CreateRequest(propertyId));

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(propertyId, created.PropertyId);
        Assert.Equal("Garanti", created.Title);
        Assert.Equal(DocumentType.Warranty, created.DocumentType);
        Assert.Equal("VVS AS", created.UploadedBy);
        Assert.InRange(created.UploadedAt, before, DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_PersistsDocument_RetrievableById()
    {
        var service = CreateService();

        var created = await service.CreateAsync(CreateRequest(Guid.NewGuid()));
        var fetched = await service.GetByIdAsync(created.Id);

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = CreateService();

        Assert.Null(await service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetForPropertyAsync_ReturnsOnlyDocumentsForThatProperty()
    {
        var service = CreateService();
        var propertyId = Guid.NewGuid();
        await service.CreateAsync(CreateRequest(propertyId));
        await service.CreateAsync(CreateRequest(propertyId));
        await service.CreateAsync(CreateRequest(Guid.NewGuid())); // different property

        var result = await service.GetForPropertyAsync(propertyId);

        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.Equal(propertyId, d.PropertyId));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenDocumentMissing()
    {
        var service = CreateService();

        var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateDocumentRequest
        {
            Title = "X",
            DocumentType = DocumentType.Other,
            UploadedBy = "Y"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ChangesMutableFields_ButPreservesPropertyIdAndUploadedAt()
    {
        var service = CreateService();
        var propertyId = Guid.NewGuid();
        var created = await service.CreateAsync(CreateRequest(propertyId));

        var updated = await service.UpdateAsync(created.Id, new UpdateDocumentRequest
        {
            Title = "Oppdatert tittel",
            DocumentType = DocumentType.Invoice,
            UploadedBy = "Regnskap AS"
        });

        Assert.NotNull(updated);
        Assert.Equal("Oppdatert tittel", updated!.Title);
        Assert.Equal(DocumentType.Invoice, updated.DocumentType);
        Assert.Equal("Regnskap AS", updated.UploadedBy);
        // Immutable after creation:
        Assert.Equal(propertyId, updated.PropertyId);
        Assert.Equal(created.UploadedAt, updated.UploadedAt);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_AndRemovesDocument()
    {
        var service = CreateService();
        var created = await service.CreateAsync(CreateRequest(Guid.NewGuid()));

        var deleted = await service.DeleteAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenDocumentMissing()
    {
        var service = CreateService();

        Assert.False(await service.DeleteAsync(Guid.NewGuid()));
    }
}
