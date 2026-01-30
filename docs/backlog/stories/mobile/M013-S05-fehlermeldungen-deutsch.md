# Story M013-S05: Fehlermeldungen Deutsch

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **deutschsprachiger Benutzer** moechte ich **alle Fehlermeldungen auf Deutsch sehen**, damit **ich sie sofort verstehen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein API-Fehler, wenn er angezeigt wird, dann ist die Meldung auf Deutsch
- [ ] Gegeben ein Validierungsfehler, wenn er angezeigt wird, dann ist die Meldung auf Deutsch
- [ ] Gegeben ein System-Fehler, wenn er angezeigt wird, dann ist die Meldung auf Deutsch
- [ ] Gegeben eine unbekannte Fehlermeldung vom Server, wenn sie angezeigt wird, dann wird eine generische deutsche Meldung verwendet

## Technische Implementierung

### Error-Message-Service

```csharp
public interface IErrorMessageService
{
    string GetMessage(string errorCode);
    string GetMessage(HttpStatusCode statusCode);
    string GetValidationMessage(string field, string validationType);
}

public class ErrorMessageService : IErrorMessageService
{
    private readonly Dictionary<string, string> _errorMessages = new()
    {
        // Netzwerk-Fehler
        ["network_error"] = "Keine Internetverbindung. Bitte pruefe deine Verbindung.",
        ["timeout"] = "Die Anfrage hat zu lange gedauert. Bitte versuche es erneut.",
        ["server_unavailable"] = "Der Server ist voruebergehend nicht erreichbar. Bitte versuche es spaeter.",

        // Authentifizierung
        ["auth_failed"] = "Anmeldung fehlgeschlagen. Bitte pruefe deine Zugangsdaten.",
        ["session_expired"] = "Deine Sitzung ist abgelaufen. Bitte melde dich erneut an.",
        ["invalid_token"] = "Deine Anmeldung ist ungueltig. Bitte melde dich erneut an.",
        ["unauthorized"] = "Du bist nicht berechtigt fuer diese Aktion.",
        ["forbidden"] = "Zugriff verweigert.",

        // Validierung
        ["validation_error"] = "Bitte ueberprüfe deine Eingaben.",
        ["required_field"] = "Dieses Feld ist erforderlich.",
        ["invalid_format"] = "Ungueltiges Format.",
        ["value_too_long"] = "Der Wert ist zu lang.",
        ["value_too_short"] = "Der Wert ist zu kurz.",
        ["value_out_of_range"] = "Der Wert liegt ausserhalb des gueltigen Bereichs.",

        // Ressourcen
        ["not_found"] = "Die angeforderten Daten wurden nicht gefunden.",
        ["already_exists"] = "Dieser Eintrag existiert bereits.",
        ["conflict"] = "Die Aenderung konnte nicht durchgefuehrt werden. Bitte lade die Seite neu.",

        // Transaktionen
        ["insufficient_balance"] = "Nicht genuegend Guthaben fuer diese Transaktion.",
        ["invalid_amount"] = "Bitte gib einen gueltigen Betrag ein.",
        ["amount_too_high"] = "Der Betrag ist zu hoch.",
        ["amount_too_low"] = "Der Betrag ist zu niedrig.",

        // Anfragen
        ["request_already_processed"] = "Diese Anfrage wurde bereits bearbeitet.",
        ["request_expired"] = "Diese Anfrage ist abgelaufen.",

        // Account
        ["email_already_registered"] = "Diese E-Mail-Adresse ist bereits registriert.",
        ["weak_password"] = "Das Passwort ist zu schwach. Bitte waehle ein staerkeres Passwort.",
        ["invalid_mfa_code"] = "Der Authentifizierungscode ist ungueltig.",
        ["account_locked"] = "Dein Konto wurde voruebergehend gesperrt. Bitte versuche es spaeter.",

        // Allgemein
        ["unknown_error"] = "Ein unerwarteter Fehler ist aufgetreten. Bitte versuche es erneut.",
        ["maintenance"] = "Die App wird gerade gewartet. Bitte versuche es spaeter."
    };

    private readonly Dictionary<string, Dictionary<string, string>> _fieldValidationMessages = new()
    {
        ["amount"] = new()
        {
            ["required"] = "Bitte gib einen Betrag ein.",
            ["range"] = "Der Betrag muss zwischen {0} und {1} EUR liegen.",
            ["positive"] = "Der Betrag muss positiv sein."
        },
        ["email"] = new()
        {
            ["required"] = "Bitte gib deine E-Mail-Adresse ein.",
            ["format"] = "Bitte gib eine gueltige E-Mail-Adresse ein.",
            ["unique"] = "Diese E-Mail-Adresse wird bereits verwendet."
        },
        ["password"] = new()
        {
            ["required"] = "Bitte gib ein Passwort ein.",
            ["minLength"] = "Das Passwort muss mindestens {0} Zeichen haben.",
            ["strength"] = "Das Passwort ist zu schwach."
        },
        ["firstName"] = new()
        {
            ["required"] = "Bitte gib deinen Vornamen ein.",
            ["maxLength"] = "Der Vorname darf maximal {0} Zeichen haben."
        },
        ["lastName"] = new()
        {
            ["required"] = "Bitte gib deinen Nachnamen ein.",
            ["maxLength"] = "Der Nachname darf maximal {0} Zeichen haben."
        },
        ["category"] = new()
        {
            ["required"] = "Bitte waehle eine Kategorie."
        },
        ["description"] = new()
        {
            ["maxLength"] = "Die Beschreibung darf maximal {0} Zeichen haben."
        }
    };

    public string GetMessage(string errorCode)
    {
        return _errorMessages.TryGetValue(errorCode, out var message)
            ? message
            : _errorMessages["unknown_error"];
    }

    public string GetMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => _errorMessages["validation_error"],
            HttpStatusCode.Unauthorized => _errorMessages["unauthorized"],
            HttpStatusCode.Forbidden => _errorMessages["forbidden"],
            HttpStatusCode.NotFound => _errorMessages["not_found"],
            HttpStatusCode.Conflict => _errorMessages["conflict"],
            HttpStatusCode.TooManyRequests => "Zu viele Anfragen. Bitte warte einen Moment.",
            HttpStatusCode.InternalServerError => _errorMessages["server_unavailable"],
            HttpStatusCode.ServiceUnavailable => _errorMessages["maintenance"],
            _ => _errorMessages["unknown_error"]
        };
    }

    public string GetValidationMessage(string field, string validationType, params object[] args)
    {
        if (_fieldValidationMessages.TryGetValue(field, out var fieldMessages) &&
            fieldMessages.TryGetValue(validationType, out var message))
        {
            return string.Format(message, args);
        }

        // Fallback zu allgemeiner Nachricht
        return validationType switch
        {
            "required" => "Dieses Feld ist erforderlich.",
            "format" => "Ungueltiges Format.",
            "range" => "Der Wert liegt ausserhalb des gueltigen Bereichs.",
            "maxLength" => $"Maximal {args.FirstOrDefault()} Zeichen erlaubt.",
            "minLength" => $"Mindestens {args.FirstOrDefault()} Zeichen erforderlich.",
            _ => "Ungueltige Eingabe."
        };
    }
}
```

