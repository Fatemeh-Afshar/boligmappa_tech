# Boligmappa — Eiendomsdokumenter

En enkel full-stack CRUD-applikasjon for å forvalte dokumenter knyttet til en eiendom.

- **Backend:** ASP.NET Core Web API (.NET 8), lagdelt (controller → service → store), in-memory lagring.
- **Frontend:** React + TypeScript (Vite).

---

## Kjøre prosjektet

### Backend

Krever [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd backend/Boligmappa.Api
dotnet run
```

API-et starter på `http://localhost:5080`. Swagger-UI er tilgjengelig på
`http://localhost:5080/swagger` i Development.

### Frontend

Krever Node 18+.

```bash
cd frontend
npm install
npm run dev
```

UI-et kjører på `http://localhost:5173` og snakker med backend via
`VITE_API_BASE_URL` (se `frontend/.env`). Backend tillater CORS fra
Vite-dev-serveren.

---

## API

| Method   | Route                                    | Beskrivelse                         |
|----------|------------------------------------------|-------------------------------------|
| `GET`    | `/api/properties/{propertyId}/documents` | List alle dokumenter for en eiendom |
| `GET`    | `/api/documents/{id}`                    | Hent ett dokument                   |
| `POST`   | `/api/documents`                         | Opprett et dokument                 |
| `PUT`    | `/api/documents/{id}`                    | Oppdater et dokument                |
| `DELETE` | `/api/documents/{id}`                    | Slett et dokument                   |

Statuskoder: `200` (ok), `201` (opprettet, med `Location`-header), `204`
(slettet), `400` (validering), `404` (ikke funnet).

Backend seeder to dokumenter på eiendom
`11111111-1111-1111-1111-111111111111`, slik at UI-et viser data ved første
oppstart.

---

## Antakelser og beslutninger

- **In-memory store som singleton.** Data lever så lenge prosessen kjører. Bak
  `IDocumentStore`-grensesnittet, så en ekte database kan byttes inn uten å røre
  service- eller controller-laget.
- **`PropertyId` og `UploadedAt` er uforanderlige etter opprettelse.** `UploadedAt`
  settes server-side ved opprettelse; `PUT` oppdaterer kun `Title`,
  `DocumentType` og `UploadedBy`. Egen `Create`/`Update`-DTO i stedet for å la
  klienten sende inn hele entiteten.
- **Enums serialiseres som strenger** (`JsonStringEnumConverter`) for et
  selvforklarende API.
- **Bugfiks i `GetDocumentsNewerThan` (startkode):** sammenligningen brukte `>=`,
  som motsier metodenavnet «newer than» og ville tatt med et dokument med
  nøyaktig samme tidsstempel som `since` (relevant ved inkrementell synk). Endret
  til strengt `>`. Iterasjonen ble også tilpasset at lageret er en `Dictionary`
  (`_documents.Values`).

---

## Hva jeg ville gjort videre med mer tid

- **Tester:** enhetstester for `DocumentService` (CRUD + not-found-stier) og
  integrasjonstester mot endpointene med `WebApplicationFactory`.
- **Validering & feilformat:** standardisert `ProblemDetails` for feil.
- **Frontend:** velge/bytte eiendom i UI-et i stedet for fast `propertyId`,
  optimistiske oppdateringer, og litt mer brukervennlig feilhåndtering.
- **Persistens:** bytte in-memory-lageret mot EF Core + en ekte database.
- **Paginering/filtrering** på listing, og bruke `GetDocumentsNewerThan` til et
  inkrementelt synk-endpoint.
```
