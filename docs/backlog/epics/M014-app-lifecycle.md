# Epic M014: App-Lifecycle & Qualität

**Status:** Offen (0/13 SP)

## Beschreibung

App-Lifecycle-Management: Version-Check, Deep-Links, Crash-Reporting, Analytics und Store-Vorbereitung.

## Business Value

Professionelle App-Qualität durch Crash-Reporting und Analytics. Deep-Links ermöglichen nahtlose Einladungs-Flows. Store-Listing ist Voraussetzung für Veröffentlichung.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M014-S01 | App-Version prüfen / Force-Update Dialog | 2 | Offen |
| M014-S02 | Deep-Link Handling (Einladungen, etc.) | 3 | Offen |
| M014-S03 | App-Rating Prompt (nach X Nutzungen) | 1 | Offen |
| M014-S04 | Crash-Reporting Integration (Sentry/AppCenter) | 2 | Offen |
| M014-S05 | Analytics Integration (anonymisiert) | 2 | Offen |
| M014-S06 | App-Icon und Splash-Screen | 1 | Offen |
| M014-S07 | Store-Listing vorbereiten (Screenshots, Beschreibung) | 2 | Offen |

**Gesamt: 13 SP**

## Abhängigkeiten

- Alle anderen Epics sollten weitgehend abgeschlossen sein
- M006 (für Deep-Link Einladungen)

## Akzeptanzkriterien (Epic-Level)

- [ ] App prüft bei Start die Mindestversion
- [ ] Force-Update Dialog bei veralteter Version
- [ ] Deep-Links werden korrekt verarbeitet
- [ ] App-Rating wird nach X Nutzungen angefragt
- [ ] Crashes werden automatisch an Sentry/AppCenter gesendet
- [ ] Anonymisierte Analytics sind aktiv
- [ ] App-Icon und Splash-Screen sind implementiert
- [ ] Store-Listing ist vorbereitet

## Deep-Link Schema

```
taschengeld://invite/{token}        → Einladung annehmen
taschengeld://request/{requestId}   → Geldanfrage anzeigen
taschengeld://transaction/{txId}    → Transaktion anzeigen
```

### Deep-Link Handler
```csharp
// AppShell.xaml.cs
protected override void OnAppLinkRequestReceived(Uri uri)
{
    var path = uri.Host;
    var param = uri.Segments.LastOrDefault();

    switch (path)
    {
        case "invite":
            await Shell.Current.GoToAsync($"invite?token={param}");
            break;
        case "request":
            await Shell.Current.GoToAsync($"request?id={param}");
            break;
    }
}
```

## Version-Check

### Backend-Endpoint
```json
GET /api/app/version
{
    "minVersion": "1.0.0",
    "currentVersion": "1.2.0",
    "forceUpdate": false,
    "updateUrl": "market://details?id=com.taschengeld"
}
```

### Update-Dialog
```
┌─────────────────────────────────┐
│                                 │
│         🔄                      │
│                                 │
│   Update erforderlich           │
│                                 │
│   Eine neue Version ist         │
│   verfügbar. Bitte aktualisiere │
│   die App, um fortzufahren.     │
│                                 │
│  ┌─────────────────────────────┐│
│  │      Jetzt aktualisieren     ││
│  └─────────────────────────────┘│
│                                 │
└─────────────────────────────────┘
```

## Crash-Reporting (Sentry)

### Setup
```csharp
// MauiProgram.cs
builder.UseSentry(options =>
{
    options.Dsn = "https://xxx@sentry.io/xxx";
    options.Debug = false;
    options.AutoSessionTracking = true;
});
```

### Zusätzliche Kontextdaten
- User-ID (anonymisiert)
- App-Version
- Gerätemodell
- Android-Version
- Benutzerrolle (Parent/Child/Relative)

## Analytics (Anonymisiert)

### Events
| Event | Beschreibung |
|-------|--------------|
| `app_open` | App gestartet |
| `login_success` | Erfolgreiche Anmeldung |
| `expense_created` | Ausgabe erfasst |
| `request_submitted` | Geldanfrage gestellt |
| `gift_sent` | Geschenk gesendet |

### Keine personenbezogenen Daten
- Keine Namen
- Keine Beträge
- Keine Transaktionsdetails

## App-Rating Prompt

- Nach 7 Tagen aktiver Nutzung
- Nach mindestens 5 Transaktionen
- Maximal einmal pro Monat anzeigen
- "Später erinnern" Option

## Store-Listing

### Screenshots (min. 4)
1. Kind-Dashboard mit Kontostand
2. Transaktionsliste
3. Ausgabe erfassen
4. Eltern-Dashboard

### Beschreibung (Deutsch)
```
TaschengeldManager - Die Familien-App für Taschengeld

Verwalte das Taschengeld deiner Kinder einfach und übersichtlich:

✅ Kontostand immer im Blick
✅ Ausgaben erfassen
✅ Automatisches Taschengeld
✅ Geldanfragen stellen und genehmigen
✅ Zinsen als Sparanreiz
✅ Geschenke von Verwandten

Sicher und kindgerecht - mit PIN-Login für Kinder
und Fingerprint-Authentifizierung für Eltern.
```

### Kategorie
Familie / Finanzen

### Altersfreigabe
USK 0 / PEGI 3 (familienfreundlich)

## Priorität

**Mittel** - Wichtig vor Store-Release

## Story Points

13 SP (0 SP abgeschlossen)
