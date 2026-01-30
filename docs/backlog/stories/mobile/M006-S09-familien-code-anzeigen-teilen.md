# Story M006-S09: Familien-Code anzeigen/teilen

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **den Familien-Code anzeigen und teilen können**, damit **meine Kinder sich damit in der App anmelden können**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienübersicht, wenn ich auf den Familien-Code tippe, dann wird er groß angezeigt
- [ ] Gegeben die Code-Anzeige, wenn ich auf "Kopieren" tippe, dann wird der Code in die Zwischenablage kopiert
- [ ] Gegeben die Code-Anzeige, wenn ich auf "Teilen" tippe, dann öffnet sich das System-Share-Sheet
- [ ] Gegeben die Code-Anzeige, wenn sie geöffnet wird, dann wird auch eine kurze Anleitung für Kinder angezeigt
- [ ] Gegeben der Familien-Code, wenn er lange nicht geändert wurde, dann kann ich optional einen neuen generieren

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Familien-Code            │
├─────────────────────────────┤
│                             │
│  Familien-Code für          │
│  Familie Müller             │
│                             │
│  ┌─────────────────────────┐│
│  │                         ││
│  │       ABC123            ││
│  │                         ││
│  └─────────────────────────┘│
│                             │
│  [📋 Kopieren] [📤 Teilen]  │
│                             │
├─────────────────────────────┤
│                             │
│  📱 So loggen sich Kinder   │
│  ein:                       │
│                             │
│  1. App öffnen              │
│  2. "Ich bin ein Kind"      │
│     auswählen               │
│  3. Familien-Code eingeben  │
│  4. Spitzname eingeben      │
│  5. PIN eingeben            │
│                             │
├─────────────────────────────┤
│                             │
│  ⚠️ Code geheim halten!     │
│  Nur mit Familienmitgliedern│
│  teilen.                    │
│                             │
│  [🔄 Neuen Code generieren] │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `FamilyCodePage.xaml` (als Modal/BottomSheet)
- **ViewModel**: `FamilyCodeViewModel.cs`
- **Service**: `IFamilyService.cs`, `IShareService.cs`

## API-Endpunkte

```
GET /api/families/{familyId}/code
Authorization: Bearer {parent-token}

Response 200:
{
  "familyId": "guid",
  "familyCode": "ABC123",
  "createdAt": "2024-01-15T10:00:00Z"
}

POST /api/families/{familyId}/regenerate-code
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "parentPassword": "string"
}

Response 200:
{
  "newCode": "XYZ789",
  "oldCodeValidUntil": "2024-01-22T10:00:00Z"
}
```

## Share-Text Template

```
Familien-Code für TaschengeldManager:

ABC123

So loggst du dich ein:
1. Öffne die TaschengeldManager App
2. Wähle "Ich bin ein Kind"
3. Gib diesen Code ein: ABC123
4. Gib deinen Spitznamen ein
5. Gib deine PIN ein

Viel Spaß! 🎉
```

## Technische Notizen

- Code groß und gut lesbar darstellen (monospace Font)
- Clipboard API für Kopieren
- Native Share-Sheet für Teilen (Text + optional Bild)
- Code-Regenerierung mit Passwortbestätigung
- Alter Code nach Regenerierung noch 7 Tage gültig (Übergangszeit)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-09-01 | Code anzeigen | Code wird groß dargestellt |
| TC-M006-09-02 | Code kopieren | In Zwischenablage |
| TC-M006-09-03 | Code teilen | Share-Sheet öffnet sich |
| TC-M006-09-04 | Code regenerieren | Neuer Code wird generiert |
| TC-M006-09-05 | Alter Code nach Regenerierung | Noch 7 Tage gültig |

## Story Points

1

## Priorität

Hoch

## Status

⬜ Offen
