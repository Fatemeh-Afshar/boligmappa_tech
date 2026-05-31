import { DOCUMENT_TYPE_LABELS, type Document } from '../types';

interface Props {
  documents: Document[];
  onEdit: (document: Document) => void;
  onDelete: (document: Document) => void;
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('no-NO', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
}

export function DocumentList({ documents, onEdit, onDelete }: Props) {
  if (documents.length === 0) {
    return <p className="empty">Ingen dokumenter for denne eiendommen ennå.</p>;
  }

  return (
    <table className="doc-table">
      <thead>
        <tr>
          <th>Tittel</th>
          <th>Type</th>
          <th>Lastet opp</th>
          <th>Av</th>
          <th aria-label="Handlinger" />
        </tr>
      </thead>
      <tbody>
        {documents.map((doc) => (
          <tr key={doc.id}>
            <td>{doc.title}</td>
            <td>{DOCUMENT_TYPE_LABELS[doc.documentType]}</td>
            <td>{formatDate(doc.uploadedAt)}</td>
            <td>{doc.uploadedBy}</td>
            <td className="row-actions">
              <button className="link" onClick={() => onEdit(doc)}>
                Rediger
              </button>
              <button className="link danger" onClick={() => onDelete(doc)}>
                Slett
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
