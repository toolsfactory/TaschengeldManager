# Story M010-S06: Konfliktaufloesung bei Sync

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Benutzer** moechte ich **dass Konflikte bei der Synchronisation sinnvoll aufgeloest werden**, damit **keine Daten verloren gehen und ich bei Problemen informiert werde**.

## Akzeptanzkriterien

- [ ] Gegeben eine Offline-Ausgabe die den Server-Kontostand ueberschreiten wuerde, wenn sie synchronisiert wird, dann wird eine Warnung angezeigt
- [ ] Gegeben unterschiedliche Kontostands-Werte (lokal vs. Server), wenn die Sync abgeschlossen ist, dann wird der Server-Wert uebernommen
- [ ] Gegeben eine doppelte Transaktion (gleiche ID), wenn sie synchronisiert werden soll, dann wird sie als Duplikat erkannt und uebersprungen
- [ ] Gegeben ein nicht aufloessbarer Konflikt, wenn er auftritt, dann wird der Benutzer informiert und kann manuell entscheiden

## UI-Entwurf

### Warnung: Kontostand ueberschritten
```
+------------------------------------+
|           Achtung                  |
+------------------------------------+
|                                    |
|  Die Offline-Ausgabe von 50,00 EUR |
|  wuerde deinen aktuellen           |
|  Kontostand (30,00 EUR)            |
|  ueberschreiten.                   |
|                                    |
|  Was moechtest du tun?             |
|                                    |
|  +--------------------------------+|
|  |    Trotzdem speichern          ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |    Ausgabe verwerfen           ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Konflikt-Dialog
```
+------------------------------------+
|        Sync-Konflikt               |
+------------------------------------+
|                                    |
|  Es gibt einen Konflikt bei        |
|  folgender Transaktion:            |
|                                    |
|  "Spielzeug" - 15,00 EUR           |
|  Erfasst am: 20.01.2026, 14:00     |
|                                    |
|  Der Server hat bereits eine       |
|  aehnliche Transaktion.            |
|                                    |
|  +--------------------------------+|
|  |    Server-Version behalten     ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |    Lokale Version verwenden    ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |    Beide behalten              ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Notizen

- Konflikttypen: `InsufficientBalance`, `DuplicateTransaction`, `DataMismatch`
- Server-Authority: Bei Zweifel immer Server-Version bevorzugen
- Duplikat-Erkennung: Gleicher Betrag + Beschreibung + Datum (+/- 1 Minute)
- Service: `IConflictResolutionService`

## Implementierungshinweise