### API-Error-Response-Mapping

```csharp
public class ApiErrorResponse
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}

public class ApiClient
{
    private readonly IErrorMessageService _errorMessageService;

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content);

            if (errorResponse?.Code != null)
            {
                throw new ApiException(
                    response.StatusCode,
                    _errorMessageService.GetMessage(errorResponse.Code));
            }

            if (errorResponse?.Errors != null)
            {
                var validationErrors = new Dictionary<string, string>();
                foreach (var (field, messages) in errorResponse.Errors)
                {
                    validationErrors[field] = string.Join(", ", messages);
                }
                throw new ValidationException(validationErrors);
            }
        }
        catch (JsonException)
        {
            // Fallback wenn Antwort nicht parsbar
        }

        throw new ApiException(
            response.StatusCode,
            _errorMessageService.GetMessage(response.StatusCode));
    }
}
```

## Fehlermeldungen-Katalog

| Code | Deutsche Meldung |
|------|------------------|
| network_error | Keine Internetverbindung |
| timeout | Anfrage hat zu lange gedauert |
| auth_failed | Anmeldung fehlgeschlagen |
| session_expired | Sitzung abgelaufen |
| validation_error | Bitte ueberprüfe deine Eingaben |
| not_found | Daten nicht gefunden |
| insufficient_balance | Nicht genuegend Guthaben |
| unknown_error | Unerwarteter Fehler |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | API gibt "auth_failed" zurueck | "Anmeldung fehlgeschlagen..." |
| TC-002 | HTTP 404 | "Die angeforderten Daten wurden nicht gefunden" |
| TC-003 | HTTP 500 | "Der Server ist voruebergehend nicht erreichbar" |
| TC-004 | Unbekannter Error-Code | "Ein unerwarteter Fehler ist aufgetreten" |
| TC-005 | Validierung: Email Format | "Bitte gib eine gueltige E-Mail-Adresse ein" |
| TC-006 | Validierung: Betrag required | "Bitte gib einen Betrag ein" |

## Story Points

1

## Prioritaet

Mittel
