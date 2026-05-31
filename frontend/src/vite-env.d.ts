/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string;
  readonly VITE_SAMPLE_PROPERTY_ID?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
