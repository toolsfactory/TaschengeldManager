# Epic E002: Familien- & Kontoverwaltung

## Beschreibung

Eltern können eine Familie erstellen, weitere Elternteile einladen, Verwandte hinzufügen und Kinder verwalten. Jedes Kind erhält ein virtuelles Taschengeld-Konto mit Kontostand. **Mehrere Eltern können Teil einer Familie sein** (z.B. beide Elternteile, Patchwork-Familien). **Verwandte** (Großeltern, Onkel/Tanten etc.) können Kindern Geld schenken.

## Business Value

Das Familien-Konzept ermöglicht die Zuordnung von Kindern zu mehreren Eltern und die gemeinsame Verwaltung des Taschengeldes. Verwandte können zu besonderen Anlässen (Geburtstag, Weihnachten) Geld überweisen, ohne vollen Einblick in die Konten zu haben.

## Rollen-Übersicht

| Rolle | Rechte |
|-------|--------|
| **Parent** | Voller Zugriff: alle Konten sehen, alle Transaktionen, Kinder/Verwandte verwalten |
| **Relative** | Eingeschränkt: Geld an Kinder überweisen, nur eigene Überweisungen sehen |
| **Child** | Eigenes Konto: Kontostand sehen, Ausgaben erfassen, Geld anfordern |

## Stories

### Familie & Mitglieder
- [x] S010 - Familie erstellen ✅
- [x] S011 - Kind zur Familie hinzufügen ✅
- [x] S012 - Familienmitglieder anzeigen ✅
- [x] S016 - Kind aus Familie entfernen ✅

### Mehrere Eltern
- [x] S017 - Elternteil zur Familie einladen ✅
- [x] S018 - Einladung annehmen ✅
- [x] S019 - Einladung ablehnen/zurückziehen ✅
- [x] S020 - Elternteil aus Familie entfernen ✅

### Verwandte
- [x] S021 - Verwandten einladen (per E-Mail) ✅
- [x] S025 - Verwandten anlegen (durch Eltern) ✅
- [x] S022 - Verwandter: Geld an Kind überweisen ✅
- [x] S023 - Verwandter: Eigene Überweisungen anzeigen ✅
- [x] S024 - Verwandten aus Familie entfernen ✅

### Konten
- [x] S013 - Taschengeld-Konto für Kind anlegen ✅
- [x] S014 - Kontostand anzeigen (Eltern-Sicht) ✅
- [x] S015 - Kontostand anzeigen (Kind-Sicht) ✅

## Abhängigkeiten

- E001 (Benutzerverwaltung muss existieren)

## Akzeptanzkriterien (Epic-Level)

### Familie & Eltern
- [ ] Eine Familie kann **mehrere Eltern** haben
- [ ] Eine Familie kann mehrere Kinder haben
- [ ] Ein Kind gehört zu genau einer Familie
- [ ] Ein Elternteil kann nur zu einer Familie gehören
- [ ] **Alle Eltern** der Familie sehen alle Konten ihrer Kinder
- [ ] **Alle Eltern** können Transaktionen für alle Kinder durchführen
- [ ] Einladungen laufen nach 7 Tagen ab

### Verwandte
- [ ] Eine Familie kann **mehrere Verwandte** haben
- [ ] Verwandte können per E-Mail eingeladen werden ODER von Eltern angelegt werden
- [ ] Verwandte können **nur Geld an Kinder überweisen** (Einzahlung)
- [ ] Verwandte sehen **nur ihre eigenen Überweisungen** (nicht Kontostand, nicht andere Transaktionen)
- [ ] Verwandte können zu **mehreren Familien** gehören (z.B. Oma mit mehreren Enkeln in verschiedenen Familien)
- [ ] Eltern können Verwandte jederzeit entfernen

### Konten
- [ ] Jedes Kind hat genau ein Taschengeld-Konto
- [ ] Kontostand wird in EUR geführt (2 Dezimalstellen)
- [ ] Kinder sehen nur ihr eigenes Konto

## Datenmodell (Entwurf)

```
Family
├── Id
├── Name
├── FamilyCode (für Kind-Login)
├── CreatedAt
├── CreatedByUserId → User
├── Parents[] → User (many-to-many)
├── Relatives[] → User (many-to-many)
└── Children[] → User (one-to-many)

User
├── Id
├── Email (null für Kinder ohne eigenen Login)
├── Role (Parent/Child/Relative)
├── FamilyId → Family (für Kinder)
├── ParentFamilies[] → Family (für Eltern, many-to-many)
└── RelativeFamilies[] → Family (für Verwandte, many-to-many)

FamilyInvitation
├── Id
├── FamilyId → Family
├── InvitedEmail
├── InvitedByUserId → User
├── InvitedRole (Parent/Relative)
├── Status (Pending/Accepted/Rejected/Expired)
├── CreatedAt
└── ExpiresAt

Account
├── Id
├── Balance (decimal)
├── UserId → User (Child)
└── CreatedAt

Transaction (Auszug - Detail in E003)
├── Id
├── AccountId → Account
├── Amount
├── Type (Deposit/Withdrawal/Allowance/Gift)
├── CreatedByUserId → User (Parent/Relative/Child)
└── ...
```

### Beziehungen

```
                    ┌─────────────┐
                    │   Family    │
                    └──────┬──────┘
           ┌───────────────┼───────────────┐
           │               │               │
           ▼               ▼               ▼
    ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
    │  Parents[]  │ │ Relatives[] │ │ Children[]  │
    │  (n:m)      │ │  (n:m)      │ │  (1:n)      │
    └─────────────┘ └─────────────┘ └──────┬──────┘
                                           │
                                           ▼
                                    ┌─────────────┐
                                    │   Account   │
                                    └─────────────┘
```

## Einladungs-Flow

```
Elternteil A erstellt Familie
         │
         ▼
Elternteil A lädt Elternteil B ein (per E-Mail)
         │
         ▼
   [Einladung Pending]
         │
    ┌────┴────┐
    ▼         ▼
Annehmen   Ablehnen
    │         │
    ▼         ▼
B ist Teil  Einladung
der Familie  gelöscht
```

## Priorität

**Hoch** - MVP-Blocker

## Story Points (geschätzt)

34 (Summe aller Stories)

| Bereich | Stories | SP |
|---------|---------|-----|
| Familie & Mitglieder | 4 | 8 |
| Mehrere Eltern | 4 | 10 |
| Verwandte | 5 | 10 |
| Konten | 3 | 6 |
