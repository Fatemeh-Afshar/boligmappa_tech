namespace Boligmappa.Api.Models;

/// <summary>
/// The category a document falls under. Mirrors the domain model in the task spec.
/// </summary>
public enum DocumentType
{
    BuildingPermit,
    QualityAssurance,
    Warranty,
    Invoice,
    Other
}
