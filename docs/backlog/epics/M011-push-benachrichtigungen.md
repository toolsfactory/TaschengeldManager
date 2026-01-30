# Epic M011: Push-Benachrichtigungen

**Status:** Offen (0/14 SP)

## Beschreibung

Push-Benachrichtigungen für wichtige Ereignisse wie neue Geldanfragen, genehmigte/abgelehnte Anfragen, Geschenke und Taschengeld-Gutschriften.

## Business Value

Echtzeit-Kommunikation zwischen Familienmitgliedern. Eltern werden sofort über Anfragen informiert, Kinder über Statusänderungen und Eingänge.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M011-S01 | Firebase Cloud Messaging Setup (Android) | 3 | Offen |
| M011-S02 | Push-Token Registrierung beim Backend | 2 | Offen |
| M011-S03 | Benachrichtigung: Neue Geldanfrage (Eltern) | 1 | Offen |
| M011-S04 | Benachrichtigung: Anfrage genehmigt/abgelehnt (Kind) | 1 | Offen |
| M011-S05 | Benachrichtigung: Neues Geschenk erhalten (Kind) | 1 | Offen |
| M011-S06 | Benachrichtigung: Taschengeld eingegangen (Kind) | 1 | Offen |
| M011-S07 | Benachrichtigungs-Einstellungen (pro Typ ein/aus) | 2 | Offen |
| M011-S08 | In-App Benachrichtigungs-Center | 3 | Offen |

**Gesamt: 14 SP**

## Abhängigkeiten

- M001-M003 (Basis-Setup)
- M002 (für Token-Registrierung nach Login)
- Backend-Unterstützung für Push-Versand

## Akzeptanzkriterien (Epic-Level)

- [ ] FCM ist korrekt konfiguriert für Android
- [ ] Push-Token wird nach Login an Backend gesendet
- [ ] Eltern erhalten Benachrichtigung bei neuer Geldanfrage
- [ ] Kinder erhalten Benachrichtigung bei Anfrage-Entscheidung
- [ ] Kinder werden über Geschenke benachrichtigt
- [ ] Kinder werden über Taschengeld-Gutschrift informiert
- [ ] Benachrichtigungen können pro Typ aktiviert/deaktiviert werden
- [ ] In-App Center zeigt alle Benachrichtigungen

## Benachrichtigungs-Typen

| Typ | Empfänger | Trigger |
|-----|-----------|---------|
| `money_request_new` | Eltern | Kind erstellt Geldanfrage |
| `money_request_approved` | Kind | Eltern genehmigen Anfrage |
| `money_request_rejected` | Kind | Eltern lehnen Anfrage ab |
| `gift_received` | Kind | Verwandter sendet Geschenk |
| `allowance_received` | Kind | Automatische Taschengeld-Zahlung |
| `interest_credited` | Kind | Zinsen gutgeschrieben |

## Technische Details

### Firebase Setup
```csharp
// MauiProgram.cs
builder.Services.AddSingleton<IPushNotificationService, FirebasePushService>();

// AndroidManifest.xml
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

### Token-Registrierung
```csharp
public interface IPushNotificationService
{
    Task<string> GetTokenAsync();
    Task RegisterTokenAsync(string token);
    Task UnregisterTokenAsync();
}
```

## Priorität

**Mittel-Hoch** - Wichtig für Echtzeit-Interaktion

## Story Points

14 SP (0 SP abgeschlossen)
