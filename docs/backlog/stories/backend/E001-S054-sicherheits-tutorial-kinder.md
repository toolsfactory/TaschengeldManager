# E001-S054: Sicherheits-Tutorial für Kinder

## Status: Frontend

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Kind** möchte ich **spielerisch lernen, warum Sicherheit wichtig ist**, damit **ich verstehe, warum ich einen Geheimcode brauche und wie ich mein Konto schütze**.

## Akzeptanzkriterien

- [ ] Interaktives Tutorial wird bei Ersteinrichtung angezeigt
- [ ] Kindgerechte Sprache und Bilder
- [ ] Erklärt: Warum brauche ich einen Geheimcode?
- [ ] Erklärt: Was ist Biometrie (Fingerabdruck/Gesicht)?
- [ ] Erklärt: Warum sage ich niemandem mein Passwort?
- [ ] Quiz-Element zum Abschluss (mit Belohnung/Badge)
- [ ] Tutorial kann übersprungen werden (Eltern-Entscheidung)
- [ ] Tutorial kann später wiederholt werden

## API-Endpunkte

```
GET /api/tutorials/security/child

Response 200:
{
  "tutorialId": "security-basics",
  "steps": [
    {
      "id": 1,
      "title": "Deine Schatztruhe",
      "content": "Stell dir vor, dein Konto ist wie eine Schatztruhe...",
      "imageUrl": "/images/tutorial/treasure-chest.png",
      "type": "info"
    },
    {
      "id": 2,
      "title": "Der Schlüssel",
      "content": "Dein Passwort ist der erste Schlüssel...",
      "imageUrl": "/images/tutorial/key.png",
      "type": "info"
    },
    {
      "id": 3,
      "title": "Der Geheimcode",
      "content": "Aber was, wenn jemand deinen Schlüssel findet?",
      "imageUrl": "/images/tutorial/secret-code.png",
      "type": "info"
    },
    {
      "id": 4,
      "title": "Quiz Zeit!",
      "question": "Wem darfst du dein Passwort sagen?",
      "options": [
        {"id": "a", "text": "Meinem besten Freund", "correct": false},
        {"id": "b", "text": "Niemandem", "correct": true},
        {"id": "c", "text": "Meinem Lehrer", "correct": false}
      ],
      "type": "quiz"
    }
  ],
  "completionBadge": {
    "name": "Sicherheits-Held",
    "imageUrl": "/images/badges/security-hero.png"
  }
}

---

POST /api/tutorials/security/child/complete

Request:
{
  "tutorialId": "security-basics",
  "quizScore": 100
}

Response 200:
{
  "completed": true,
  "badge": {
    "name": "Sicherheits-Held",
    "earnedAt": "2024-01-20T10:30:00Z"
  },
  "message": "Super gemacht! Du bist jetzt ein Sicherheits-Held!"
}
```

## Technische Hinweise

- Tutorial-Inhalte serverseitig für einfache Aktualisierung
- Fortschritt wird gespeichert (Kind kann unterbrechen)
- Badge wird in Benutzerprofil gespeichert
- A/B-Testing möglich für verschiedene Tutorial-Varianten
- Eltern können Tutorial-Abschluss in Kinder-Profil sehen
- Mehrsprachig vorbereiten (DE, später EN)
- Barrierefreiheit: Vorlesefunktion, große Buttons

## Story Points

5

## Priorität

Niedrig - Frontend-Story (Backend nur Datenlieferung)

## Hinweis

Diese Story ist primär eine Frontend-Story. Das Backend liefert nur die Tutorial-Inhalte und speichert den Fortschritt. Die eigentliche Implementierung der interaktiven Elemente erfolgt im Mobile-Client.
