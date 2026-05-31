import { useCallback, useEffect, useState } from 'react';
import { documentsApi } from './api';
import { DocumentForm } from './components/DocumentForm';
import { DocumentList } from './components/DocumentList';
import type { Document, DocumentInput } from './types';

const PROPERTY_ID = import.meta.env.VITE_SAMPLE_PROPERTY_ID ?? '11111111-1111-1111-1111-111111111111';

export default function App() {
  const [documents, setDocuments] = useState<Document[]>([]);
  const [editing, setEditing] = useState<Document | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setDocuments(await documentsApi.listByProperty(PROPERTY_ID));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Kunne ikke laste dokumenter');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const handleSubmit = async (input: DocumentInput) => {
    try {
      if (editing) {
        await documentsApi.update(editing.id, input);
        setEditing(null);
      } else {
        await documentsApi.create(PROPERTY_ID, input);
      }
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Lagring feilet');
    }
  };

  const handleDelete = async (doc: Document) => {
    if (!window.confirm(`Slette «${doc.title}»?`)) return;
    try {
      await documentsApi.remove(doc.id);
      if (editing?.id === doc.id) setEditing(null);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Sletting feilet');
    }
  };

  return (
    <main className="container">
      <header>
        <h1>Boligmappa</h1>
        <p className="subtitle">Dokumenter for eiendom {PROPERTY_ID}</p>
      </header>

      {error && <div className="error">{error}</div>}

      <DocumentForm editing={editing} onSubmit={handleSubmit} onCancelEdit={() => setEditing(null)} />

      <section className="card card--list">
        <h2>Dokumenter</h2>
        {loading ? (
          <p>Laster …</p>
        ) : (
          <DocumentList documents={documents} onEdit={setEditing} onDelete={handleDelete} />
        )}
      </section>
    </main>
  );
}
