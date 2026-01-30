# Story M014-S07: Store-Listing vorbereiten

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Produktmanager** moechte ich **das Store-Listing fuer Google Play vorbereiten**, damit **die App veroeffentlicht werden kann und Benutzer sie finden**.

## Akzeptanzkriterien

- [ ] Gegeben ein Store-Listing, wenn es erstellt wird, dann enthaelt es eine ansprechende Beschreibung auf Deutsch
- [ ] Gegeben Screenshots, wenn sie erstellt werden, dann zeigen sie die wichtigsten Funktionen der App
- [ ] Gegeben ein Feature-Graphic, wenn es erstellt wird, dann ist es 1024x500 Pixel gross
- [ ] Gegeben Altersfreigabe, wenn sie beantragt wird, dann ist sie familienfreundlich (USK 0 / PEGI 3)

## Store-Listing Inhalte

### App-Name
```
TaschengeldManager - Familien-App
```
(Max. 30 Zeichen)

### Kurzbeschreibung
```
Taschengeld einfach verwalten - fuer Eltern und Kinder
```
(Max. 80 Zeichen)

### Ausfuehrliche Beschreibung
```
TaschengeldManager - Die Familien-App fuer Taschengeld

Verwalte das Taschengeld deiner Kinder einfach und uebersichtlich. Mit TaschengeldManager lernen Kinder spielerisch den Umgang mit Geld.

FUeR ELTERN:
- Automatisches Taschengeld einrichten (woechentlich/monatlich)
- Geldanfragen der Kinder pruefen und genehmigen
- Ueberblick ueber alle Ausgaben
- Zinsen als Sparanreiz einstellen
- Mehrere Kinder verwalten

FUeR KINDER:
- Eigenes Konto mit Kontostand
- Ausgaben einfach erfassen
- Geld bei den Eltern anfragen
- Sparziele setzen und verfolgen
- Sicherer Login mit PIN oder Fingerabdruck

FUeR DIE GANZE FAMILIE:
- Verwandte koennen Geschenke senden
- Geburtstags-Erinnerungen
- Offline-Nutzung moeglich
- Push-Benachrichtigungen

SICHERHEIT:
- Kindgerechte Oberflaeche
- Keine echten Bankdaten erforderlich
- DSGVO-konform
- Server in Deutschland

KOSTENLOS:
- Alle Grundfunktionen kostenlos
- Keine Werbung
- Keine In-App-Kaeufe fuer Kernfunktionen

Perfekt fuer Familien, die ihren Kindern einen verantwortungsvollen Umgang mit Geld beibringen moechten.

Fragen oder Feedback? kontakt@taschengeld.app
```
(Max. 4000 Zeichen)

### Screenshots (min. 4, max. 8)

1. **Kind-Dashboard**
   - Kontostand prominent
   - Letzte Transaktionen
   - Freundliche Farben

2. **Ausgabe erfassen**
   - Einfaches Formular
   - Kategorien-Auswahl
   - Speichern-Button

3. **Transaktionsliste**
   - Uebersichtliche Liste
   - Kategorien-Icons
   - Filter-Option

4. **Eltern-Dashboard**
   - Familienuebersicht
   - Anfragen-Liste
   - Kinder-Konten

5. **Geldanfrage stellen** (optional)
   - Betrag eingeben
   - Grund angeben
   - Absenden

6. **Automatisches Taschengeld** (optional)
   - Einrichtungs-Wizard
   - Betrag und Haeufigkeit
   - Bestaetigung

### Screenshot-Spezifikationen

| Geraet | Groesse | Format |
|--------|---------|--------|
| Phone | 1080x1920 (min) | PNG/JPEG |
| 7" Tablet | 1200x1920 | PNG/JPEG |
| 10" Tablet | 1600x2560 | PNG/JPEG |

### Feature-Graphic
```
Groesse: 1024x500 Pixel
Inhalt: App-Logo + Tagline + Illustration
Farben: Markenfarben (#4CAF50, #FFC107)
Format: PNG/JPEG
```

### App-Icon
```
Groesse: 512x512 Pixel
Format: PNG (32-bit mit Alpha)
```

## Kategorisierung

### Kategorie
- Primaer: Finanzen
- Sekundaer: Familie

### Altersfreigabe
- USK: 0 (Freigegeben ohne Altersbeschraenkung)
- PEGI: 3
- Kein anstoe?iger Inhalt
- Keine In-App-Kaeufe mit echtem Geld

### Content-Rating Fragebogen
- Gewalt: Nein
- Angst: Nein
- Sexuelle Inhalte: Nein
- Drogen: Nein
- Gluecksspiel: Nein
- Benutzer-Interaktion: Ja (Familie)
- Standort-Zugriff: Nein
- Personenbezogene Daten: Ja (mit Einwilligung)

## Checkliste vor Veroeffentlichung

### Pflicht
- [ ] App-Name (30 Zeichen)
- [ ] Kurzbeschreibung (80 Zeichen)
- [ ] Ausfuehrliche Beschreibung (4000 Zeichen)
- [ ] App-Icon (512x512)
- [ ] Feature-Graphic (1024x500)
- [ ] Screenshots (min. 4 Phone)
- [ ] Altersfreigabe (Fragebogen ausgefuellt)
- [ ] Datenschutzerklaerung URL
- [ ] Kategorie gewaehlt

### Empfohlen
- [ ] Tablet-Screenshots
- [ ] Video-Vorschau (optional)
- [ ] Lokalisierung (Deutsch primary)
- [ ] Kontakt-Email
- [ ] Website-URL

## Datenschutzerklaerung

URL: `https://taschengeld.app/privacy`

Inhalt muss enthalten:
- Welche Daten werden gesammelt
- Wie werden Daten verwendet
- Speicherung und Sicherheit
- Rechte der Nutzer (DSGVO)
- Kontaktinformationen

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Beschreibung | Grammatikalisch korrekt, ansprechend |
| TC-002 | Screenshots | Alle Kernfunktionen gezeigt |
| TC-003 | Icon | Erkennbar in verschiedenen Groessen |
| TC-004 | Feature-Graphic | Sieht professionell aus |
| TC-005 | Altersfreigabe | USK 0 / PEGI 3 |
| TC-006 | Datenschutz-URL | Erreichbar und aktuell |

## Story Points

2

## Prioritaet

Mittel
