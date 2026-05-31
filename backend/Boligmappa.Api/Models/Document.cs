namespace Boligmappa.Api.Models;

/// <summary>
/// A piece of documentation attached to a property (building permit, warranty, invoice, etc.).
/// </summary>
public class Document
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}
