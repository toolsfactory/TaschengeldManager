# Epic E004: Automatische Taschengeld-Zahlungen

## Beschreibung

Eltern können wiederkehrende Taschengeld-Zahlungen einrichten, die automatisch zu definierten Zeitpunkten auf die Konten der Kinder gebucht werden.

## Business Value

Kernfeature laut Anforderung. Reduziert manuellen Aufwand für Eltern und stellt sicher, dass Kinder regelmäßig ihr Taschengeld erhalten. Lehrt Kinder das Konzept von regelmäßigem Einkommen.

## Stories

- [x] S030 - Wiederkehrende Zahlung einrichten ✅
- [x] S031 - Zahlungsintervall wählen (wöchentlich/monatlich) ✅
- [x] S032 - Zahlungstag festlegen ✅
- [x] S033 - Wiederkehrende Zahlung bearbeiten ✅
- [x] S034 - Wiederkehrende Zahlung pausieren ✅
- [x] S035 - Wiederkehrende Zahlung löschen ✅
- [x] S036 - Übersicht aller aktiven Zahlungen ✅
- [x] S037 - Automatische Ausführung der Zahlungen (Backend-Job) ✅

## Abhängigkeiten

- E001 (Benutzerverwaltung)
- E002 (Kontoverwaltung)
- E003 (Transaktionen)

## Akzeptanzkriterien (Epic-Level)

- [x] Eltern können pro Kind eine oder mehrere wiederkehrende Zahlungen einrichten ✅
- [x] Unterstützte Intervalle: wöchentlich, 14-tägig, monatlich ✅
- [x] Zahlungen werden automatisch zum konfigurierten Zeitpunkt ausgeführt ✅
- [x] Bei Ausführung wird eine Transaktion vom Typ `Allowance` erstellt ✅
- [ ] Eltern erhalten optional eine Benachrichtigung bei Ausführung 🔜 E007
- [ ] Kinder erhalten optional eine Benachrichtigung bei Eingang 🔜 E007

## Datenmodell (Entwurf)

```
RecurringPayment
├── Id
├── ChildAccountId → Account
├── Amount (decimal)
├── Interval (Weekly/Biweekly/Monthly)
├── DayOfWeek (für wöchentlich)
├── DayOfMonth (für monatlich)
├── NextExecutionDate
├── IsActive (bool)
├── CreatedByUserId → User
├── CreatedAt
└── UpdatedAt
```

## Technische Notizen

- Backend-Job (Hangfire oder ähnlich) für automatische Ausführung
- Job sollte täglich laufen und fällige Zahlungen verarbeiten
- Idempotenz sicherstellen (keine doppelten Zahlungen)

## Priorität

**Hoch** - Core Feature

## Story Points (geschätzt)

21 (Summe aller Stories)
