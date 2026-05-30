# Boligmappa — Teknisk vurderingsoppgave

**Estimert tid:** 1 time  
**Stack:** C# / .NET · Angular eller React

> **Vennligst ikke bruk mer enn 1 time på denne oppgaven.**
> Vi er ikke ute etter et ferdig produkt. Det vi ønsker å se på er tilnærmingen din og beslutningene du tar, avveiingene du vurderer, og hvordan du resonnerer deg gjennom et problem. En velstrukturert, delvis ferdig løsning forteller oss langt mer enn en komplett, men forhastet løsning.
> Når du når tidsgrensen, stopp og noter hva du ville gjort videre.

---

## Bakgrunn

Boligmappa er en digital plattform der boligeiere og fagpersoner lagrer og forvalter dokumentasjon knyttet til arbeid på eiendom — byggetillatelser, kvalitetssikringer, garantier, fakturaer og mer.

---

## Oppgaven

Bygg en enkel full-stack CRUD-applikasjon for å forvalte eiendomsdokumenter.

---

### Domenemodell

```
Document
  Id            : Guid
  PropertyId    : Guid
  Title         : string
  DocumentType  : enum (BuildingPermit | QualityAssurance | Warranty | Invoice | Other)
  UploadedAt    : DateTimeOffset
  UploadedBy    : string
```

---

### Backend — C# / .NET Web API

Implementer følgende endpoints:

| Method   | Route                                    | Beskrivelse                          |
|----------|------------------------------------------|--------------------------------------|
| `GET`    | `/api/properties/{propertyId}/documents` | List alle dokumenter for en eiendom  |
| `GET`    | `/api/documents/{id}`                    | Hent ett enkelt dokument             |
| `POST`   | `/api/documents`                         | Opprett et nytt dokument             |
| `PUT`    | `/api/documents/{id}`                    | Oppdater et eksisterende dokument    |
| `DELETE` | `/api/documents/{id}`                    | Slett et dokument                    |

- Bruk .NET 6 eller nyere
- En in-memory store er tilstrekkelig — ingen ekte database kreves
- Returner passende HTTP-statuskoder

### Startkode

Følgende hjelpemetode er allerede koblet inn i in-memory store — bruk den der det er hensiktsmessig i service-laget ditt, og rett opp eventuelle bugs du finner før du leverer:

```csharp
public async Task<IEnumerable<Document>> GetDocumentsNewerThan(Guid propertyId, DateTimeOffset? since)
{
    if (since == null)
        return Enumerable.Empty<Document>();

    return await Task.FromResult(
        _documents
            .Where(d => d.PropertyId == propertyId && d.UploadedAt >= since)
            .ToList()
    );
}
```

---

### Frontend — Angular eller React (ditt valg)

Bygg et enkelt UI som:

1. Lister alle dokumenter for en eiendom
2. Lar deg opprette et nytt dokument via et skjema
3. Lar deg redigere et eksisterende dokument
4. Lar deg slette et dokument

---

## Innlevering

1. **Fork** dette repositoriet til din egen GitHub-konto.
2. **Klon** din fork lokalt og opprett en ny branch (f.eks. `feature/solution`).
3. Implementer løsningen din på den branchen.
4. **Commit** jevnlig med meningsfulle commit-meldinger — vi ønsker å se hvordan du jobber steg for steg.
5. **Push** branchen din til **din egen fork** og åpne en Pull Request mot **din egen forks `main`-branch** — åpne **ikke** en PR mot det originale repositoriet.
6. Del lenken til din fork (eller PR-en på din fork) med oss.

Inkluder en kort `README` med steg for å kjøre prosjektet, eventuelle antakelser du gjorde, og hva du ville gjort videre hvis du hadde mer tid.
