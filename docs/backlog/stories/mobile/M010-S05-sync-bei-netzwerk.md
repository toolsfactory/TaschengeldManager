# Story M010-S05: Sync bei Netzwerkverbindung

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Benutzer** moechte ich **dass meine offline erfassten Daten automatisch synchronisiert werden, wenn ich wieder online bin**, damit **meine Daten auf dem Server aktuell sind ohne dass ich manuell etwas tun muss**.

## Akzeptanzkriterien

- [ ] Gegeben ausstehende Offline-Aktionen, wenn die Internetverbindung wiederhergestellt wird, dann startet die Synchronisation automatisch
- [ ] Gegeben laufende Synchronisation, wenn der Benutzer die App nutzt, dann sieht er einen Fortschrittsindikator
- [ ] Gegeben eine erfolgreiche Synchronisation, wenn alle Aktionen verarbeitet sind, dann werden die lokalen Daten aktualisiert
- [ ] Gegeben eine fehlgeschlagene Synchronisation einer Aktion, wenn der Fehler behoben werden kann, dann wird erneut versucht (max. 3x)
- [ ] Gegeben eine abgeschlossene Synchronisation, wenn der Benutzer die App nutzt, dann wird eine Bestaetigung angezeigt

## UI-Entwurf

### Sync-Indikator
```
+------------------------------------+
| [Sync] Synchronisiere... (2/5)     |
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
|                                    |
|  ...                               |
+------------------------------------+
```

### Sync abgeschlossen
```
+------------------------------------+
| [Check] 5 Aenderungen sync.        |
+------------------------------------+
(verschwindet nach 3 Sekunden)
```

### Sync fehlgeschlagen
```
+------------------------------------+
| [X] Sync fehlgeschlagen    [Retry] |
+------------------------------------+
```

## Technische Notizen

- Service: `ISyncService`
- Trigger: `Connectivity.ConnectivityChanged` Event
- Retry-Strategie: Exponential Backoff (1s, 2s, 4s)
- Max. 3 Versuche pro Aktion
- Sync laeuft im Hintergrund (BackgroundService)

## Implementierungshinweise

```csharp
public interface ISyncService
{
    bool IsSyncing { get; }
    int PendingCount { get; }
    int CurrentIndex { get; }
    event EventHandler<SyncProgressEventArgs> SyncProgressChanged;
    event EventHandler<SyncCompletedEventArgs> SyncCompleted;
    Task StartSyncAsync();
}

public class SyncService : ISyncService
{
    private readonly IOfflineQueueService _offlineQueue;
    private readonly ITransactionService _transactionService;
    private readonly IConnectivityService _connectivity;
    private readonly IBalanceCacheService _balanceCache;

    private const int MaxRetries = 3;

    public SyncService(
        IConnectivityService connectivity,
        IOfflineQueueService offlineQueue,
        ITransactionService transactionService)
    {
        _connectivity = connectivity;
        _offlineQueue = offlineQueue;
        _transactionService = transactionService;

        // Automatische Synchronisation bei Verbindung
        connectivity.ConnectivityChanged += async (s, e) =>
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                await StartSyncAsync();
            }
        };
    }

    public async Task StartSyncAsync()
    {
        if (IsSyncing || !_connectivity.IsConnected) return;

        IsSyncing = true;
        var pending = await _offlineQueue.GetPendingTransactionsAsync(_authService.UserId);
        PendingCount = pending.Count;

        var syncedCount = 0;
        var failedCount = 0;

        for (int i = 0; i < pending.Count; i++)
        {
            CurrentIndex = i + 1;
            SyncProgressChanged?.Invoke(this, new SyncProgressEventArgs(CurrentIndex, PendingCount));

            var item = pending[i];
            var success = await SyncItemWithRetryAsync(item);

            if (success)
            {
                syncedCount++;
                await _offlineQueue.RemoveSyncedAsync(item.LocalId);
            }
            else
            {
                failedCount++;
            }
        }

        // Cache aktualisieren nach erfolgreicher Sync
        if (syncedCount > 0)
        {
            await RefreshCacheAsync();
        }

        IsSyncing = false;
        SyncCompleted?.Invoke(this, new SyncCompletedEventArgs(syncedCount, failedCount));
    }

    private async Task<bool> SyncItemWithRetryAsync(PendingTransaction item)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                await _offlineQueue.UpdateStatusAsync(item.LocalId, PendingStatus.Syncing);

                await _transactionService.CreateTransactionAsync(new CreateTransactionRequest
                {
                    Amount = item.Amount,
                    Category = item.Category,
                    Description = item.Description
                });

                await _offlineQueue.UpdateStatusAsync(item.LocalId, PendingStatus.Synced);
                return true;
            }
            catch (Exception ex)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                await Task.Delay(delay);
            }
        }

        await _offlineQueue.UpdateStatusAsync(item.LocalId, PendingStatus.Failed, "Max retries exceeded");
        return false;
    }

    private async Task RefreshCacheAsync()
    {
        var balance = await _transactionService.GetBalanceAsync();
        await _balanceCache.SetCachedBalanceAsync(_authService.UserId, balance.Amount);

        var transactions = await _transactionService.GetTransactionsAsync();
        await _transactionCache.SetCachedTransactionsAsync(_authService.UserId, transactions);
    }
}

// AppShell oder MainPage - Sync UI
public partial class AppShellViewModel : ObservableObject
{
    private readonly ISyncService _syncService;

    [ObservableProperty]
    private bool _isSyncing;

    [ObservableProperty]
    private string _syncStatus = string.Empty;

    public AppShellViewModel(ISyncService syncService)
    {
        _syncService = syncService;

        syncService.SyncProgressChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsSyncing = true;
                SyncStatus = $"Synchronisiere... ({e.Current}/{e.Total})";
            });
        };

        syncService.SyncCompleted += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SyncStatus = e.FailedCount > 0
                    ? $"Sync: {e.FailedCount} fehlgeschlagen"
                    : $"{e.SyncedCount} Aenderungen synchronisiert";

                await Task.Delay(3000);
                IsSyncing = false;
            });
        };
    }
}
```

## API-Endpunkte

Die Sync verwendet die bestehenden API-Endpunkte:
- `POST /api/transactions` - Transaktion erstellen
- `GET /api/accounts/balance` - Kontostand abrufen
- `GET /api/transactions` - Transaktionsliste

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Online gehen mit 3 Pending | Alle 3 werden synchronisiert |
| TC-002 | Sync-Fehler bei 1 von 3 | 2 erfolgreich, 1 als Failed markiert |
| TC-003 | Retry bei Netzwerkfehler | Bis zu 3 Versuche |
| TC-004 | Fortschrittsanzeige | "2/5" wird korrekt angezeigt |
| TC-005 | Cache nach Sync | Balance und Transactions aktualisiert |
| TC-006 | Sync waehrend App-Nutzung | Kein Einfrieren der UI |

## Story Points

5

## Prioritaet

Hoch
