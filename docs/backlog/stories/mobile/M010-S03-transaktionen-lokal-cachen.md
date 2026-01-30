# Story M010-S03: Transaktionen lokal cachen

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Kind** moechte ich **meine letzten Transaktionen auch offline sehen koennen**, damit **ich nachvollziehen kann, wofuer ich Geld ausgegeben habe**.

## Akzeptanzkriterien

- [ ] Gegeben vorher abgerufene Transaktionen, wenn der Benutzer offline ist, dann werden die gecachten Transaktionen angezeigt
- [ ] Gegeben gecachte Transaktionen, wenn sie angezeigt werden, dann ist ein Hinweis sichtbar dass die Daten moeglicherweise veraltet sind
- [ ] Gegeben Online-Status, wenn Transaktionen abgerufen werden, dann wird der Cache mit den neuen Daten aktualisiert
- [ ] Gegeben ein Cache, wenn neue Transaktionen abgerufen werden, dann werden die letzten 30 Transaktionen gespeichert

## UI-Entwurf

```
+------------------------------------+
| [!] Offline - Daten evtl. veraltet |
+------------------------------------+
|  <- Zurueck       Transaktionen    |
+------------------------------------+
|                                    |
|  Letzte Transaktionen (gecacht)    |
|                                    |
|  +--------------------------------+|
|  | [Cart] Suessigkeiten           ||
|  |        -2,50 EUR               ||
|  |        20.01.2026, 14:30       ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Plus] Taschengeld             ||
|  |        +10,00 EUR              ||
|  |        15.01.2026, 00:00       ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Cart] Spielzeug               ||
|  |        -5,00 EUR               ||
|  |        14.01.2026, 16:45       ||
|  +--------------------------------+|
|                                    |
|  Stand: 20.01.2026, 14:30 Uhr      |
|                                    |
+------------------------------------+
```

## Technische Notizen

- SQLite-Entity: `CachedTransaction`
- Maximale Cache-Groesse: 30 Transaktionen pro Benutzer
- Bei neuen Daten: Alte loeschen, neue speichern
- Service: `ITransactionCacheService`

## Implementierungshinweise

```csharp
// Datenbank-Entity
[Table("CachedTransactions")]
public class CachedTransaction
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal, etc.
    public DateTime CachedAt { get; set; }
}

// Cache Metadata
[Table("CacheMetadata")]
public class CacheMetadata
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

// Cache Service
public interface ITransactionCacheService
{
    Task<IReadOnlyList<CachedTransaction>> GetCachedTransactionsAsync(Guid userId);
    Task SetCachedTransactionsAsync(Guid userId, IEnumerable<TransactionDto> transactions);
    Task<DateTime?> GetLastCacheUpdateAsync(Guid userId);
}

public class TransactionCacheService : ITransactionCacheService
{
    private const int MaxCachedTransactions = 30;
    private readonly SQLiteAsyncConnection _db;

    public async Task<IReadOnlyList<CachedTransaction>> GetCachedTransactionsAsync(Guid userId)
    {
        return await _db.Table<CachedTransaction>()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(MaxCachedTransactions)
            .ToListAsync();
    }

    public async Task SetCachedTransactionsAsync(Guid userId, IEnumerable<TransactionDto> transactions)
    {
        // Alte Transaktionen loeschen
        await _db.Table<CachedTransaction>()
            .DeleteAsync(t => t.UserId == userId);

        // Neue cachen
        var cached = transactions.Take(MaxCachedTransactions).Select(t => new CachedTransaction
        {
            Id = t.Id,
            UserId = userId,
            Amount = t.Amount,
            Description = t.Description,
            Category = t.Category,
            TransactionDate = t.Date,
            TransactionType = t.Type,
            CachedAt = DateTime.UtcNow
        });

        await _db.InsertAllAsync(cached);

        // Metadata aktualisieren
        var key = $"transactions_{userId}";
        var metadata = await _db.Table<CacheMetadata>()
            .FirstOrDefaultAsync(m => m.Key == key);

        if (metadata == null)
        {
            metadata = new CacheMetadata { Key = key };
            await _db.InsertAsync(metadata);
        }
        metadata.LastUpdated = DateTime.UtcNow;
        await _db.UpdateAsync(metadata);
    }
}

// ViewModel Integration
public partial class TransactionsViewModel : BaseViewModel
{
    public ObservableCollection<TransactionViewModel> Transactions { get; } = new();

    [ObservableProperty]
    private bool _isFromCache;

    [ObservableProperty]
    private DateTime? _cacheTimestamp;

    [RelayCommand]
    private async Task LoadTransactionsAsync()
    {
        if (_connectivity.IsConnected)
        {
            var transactions = await _transactionService.GetTransactionsAsync();
            UpdateList(transactions);
            IsFromCache = false;

            await _transactionCache.SetCachedTransactionsAsync(
                _authService.UserId,
                transactions
            );
        }
        else
        {
            var cached = await _transactionCache.GetCachedTransactionsAsync(_authService.UserId);
            UpdateList(cached.Select(MapToDto));
            IsFromCache = true;
            CacheTimestamp = await _transactionCache.GetLastCacheUpdateAsync(_authService.UserId);
        }
    }
}
```

## API-Endpunkt

```
GET /api/transactions?limit=30
Authorization: Bearer {token}

Response 200:
{
  "transactions": [
    {
      "id": "guid",
      "amount": -2.50,
      "description": "Suessigkeiten",
      "category": "Essen",
      "type": "Withdrawal",
      "date": "2026-01-20T14:30:00Z"
    }
  ],
  "totalCount": 150
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Online: Transaktionen laden | API-Aufruf, Cache aktualisiert |
| TC-002 | Offline mit Cache | Gecachte Transaktionen angezeigt |
| TC-003 | Cache mit 50 Transaktionen | Nur 30 werden gespeichert |
| TC-004 | Neuer Abruf | Alte Cache-Daten werden ersetzt |
| TC-005 | Offline ohne Cache | Hinweis "Keine Daten" |

## Story Points

3

## Prioritaet

Hoch