```csharp
public enum ConflictType
{
    InsufficientBalance,
    DuplicateTransaction,
    DataMismatch
}

public enum ConflictResolution
{
    UseServer,
    UseLocal,
    KeepBoth,
    Discard
}

public class SyncConflict
{
    public ConflictType Type { get; set; }
    public PendingTransaction LocalItem { get; set; } = null!;
    public TransactionDto? ServerItem { get; set; }
    public string Message { get; set; } = string.Empty;
}

public interface IConflictResolutionService
{
    Task<SyncConflict?> DetectConflictAsync(PendingTransaction pending);
    Task<bool> ResolveAsync(SyncConflict conflict, ConflictResolution resolution);
}

public class ConflictResolutionService : IConflictResolutionService
{
    public async Task<SyncConflict?> DetectConflictAsync(PendingTransaction pending)
    {
        // Pruefen: Kontostand ausreichend?
        var currentBalance = await _accountService.GetBalanceAsync();
        if (pending.Amount < 0 && Math.Abs(pending.Amount) > currentBalance.Amount)
        {
            return new SyncConflict
            {
                Type = ConflictType.InsufficientBalance,
                LocalItem = pending,
                Message = $"Ausgabe ({Math.Abs(pending.Amount):C}) ueberschreitet Kontostand ({currentBalance.Amount:C})"
            };
        }

        // Pruefen: Duplikat?
        var recentTransactions = await _transactionService.GetTransactionsAsync();
        var potentialDuplicate = recentTransactions.FirstOrDefault(t =>
            t.Amount == pending.Amount &&
            t.Description == pending.Description &&
            Math.Abs((t.Date - pending.CreatedAt).TotalMinutes) < 1);

        if (potentialDuplicate != null)
        {
            return new SyncConflict
            {
                Type = ConflictType.DuplicateTransaction,
                LocalItem = pending,
                ServerItem = potentialDuplicate,
                Message = "Eine aehnliche Transaktion existiert bereits"
            };
        }

        return null;
    }

    public async Task<bool> ResolveAsync(SyncConflict conflict, ConflictResolution resolution)
    {
        switch (resolution)
        {
            case ConflictResolution.UseServer:
                // Lokale Aenderung verwerfen
                await _offlineQueue.UpdateStatusAsync(conflict.LocalItem.LocalId, PendingStatus.Synced);
                return true;

            case ConflictResolution.UseLocal:
                // Trotzdem synchronisieren
                await _transactionService.CreateTransactionAsync(MapToRequest(conflict.LocalItem));
                await _offlineQueue.UpdateStatusAsync(conflict.LocalItem.LocalId, PendingStatus.Synced);
                return true;

            case ConflictResolution.KeepBoth:
                // Lokale Version zusaetzlich speichern
                await _transactionService.CreateTransactionAsync(MapToRequest(conflict.LocalItem));
                await _offlineQueue.UpdateStatusAsync(conflict.LocalItem.LocalId, PendingStatus.Synced);
                return true;

            case ConflictResolution.Discard:
                // Lokale Aenderung entfernen
                await _offlineQueue.RemoveSyncedAsync(conflict.LocalItem.LocalId);
                return true;

            default:
                return false;
        }
    }
}

// Sync Service Integration
public partial class SyncService
{
    private async Task<bool> SyncItemWithRetryAsync(PendingTransaction item)
    {
        // Konflikt-Erkennung
        var conflict = await _conflictResolution.DetectConflictAsync(item);

        if (conflict != null)
        {
            // Bei kritischen Konflikten Benutzer fragen
            if (conflict.Type == ConflictType.InsufficientBalance ||
                conflict.Type == ConflictType.DuplicateTransaction)
            {
                var resolution = await ShowConflictDialogAsync(conflict);
                return await _conflictResolution.ResolveAsync(conflict, resolution);
            }
        }

        // Normale Sync ohne Konflikt
        // ... (wie in M010-S05)
    }

    private async Task<ConflictResolution> ShowConflictDialogAsync(SyncConflict conflict)
    {
        var result = await Application.Current!.MainPage!.DisplayActionSheet(
            conflict.Message,
            "Abbrechen",
            null,
            "Server-Version behalten",
            "Lokale Version verwenden",
            "Beide behalten"
        );

        return result switch
        {
            "Server-Version behalten" => ConflictResolution.UseServer,
            "Lokale Version verwenden" => ConflictResolution.UseLocal,
            "Beide behalten" => ConflictResolution.KeepBoth,
            _ => ConflictResolution.Discard
        };
    }
}
```

## Konfliktaufloesung-Matrix

| Konflikt | Standard-Aufloesung | Benutzer-Optionen |
|----------|---------------------|-------------------|
| Kontostand ueberschritten | Benutzer fragen | Trotzdem / Verwerfen |
| Duplikat erkannt | Benutzer fragen | Server / Lokal / Beide |
| Server-Fehler | Retry (3x) | - |
| Unbekannter Fehler | Als Failed markieren | - |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Ausgabe > Kontostand | Dialog erscheint |
| TC-002 | Duplikat erkannt | Dialog mit Optionen |
| TC-003 | "Trotzdem speichern" gewaehlt | Ausgabe wird synchronisiert |
| TC-004 | "Verwerfen" gewaehlt | Ausgabe wird entfernt |
| TC-005 | Server-Version behalten | Lokale wird entfernt |
| TC-006 | Beide behalten | Zwei Transaktionen existieren |

## Story Points

3

## Prioritaet

Mittel
