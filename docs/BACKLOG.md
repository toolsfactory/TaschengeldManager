# Backlog

Priorisierte Liste von Verbesserungen und technischen Schulden.

---

## Niedrige Priorität

### SEC-001: Test Credentials Endpoint absichern
**Beschreibung:** Der `/api/dev/credentials` Endpoint ist aktuell in Development und Testing verfügbar. Dieser sollte nur in der Development-Umgebung aktiviert sein.

**Problem:**
- Der Endpoint gibt Test-Passwörter im Klartext zurück
- Könnte bei falscher Deployment-Konfiguration in Produktion landen

**Lösung:**
- Endpoint nur in `IsDevelopment()` aktivieren (nicht in Testing)
- Alternativ: Endpoint komplett entfernen und Test-Credentials nur in Dokumentation führen

**Aufwand:** Gering (1-2 Stunden)

**Dateien:**
- `src/TaschengeldManager.Api/Endpoints/DevEndpoints.cs`
- `src/TaschengeldManager.Api/Program.cs`

---

### SEC-002: Vorhersagbare Test-Daten randomisieren
**Beschreibung:** Die DevSeeder-Daten verwenden vorhersagbare Passwörter wie `Parent123!` und `Child123!`.

**Problem:**
- Wenn Seed-Daten versehentlich in Produktion gelangen, sind die Passwörter bekannt
- Automatisierte Angriffe könnten diese Standard-Passwörter ausprobieren

**Lösung:**
- Option A: Randomisierte Passwörter generieren und beim Seeding in Console ausgeben
- Option B: Environment-Variable für Seed-Passwörter verwenden
- Option C: Seed-Funktion nur in Development aktivieren (nicht in Testing)

**Aufwand:** Gering (1-2 Stunden)

**Dateien:**
- `src/TaschengeldManager.Infrastructure/Services/DevSeederService.cs`

---

## Legende

| Priorität | Beschreibung |
|-----------|--------------|
| Kritisch | Sofort beheben - Sicherheitsrisiko oder Breaking |
| Hoch | Zeitnah beheben - Wichtig für Stabilität |
| Mittel | Geplant umsetzen - Verbesserung |
| Niedrig | Nice-to-have - Bei Gelegenheit |

---

*Erstellt: 2026-01-19*
