# Story W002-S07: Dark Mode Unterstützung

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind** möchte ich **das Kassenbuch auch im Dark Mode gut lesen können**, damit **meine Augen geschont werden und die App einheitlich aussieht**.

## Akzeptanzkriterien

- [x] Gegeben der Dark Mode ist aktiviert, wenn das Kassenbuch angezeigt wird, dann sind alle Farben dem Dark Theme angepasst
- [x] Gegeben die Zusammenfassungs-Kacheln, wenn sie im Dark Mode angezeigt werden, dann sind sie gut lesbar mit angepassten Hintergrundfarben
- [x] Gegeben die Tabelle im Dark Mode, wenn sie angezeigt wird, dann haben Zebrastreifen einen dunklen Kontrast
- [x] Gegeben positive/negative Werte, wenn sie im Dark Mode angezeigt werden, dann sind Grün/Rot-Töne für dunkle Hintergründe optimiert
- [x] Gegeben die Mobile Card-Ansicht, wenn sie im Dark Mode angezeigt wird, dann sind alle Elemente gut lesbar

## Technische Hinweise

### Dark Mode Farben

Die Kassenbuch-Komponenten nutzen Tailwind CSS Dark Mode Klassen:

```tsx
// Beispiel: Zusammenfassungs-Kachel
<div className="bg-green-50 dark:bg-green-900/30">
  <p className="text-green-600 dark:text-green-400">
    +{totalIncome.toFixed(2)} €
  </p>
</div>
```

### Farbschema für Dark Mode

| Element | Light Mode | Dark Mode |
|---------|------------|-----------|
| Hintergrund Kachel | `bg-gray-50` | `dark:bg-gray-700` |
| Einnahmen BG | `bg-green-50` | `dark:bg-green-900/30` |
| Einnahmen Text | `text-green-600` | `dark:text-green-400` |
| Ausgaben BG | `bg-red-50` | `dark:bg-red-900/30` |
| Ausgaben Text | `text-red-600` | `dark:text-red-400` |
| Saldo positiv BG | `bg-blue-50` | `dark:bg-blue-900/30` |
| Saldo positiv Text | `text-blue-600` | `dark:text-blue-400` |
| Tabellen-Zebrastreifen | `bg-gray-50` | `dark:bg-gray-800/50` |
| Tabellen-Hover | `hover:bg-gray-100` | `dark:hover:bg-gray-700` |
| Border | `border-gray-200` | `dark:border-gray-700` |

### Komponenten-Checkliste

Folgende Komponenten müssen Dark Mode Klassen haben:

1. **CashbookHeader.tsx**
   - Titel und Monat/Jahr
   - Vier Zusammenfassungs-Kacheln
   - Saldo-Differenz-Nachricht

2. **CashbookTable.tsx**
   - Tabellen-Header
   - Zebrastreifen
   - Hover-Effekte
   - Summenzeile

3. **CashbookRow.tsx**
   - Datum-Spalte
   - Beschreibung
   - Einnahmen (grün)
   - Ausgaben (rot)
   - Saldo

4. **MonthSelector.tsx**
   - Navigation-Buttons
   - Dropdown
   - Monatsanzeige

5. **CashbookMobileCard.tsx**
   - Card-Container
   - Alle Text-Elemente
   - Betrags-Anzeigen

### Konsistenz mit W001 Dark Mode

Die Implementierung muss konsistent sein mit den in W001 definierten Dark Mode Tokens:

```css
/* Aus W001 - CSS Variablen */
:root {
  --color-background: theme('colors.gray.50');
  --color-card: theme('colors.white');
  --color-text-primary: theme('colors.gray.900');
  --color-text-secondary: theme('colors.gray.600');
}

.dark {
  --color-background: theme('colors.gray.900');
  --color-card: theme('colors.gray.800');
  --color-text-primary: theme('colors.gray.100');
  --color-text-secondary: theme('colors.gray.400');
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-60 | Kassenbuch im Light Mode | Alle Farben hell und lesbar |
| TC-W002-61 | Kassenbuch im Dark Mode | Alle Farben dunkel und lesbar |
| TC-W002-62 | Dark Mode Toggle während Nutzung | Farben wechseln sofort |
| TC-W002-63 | Einnahmen im Dark Mode | Grün-Ton gut sichtbar auf dunklem Hintergrund |
| TC-W002-64 | Ausgaben im Dark Mode | Rot-Ton gut sichtbar auf dunklem Hintergrund |
| TC-W002-65 | Zebrastreifen im Dark Mode | Kontrast zwischen Zeilen erkennbar |
| TC-W002-66 | Mobile Cards im Dark Mode | Alle Elemente lesbar |

## Abhängigkeiten

- W001 (Dark Mode Toggle) muss implementiert sein
- W002-S01 bis W002-S06 sollten implementiert sein

## Story Points

2

## Priorität

Niedrig
