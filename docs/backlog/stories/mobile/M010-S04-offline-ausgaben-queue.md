# Story M010-S04: Offline-Ausgaben in Queue

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Kind** moechte ich **auch offline Ausgaben erfassen koennen**, damit **ich meine Ausgaben sofort eintragen kann, auch wenn ich kein Internet habe**.

## Akzeptanzkriterien

- [ ] Gegeben Offline-Status, wenn das Kind eine Ausgabe erfasst, dann wird sie lokal in einer Queue gespeichert
- [ ] Gegeben eine Offline-Ausgabe, wenn sie gespeichert wird, dann wird eine Bestaetigung angezeigt mit Hinweis auf spaetere Synchronisation
- [ ] Gegeben Offline-Ausgaben in der Queue, wenn der Benutzer die Transaktionsliste oeffnet, dann werden sie mit einem "Ausstehend"-Marker angezeigt
- [ ] Gegeben Offline-Ausgaben, wenn der lokale Kontostand angezeigt wird, dann wird er um die ausstehenden Ausgaben reduziert

## UI-Entwurf

### Ausgabe erfassen (Offline)
```
+------------------------------------+
| [!] Offline - wird spaeter sync.   |
+------------------------------------+
|  <- Zurueck      Ausgabe erfassen  |
+------------------------------------+
|                                    |
|  Betrag                            |
|  +--------------------------------+|
|  |          5,00 EUR              ||
|  +--------------------------------+|
|                                    |
|  Kategorie                         |
|  +--------------------------------+|
|  | Essen & Trinken            [v] ||
|  +--------------------------------+|
|                                    |
|  Beschreibung (optional)           |
|  +--------------------------------+|
|  | Eis                            ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |     [Save] Ausgabe speichern   ||
|  +--------------------------------+|
|                                    |
|  [Info] Wird synchronisiert, wenn  |
|  du wieder online bist.            |
|                                    |
+------------------------------------+
```

### Transaktionsliste mit ausstehenden Ausgaben
```
+------------------------------------+
| [!] Offline - Daten evtl. veraltet |
+------------------------------------+
|  <- Zurueck       Transaktionen    |
+------------------------------------+
|                                    |
|  Ausstehend (2)                    |
|  +--------------------------------+|
|  | [Clock] Eis                    ||
|  |         -5,00 EUR              ||
|  |         Gerade eben - aussteh. ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Clock] Comic                  ||
|  |         -3,50 EUR              ||
|  |         vor 10 Min - aussteh.  ||
|  +--------------------------------+|
|                                    |
|  Synchronisiert                    |
|  +--------------------------------+|
|  | [Cart] Suessigkeiten           ||
|  |        -2,50 EUR               ||
|  |        20.01.2026, 14:30       ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Notizen

- SQLite-Entity: `PendingTransaction`
- Queue wird bei Sync abgearbeitet (FIFO)
- Lokaler Kontostand: `CachedBalance - Sum(PendingTransactions)`
- Service: `IOfflineQueueService`

## Implementierungshinweise

```csharp
// Datenbank-Entity
[Table("PendingTransactions")]
public class PendingTransaction
{
    [PrimaryKey]
    public Guid LocalId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public PendingStatus Status { get; set; } = PendingStatus.Pending;
    public string? ErrorMessage { get; set; }
}

public enum PendingStatus
{
    Pending,
    Syncing,
    Synced,
    Failed
}

// Queue Service
public interface IOfflineQueueService
{
    Task<Guid> EnqueueTransactionAsync(CreateTransactionRequest request);
    Task<IReadOnlyList<PendingTransaction>> GetPendingTransactionsAsync(Guid userId);
    Task<decimal> GetPendingTotalAsync(Guid userId);
    Task UpdateStatusAsync(Guid localId, PendingStatus status, string? error = null);
    Task RemoveSyncedAsync(Guid localId);
}

public class OfflineQueueService : IOfflineQueueService
{
    private readonly SQLiteAsyncConnection _db;

    public async Task<Guid> EnqueueTransactionAsync(CreateTransactionRequest request)
    {
        var pending = new PendingTransaction
        {
            LocalId = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = request.Amount,
            Category = request.Category,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Status = PendingStatus.Pending
        };

        await _db.InsertAsync(pending);
        return pending.LocalId;
    }

    public async Task<decimal> GetPendingTotalAsync(Guid userId)
    {
        var pending = await _db.Table<PendingTransaction>()
            .Where(p => p.UserId == userId && p.Status == PendingStatus.Pending)
            .ToListAsync();

        return pending.Sum(p => p.Amount);
    }
}

// ViewModel fuer Ausgabe erfassen
public partial class CreateExpenseViewModel : BaseViewModel
{
    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [RelayCommand]
    private async Task SaveExpenseAsync()
    {
        var request = new CreateTransactionRequest
        {
            UserId = _authService.UserId,
            Amount = -Math.Abs(Amount), // Ausgaben sind negativ
            Category = Category,
            Description = Description
        };

        if (_connectivity.IsConnected)
        {
            await _transactionService.CreateTransactionAsync(request);
            await _toastService.ShowSuccessAsync("Ausgabe gespeichert!");
        }
        else
        {
            await _offlineQueue.EnqueueTransactionAsync(request);
            await _toastService.ShowInfoAsync("Ausgabe gespeichert - wird spaeter synchronisiert");
        }

        await Shell.Current.GoToAsync("..");
    }
}

// Dashboard: Effektiver Kontostand
public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private decimal _displayedBalance;

    [ObservableProperty]
    private decimal _pendingAmount;

    private async Task UpdateDisplayedBalanceAsync()
    {
        var cachedBalance = await _balanceCache.GetCachedBalanceAsync(_authService.UserId);
        var pendingTotal = await _offlineQueue.GetPendingTotalAsync(_authService.UserId);

        PendingAmount = pendingTotal;
        DisplayedBalance = (cachedBalance?.Amount ?? 0) + pendingTotal;
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Ausgabe offline erfassen | Wird in Queue gespeichert |
| TC-002 | 3 Ausgaben offline | Alle 3 in Queue |
| TC-003 | Transaktionsliste offline | Ausstehende werden angezeigt |
| TC-004 | Kontostand mit Pending | Balance - Pending korrekt |
| TC-005 | Betrag 0 eingeben | Validierungsfehler |

## Story Points

3

## Prioritaet

Hoch
