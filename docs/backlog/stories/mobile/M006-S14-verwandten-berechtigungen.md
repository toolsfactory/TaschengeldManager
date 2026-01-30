# Story M006-S14: Verwandten-Berechtigungen

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **die Berechtigungen von Verwandten verwalten können**, damit **ich kontrollieren kann, was Großeltern oder andere Verwandte tun dürfen**.

## Akzeptanzkriterien

- [ ] Gegeben das Profil eines Verwandten, wenn ich auf "Berechtigungen" tippe, dann sehe ich alle verfügbaren Optionen
- [ ] Gegeben die Berechtigungseinstellungen, wenn ich eine Option ändere, dann wird sie sofort gespeichert
- [ ] Gegeben die Berechtigungen, wenn ich "Kontostände sehen" deaktiviere, dann sieht der Verwandte keine Beträge mehr
- [ ] Gegeben die Berechtigungen, wenn ich "Einzahlungen" aktiviere, dann kann der Verwandte Geld einzahlen
- [ ] Gegeben die Berechtigungen, wenn ich sie pro Kind einstelle, dann gelten sie nur für dieses Kind

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück   Berechtigungen  │
├─────────────────────────────┤
│                             │
│  Berechtigungen für         │
│  Oma Helga                  │
│                             │
│  Allgemeine Berechtigungen: │
│  ┌─────────────────────────┐│
│  │ Kontostände sehen       ││
│  │ [=========○=====] An    ││
│  │                         ││
│  │ Einzahlungen tätigen    ││
│  │ [=========○=====] An    ││
│  │                         ││
│  │ Ausgaben erfassen       ││
│  │ [○================] Aus ││
│  │                         ││
│  │ Transaktionen sehen     ││
│  │ [○================] Aus ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  Zugriff auf Kinder:        │
│  ┌─────────────────────────┐│
│  │ 👧 Emma                 ││
│  │ [=========○=====] ✓     ││
│  │                         ││
│  │ 👦 Max Jr.              ││
│  │ [=========○=====] ✓     ││
│  └─────────────────────────┘│
│                             │
│  ℹ️ Berechtigungen werden   │
│  sofort wirksam.            │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `MemberPermissionsPage.xaml`
- **ViewModel**: `MemberPermissionsViewModel.cs`
- **Model**: `MemberPermissions.cs`

## API-Endpunkte

```
GET /api/families/{familyId}/members/{memberId}/permissions
Authorization: Bearer {parent-token}

Response 200:
{
  "memberId": "guid",
  "memberName": "Oma Helga",
  "role": "relative",
  "permissions": {
    "canViewBalance": true,
    "canDeposit": true,
    "canRecordExpense": false,
    "canViewTransactions": false
  },
  "childAccess": [
    {
      "childId": "guid",
      "childName": "Emma",
      "hasAccess": true
    },
    {
      "childId": "guid",
      "childName": "Max Jr.",
      "hasAccess": true
    }
  ]
}

PUT /api/families/{familyId}/members/{memberId}/permissions
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "permissions": {
    "canViewBalance": true,
    "canDeposit": true,
    "canRecordExpense": true,
    "canViewTransactions": false
  },
  "childAccess": [
    {"childId": "guid", "hasAccess": true},
    {"childId": "guid", "hasAccess": false}
  ]
}

Response 200:
{
  "message": "Berechtigungen aktualisiert"
}
```

## Verfügbare Berechtigungen

| Berechtigung | Beschreibung |
|--------------|--------------|
| canViewBalance | Kann Kontostände der zugewiesenen Kinder sehen |
| canDeposit | Kann Geld auf Kinderkonten einzahlen |
| canRecordExpense | Kann Ausgaben für Kinder erfassen |
| canViewTransactions | Kann Transaktionshistorie einsehen |

## Technische Notizen

- Berechtigungen werden sofort nach Toggle gespeichert (Auto-Save)
- Granulare Kontrolle pro Kind möglich
- Bei Deaktivierung: UI-Elemente werden beim Verwandten ausgeblendet
- API-Calls werden ebenfalls gegen Berechtigungen geprüft
- Audit-Log für Berechtigungsänderungen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-14-01 | Berechtigung aktivieren | Sofort wirksam |
| TC-M006-14-02 | Berechtigung deaktivieren | Zugriff entfernt |
| TC-M006-14-03 | Kind-Zugriff entfernen | Kein Zugriff mehr |
| TC-M006-14-04 | Verwandter ohne Rechte | Sieht nichts |
| TC-M006-14-05 | API ohne Berechtigung | Fehler 403 |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
