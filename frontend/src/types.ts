// Mirrors the backend domain model. DocumentType values are the enum *names*
// because the API serializes enums as strings (see JsonStringEnumConverter).

export const DOCUMENT_TYPES = [
  'BuildingPermit',
  'QualityAssurance',
  'Warranty',
  'Invoice',
  'Other'
] as const;

export type DocumentType = (typeof DOCUMENT_TYPES)[number];

export interface Document {
  id: string;
  propertyId: string;
  title: string;
  documentType: DocumentType;
  uploadedAt: string; // ISO 8601
  uploadedBy: string;
}

/** Fields a user can set when creating or editing a document. */
export interface DocumentInput {
  title: string;
  documentType: DocumentType;
  uploadedBy: string;
}

/** Human-friendly labels for the document type dropdown. */
export const DOCUMENT_TYPE_LABELS: Record<DocumentType, string> = {
  BuildingPermit: 'Byggetillatelse',
  QualityAssurance: 'Kvalitetssikring',
  Warranty: 'Garanti',
  Invoice: 'Faktura',
  Other: 'Annet'
};
