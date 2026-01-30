# Epic M012: Profil & Account-Verwaltung

**Status:** Offen (0/16 SP)

## Beschreibung

Benutzer können ihr eigenes Profil und Account-Einstellungen verwalten: Profildaten ändern, Passwort aktualisieren, MFA konfigurieren und Account löschen.

## Business Value

Selbstverwaltung von Account-Einstellungen reduziert Support-Aufwand. DSGVO-Konformität durch Datenexport und Account-Löschung.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M012-S01 | Eigenes Profil anzeigen | 1 | Offen |
| M012-S02 | Profil bearbeiten (Name, Avatar) | 2 | Offen |
| M012-S03 | Passwort ändern | 2 | Offen |
| M012-S04 | Email ändern (mit Verifizierung) | 3 | Offen |
| M012-S05 | MFA aktivieren/deaktivieren | 2 | Offen |
| M012-S06 | Account löschen (mit Bestätigung) | 2 | Offen |
| M012-S07 | Datenschutz-Einstellungen | 1 | Offen |
| M012-S08 | DSGVO-Datenexport | 3 | Offen |

**Gesamt: 16 SP**

## Abhängigkeiten

- M001-M003 (Basis-Setup)
- M002 (Authentifizierung)

## Akzeptanzkriterien (Epic-Level)

- [ ] Benutzer kann eigenes Profil anzeigen
- [ ] Name und Avatar können geändert werden
- [ ] Passwort kann geändert werden (mit altem Passwort)
- [ ] Email kann geändert werden (mit Verifizierung)
- [ ] MFA kann aktiviert/deaktiviert werden
- [ ] Account kann gelöscht werden (mit Bestätigung)
- [ ] Datenschutz-Einstellungen sind zugänglich
- [ ] Alle eigenen Daten können exportiert werden

## Geplante Pages

- `ProfilePage` - Profilübersicht
- `EditProfilePage` - Profil bearbeiten
- `ChangePasswordPage` - Passwort ändern
- `ChangeEmailPage` - Email ändern
- `MfaSettingsPage` - MFA-Einstellungen
- `PrivacySettingsPage` - Datenschutz
- `DeleteAccountPage` - Account löschen

## UI-Entwurf

### Profil-Seite
```
┌─────────────────────────────────┐
│  ← Zurück        Profil         │
├─────────────────────────────────┤
│                                 │
│         [ 👤 Avatar ]           │
│         Max Mustermann          │
│         max@example.com         │
│                                 │
│  ┌─────────────────────────────┐│
│  │ 📝 Profil bearbeiten    >  ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 🔒 Passwort ändern      >  ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 📧 Email ändern         >  ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 🔐 Zwei-Faktor-Auth     >  ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 🔓 Datenschutz          >  ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 📤 Daten exportieren    >  ││
│  └─────────────────────────────┘│
│                                 │
│  ┌─────────────────────────────┐│
│  │ ❌ Account löschen      >  ││
│  └─────────────────────────────┘│
│                                 │
└─────────────────────────────────┘
```

## Account-Löschung

Die Account-Löschung erfordert:
1. Bestätigung per Dialog
2. Passwort-Eingabe zur Verifizierung
3. Hinweis auf unwiderrufliche Löschung
4. 30-Tage Karenzzeit (optional reaktivierbar)

## DSGVO-Datenexport

Export enthält:
- Alle persönlichen Daten
- Transaktionshistorie
- Anfragen-Historie
- Account-Einstellungen

Format: JSON oder PDF

## Priorität

**Mittel** - Wichtig für DSGVO und Benutzerzufriedenheit

## Story Points

16 SP (0 SP abgeschlossen)
