# Epic E005: Anfragen-System (Kinder → Eltern)

## Beschreibung

Kinder können Geldanfragen an ihre Eltern stellen (z.B. für größere Anschaffungen oder Vorschüsse). Eltern können diese Anfragen genehmigen oder ablehnen.

## Business Value

Fördert Kommunikation zwischen Eltern und Kindern über Geld. Lehrt Kinder, Wünsche zu artikulieren und auf Genehmigung zu warten. Gibt Eltern Kontrolle über außerplanmäßige Ausgaben.

## Stories

- [x] S040 - Geldanfrage erstellen (Kind) ✅
- [x] S041 - Anfrage mit Begründung versehen ✅
- [x] S042 - Anfragen-Liste anzeigen (Kind - eigene) ✅
- [x] S043 - Anfragen-Liste anzeigen (Eltern - alle Kinder) ✅
- [x] S044 - Anfrage genehmigen (Eltern) ✅
- [x] S045 - Anfrage ablehnen mit Begründung (Eltern) ✅
- [x] S046 - Anfrage zurückziehen (Kind) ✅
- [ ] S047 - Benachrichtigung bei neuer Anfrage (Eltern) 🔜 E007
- [ ] S048 - Benachrichtigung bei Entscheidung (Kind) 🔜 E007

## Abhängigkeiten

- E001 (Benutzerverwaltung)
- E002 (Kontoverwaltung)
- E003 (Transaktionen - für Genehmigung)

## Akzeptanzkriterien (Epic-Level)

- [x] Kinder können Anfragen mit Betrag und Begründung erstellen ✅
- [x] Eltern sehen alle offenen Anfragen ihrer Kinder ✅
- [x] Bei Genehmigung wird automatisch eine Einzahlung gebucht ✅
- [x] Abgelehnte Anfragen zeigen den Ablehnungsgrund ✅
- [x] Anfragen haben Status: Pending, Approved, Rejected, Withdrawn ✅
- [x] Kinder können nur offene Anfragen zurückziehen ✅

## Datenmodell (Entwurf)

```
MoneyRequest
├── Id
├── ChildUserId → User
├── Amount (decimal)
├── Reason (string)
├── Status (Pending/Approved/Rejected/Withdrawn)
├── ResponseNote (string, optional)
├── RespondedByUserId → User (optional)
├── RespondedAt (DateTime, optional)
├── CreatedAt
└── ResultingTransactionId → Transaction (optional)
```

## User Flow

```
Kind erstellt Anfrage
        │
        ▼
   [Pending] ──────────────────┐
        │                      │
        ▼                      ▼
Eltern genehmigen        Eltern lehnen ab
        │                      │
        ▼                      ▼
  [Approved]              [Rejected]
        │
        ▼
Automatische Einzahlung
auf Kind-Konto
```

## Priorität

**Hoch** - Core Feature

## Story Points (geschätzt)

21 (Summe aller Stories)
