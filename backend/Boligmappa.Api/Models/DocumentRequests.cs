using System.ComponentModel.DataAnnotations;

namespace Boligmappa.Api.Models;

/// <summary>
/// Payload for creating a document. Id and UploadedAt are assigned server-side.
/// </summary>
public class CreateDocumentRequest
{
    [Required]
    public Guid PropertyId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(DocumentType))]
    public DocumentType DocumentType { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string UploadedBy { get; set; } = string.Empty;
}

/// <summary>
/// Payload for updating an existing document. The route id identifies the target;
/// PropertyId / UploadedAt are treated as immutable and are not editable here.
/// </summary>
public class UpdateDocumentRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(DocumentType))]
    public DocumentType DocumentType { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string UploadedBy { get; set; } = string.Empty;
}
