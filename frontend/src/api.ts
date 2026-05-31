import type { Document, DocumentInput } from './types';

// In production the SPA is served by the API itself, so calls are same-origin
// (empty base => "/api/..."). In dev the Vite server proxies via VITE_API_BASE_URL.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

async function handle<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const body = await response.text();
    throw new Error(`API ${response.status} ${response.statusText}${body ? `: ${body}` : ''}`);
  }
  // 204 No Content has no body to parse.
  if (response.status === 204) {
    return undefined as T;
  }
  return (await response.json()) as T;
}

export const documentsApi = {
  listByProperty(propertyId: string): Promise<Document[]> {
    return fetch(`${API_BASE_URL}/api/properties/${propertyId}/documents`).then(handle<Document[]>);
  },

  getById(id: string): Promise<Document> {
    return fetch(`${API_BASE_URL}/api/documents/${id}`).then(handle<Document>);
  },

  create(propertyId: string, input: DocumentInput): Promise<Document> {
    return fetch(`${API_BASE_URL}/api/documents`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ propertyId, ...input })
    }).then(handle<Document>);
  },

  update(id: string, input: DocumentInput): Promise<Document> {
    return fetch(`${API_BASE_URL}/api/documents/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(input)
    }).then(handle<Document>);
  },

  remove(id: string): Promise<void> {
    return fetch(`${API_BASE_URL}/api/documents/${id}`, { method: 'DELETE' }).then(handle<void>);
  }
};
