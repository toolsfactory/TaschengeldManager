# Epic M013: Error Handling & User Feedback

**Status:** Offen (0/12 SP)

## Beschreibung

Konsistente Fehlerbehandlung und Benutzer-Feedback durch Toast-Nachrichten, Validierung, Loading-States und Skeleton-Loader.

## Business Value

Professionelle Fehlerbehandlung verbessert die User Experience und reduziert Frustration. Klare Feedback-Mechanismen helfen Benutzern, den Zustand der App zu verstehen.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M013-S01 | Globale Exception-Behandlung | 2 | Offen |
| M013-S02 | Toast/Snackbar Service für Feedback | 2 | Offen |
| M013-S03 | Retry-Mechanismus bei API-Fehlern | 2 | Offen |
| M013-S04 | Validierungs-Feedback in Formularen | 2 | Offen |
| M013-S05 | Fehlermeldungen lokalisieren (Deutsch) | 1 | Offen |
| M013-S06 | Loading-States für alle Aktionen | 1 | Offen |
| M013-S07 | Skeleton-Loader für Listen | 2 | Offen |

**Gesamt: 12 SP**

## Abhängigkeiten

- M001 (Projekt-Setup)
- Parallel zu allen anderen Epics implementierbar

## Akzeptanzkriterien (Epic-Level)

- [ ] Unbehandelte Exceptions werden global abgefangen
- [ ] Toast/Snackbar zeigt Erfolg/Fehler-Meldungen
- [ ] Bei API-Fehlern wird automatisch Retry versucht
- [ ] Formular-Felder zeigen Validierungsfehler inline
- [ ] Alle Fehlermeldungen sind auf Deutsch
- [ ] Loading-Spinner während API-Aufrufen
- [ ] Skeleton-Loader für Listen während des Ladens

## Toast-Typen

| Typ | Icon | Farbe | Dauer |
|-----|------|-------|-------|
| Success | ✅ | Grün | 3s |
| Error | ❌ | Rot | 5s |
| Warning | ⚠️ | Orange | 4s |
| Info | ℹ️ | Blau | 3s |

## Implementierung

### Toast Service
```csharp
public interface IToastService
{
    Task ShowSuccessAsync(string message);
    Task ShowErrorAsync(string message);
    Task ShowWarningAsync(string message);
    Task ShowInfoAsync(string message);
}
```

### Globale Exception Handler
```csharp
// MauiProgram.cs
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    var exception = e.ExceptionObject as Exception;
    // Log to Sentry/AppCenter
    // Show user-friendly message
};
```

### Retry-Policy (Polly)
```csharp
services.AddHttpClient<IApiClient>()
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## UI-Entwurf

### Toast/Snackbar
```
┌─────────────────────────────────┐
│                                 │
│  ... App Content ...            │
│                                 │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │ ✅ Ausgabe erfolgreich      │ │
│ │    gespeichert              │ │
│ └─────────────────────────────┘ │
└─────────────────────────────────┘
```

### Skeleton-Loader
```
┌─────────────────────────────────┐
│  Transaktionen                  │
│  ┌─────────────────────────────┐│
│  │ ▓▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓▓▓ ││
│  │ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓           ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ ▓▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓▓▓ ││
│  │ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓           ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ ▓▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓▓▓ ││
│  │ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓           ││
│  └─────────────────────────────┘│
└─────────────────────────────────┘
```

## Fehlermeldungen (Deutsch)

| Code | Meldung |
|------|---------|
| `network_error` | Keine Internetverbindung |
| `server_error` | Server nicht erreichbar |
| `auth_error` | Anmeldung fehlgeschlagen |
| `validation_error` | Bitte überprüfe deine Eingaben |
| `not_found` | Daten nicht gefunden |
| `permission_denied` | Keine Berechtigung |

## Priorität

**Hoch** - Grundlage für gute UX

## Story Points

12 SP (0 SP abgeschlossen)
