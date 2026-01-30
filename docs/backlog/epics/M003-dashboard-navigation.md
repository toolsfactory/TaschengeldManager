# Epic M003: Dashboard & Navigation

**Status:** 🔶 Teilweise abgeschlossen (18/23 SP)

## Beschreibung

Rollenbasierte Dashboards und Navigation für Eltern, Kinder und Verwandte. Shell-basierte Navigation mit Bottom TabBar.

## Business Value

Intuitive Bedienung für alle Benutzergruppen. Schneller Zugriff auf wichtige Funktionen.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M003-S01 | AppShell mit rollenbasierter Navigation | 3 | ✅ |
| M003-S02 | Eltern-Dashboard (Familienübersicht) | 5 | ✅ |
| M003-S03 | Kind-Dashboard (eigenes Konto) | 3 | ✅ |
| M003-S04 | Verwandten-Dashboard (Geschenke-Übersicht) | 2 | ✅ |
| M003-S05 | Pull-to-Refresh für alle Listen | 1 | ✅ |
| M003-S06 | Bottom Navigation Bar | 2 | ✅ |
| M003-S07 | Einstellungen-Seite | 2 | ✅ |
| M003-S08 | Onboarding-Screens für neue Benutzer | 3 | ⬜ |
| M003-S09 | Leere Zustände (Empty States) mit Call-to-Action | 2 | ⬜ |

**Gesamt: 23 SP** (18 SP abgeschlossen, 5 SP offen)

## Abhängigkeiten

- M001 (Projekt-Setup)
- M002 (Authentifizierung)

## Akzeptanzkriterien (Epic-Level)

- [x] Navigation wechselt basierend auf Benutzerrolle
- [x] Jede Rolle hat eigenes Dashboard
- [x] Bottom TabBar ist rollenspezifisch
- [x] Pull-to-Refresh funktioniert überall
- [x] Einstellungen sind erreichbar
- [ ] Neue Benutzer sehen Onboarding
- [ ] Leere Listen zeigen hilfreiche Hinweise

## Implementierte Pages

### Parent
- `ParentDashboardPage` - Familienübersicht
- `ParentFamilyPage` - Familienmitglieder
- `ParentPaymentsPage` - Wiederkehrende Zahlungen

### Child
- `ChildDashboardPage` - Kontoübersicht
- `ChildHistoryPage` - Transaktionsverlauf
- `ChildRequestsPage` - Geldanfragen

### Relative
- `RelativeDashboardPage` - Übersicht
- `RelativeGiftsPage` - Geschenke-Historie

### Shared
- `SettingsPage` - Einstellungen (alle Rollen)

## Shell-Struktur

```xml
<Shell>
    <!-- Auth (keine Tabs) -->
    <ShellContent Route="login" />

    <!-- Parent TabBar -->
    <TabBar Route="parent">
        <ShellContent Route="dashboard" Icon="icon_home.svg" Title="Übersicht" />
        <ShellContent Route="family" Icon="icon_family.svg" Title="Familie" />
        <ShellContent Route="payments" Icon="icon_payment.svg" Title="Zahlungen" />
        <ShellContent Route="settings" Icon="icon_settings.svg" Title="Einstellungen" />
    </TabBar>

    <!-- Child TabBar -->
    <TabBar Route="child">
        <ShellContent Route="dashboard" Icon="icon_wallet.svg" Title="Mein Geld" />
        <ShellContent Route="history" Icon="icon_history.svg" Title="Verlauf" />
        <ShellContent Route="requests" Icon="icon_request.svg" Title="Anfragen" />
        <ShellContent Route="settings" Icon="icon_settings.svg" Title="Einstellungen" />
    </TabBar>

    <!-- Relative TabBar -->
    <TabBar Route="relative">
        <ShellContent Route="dashboard" Icon="icon_home.svg" Title="Übersicht" />
        <ShellContent Route="gifts" Icon="icon_gift.svg" Title="Geschenke" />
        <ShellContent Route="settings" Icon="icon_settings.svg" Title="Einstellungen" />
    </TabBar>
</Shell>
```

## Priorität

**Hoch** - Basis für alle Rollen-Features

## Story Points

23 SP (18 SP abgeschlossen, 5 SP offen)
