import { useEffect, useState } from 'react';
import {
  DOCUMENT_TYPES,
  DOCUMENT_TYPE_LABELS,
  type Document,
  type DocumentInput,
  type DocumentType
} from '../types';

interface Props {
  /** When set, the form edits this document; otherwise it creates a new one. */
  editing: Document | null;
  onSubmit: (input: DocumentInput) => Promise<void>;
  onCancelEdit: () => void;
}

const emptyForm: DocumentInput = {
  title: '',
  documentType: 'Other',
  uploadedBy: ''
};

export function DocumentForm({ editing, onSubmit, onCancelEdit }: Props) {
  const [form, setForm] = useState<DocumentInput>(emptyForm);
  const [submitting, setSubmitting] = useState(false);

  // Load the selected document into the form when entering edit mode.
  useEffect(() => {
    if (editing) {
      setForm({
        title: editing.title,
        documentType: editing.documentType,
        uploadedBy: editing.uploadedBy
      });
    } else {
      setForm(emptyForm);
    }
  }, [editing]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await onSubmit(form);
      if (!editing) setForm(emptyForm); // reset after a create
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form className="card" onSubmit={handleSubmit}>
      <h2>{editing ? 'Rediger dokument' : 'Nytt dokument'}</h2>

      <label>
        Tittel
        <input
          type="text"
          required
          value={form.title}
          onChange={(e) => setForm({ ...form, title: e.target.value })}
        />
      </label>

      <label>
        Dokumenttype
        <select
          value={form.documentType}
          onChange={(e) => setForm({ ...form, documentType: e.target.value as DocumentType })}
        >
          {DOCUMENT_TYPES.map((type) => (
            <option key={type} value={type}>
              {DOCUMENT_TYPE_LABELS[type]}
            </option>
          ))}
        </select>
      </label>

      <label>
        Lastet opp av
        <input
          type="text"
          required
          value={form.uploadedBy}
          onChange={(e) => setForm({ ...form, uploadedBy: e.target.value })}
        />
      </label>

      <div className="actions">
        <button type="submit" disabled={submitting}>
          {editing ? 'Lagre endringer' : 'Opprett'}
        </button>
        {editing && (
          <button type="button" className="secondary" onClick={onCancelEdit} disabled={submitting}>
            Avbryt
          </button>
        )}
      </div>
    </form>
  );
}
