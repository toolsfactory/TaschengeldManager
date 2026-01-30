# Story M005-S02: Konto-Detail mit Transaktionshistorie

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **die detaillierte Transaktionshistorie eines Kinderkontos einsehen**, damit **ich alle Ein- und Auszahlungen nachvollziehen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein ausgewähltes Kinderkonto, wenn die Detailansicht geöffnet wird, dann sehe ich den aktuellen Kontostand prominent angezeigt
- [ ] Gegeben die Detailansicht, wenn die Transaktionsliste geladen wird, dann werden alle Transaktionen chronologisch sortiert angezeigt (neueste zuerst)
- [ ] Gegeben eine Transaktion, wenn sie angezeigt wird, dann sehe ich Datum, Betrag, Beschreibung und Typ (Einzahlung/Ausgabe)
- [ ] Gegeben viele Transaktionen, wenn ich scrolle, dann werden weitere Transaktionen nachgeladen (Infinite Scroll)
- [ ] Gegeben die Transaktionsliste, wenn ich einen Datumsfilter setze, dann werden nur Transaktionen aus diesem Zeitraum angezeigt
- [ ] Gegeben eine Transaktion, wenn ich darauf tippe, dann sehe ich weitere Details (Kategorie, Notizen, wer die Transaktion erstellt hat)

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück     Emma    ⋮     │
├─────────────────────────────┤
│                             │
│       Kontostand            │
│       45,00 €               │
│                             │
│   [+ Einzahlung] [- Ausgabe]│
│                             │
├─────────────────────────────┤
│  Filter: [Alle ▼] [Januar ▼]│
├─────────────────────────────┤
│                             │
│  15. Jan 2024               │
│  ┌─────────────────────────┐│
│  │ ▲ Taschengeld    +5,00 €││
│  │   Wöchentlich            ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ▼ Süßigkeiten   -2,50 €││
│  │   Ausgabe                ││
│  └─────────────────────────┘│
│                             │
│  14. Jan 2024               │
│  ┌─────────────────────────┐│
│  │ ▲ Oma-Besuch   +10,00 €││
│  │   Einzahlung             ││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ChildAccountDetailPage.xaml`
- **ViewModel**: `ChildAccountDetailViewModel.cs`
- **Models**: `Transaction.cs`, `TransactionFilter.cs`

## API-Endpunkte

```
GET /api/children/{childId}/account
Authorization: Bearer {parent-token}

Response 200:
{
  "accountId": "guid",
  "childId": "guid",
  "childName": "Emma",
  "balance": 45.00,
  "lastUpdated": "2024-01-15T10:30:00Z"
}

GET /api/children/{childId}/transactions?page=1&pageSize=20&from=2024-01-01&to=2024-01-31&type=all
Authorization: Bearer {parent-token}

Response 200:
{
  "transactions": [
    {
      "transactionId": "guid",
      "date": "2024-01-15T10:30:00Z",
      "amount": 5.00,
      "type": "deposit",
      "description": "Taschengeld",
      "category": "recurring",
      "createdBy": "Mama",
      "notes": "Wöchentliches Taschengeld"
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20
}
```

## Technische Notizen

- Transaktionen nach Datum gruppieren für bessere Lesbarkeit
- Farbcodierung: Grün für Einzahlungen, Rot für Ausgaben
- Pagination mit Infinite Scroll implementieren
- Filter-Einstellungen im ViewModel persistieren

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-02-01 | Konto mit Transaktionen | Liste zeigt alle Transaktionen |
| TC-M005-02-02 | Konto ohne Transaktionen | Leerer Zustand |
| TC-M005-02-03 | Scroll bis Ende | Weitere Transaktionen laden |
| TC-M005-02-04 | Datumsfilter setzen | Nur gefilterte Transaktionen |
| TC-M005-02-05 | Transaktion antippen | Detail-Popup erscheint |

## Story Points

3

## Priorität

Hoch

## Status

⬜ Offen
