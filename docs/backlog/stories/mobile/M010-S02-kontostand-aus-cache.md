# Story M010-S02: Kontostand aus Cache laden

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Kind** moechte ich **meinen Kontostand auch offline sehen koennen**, damit **ich auch ohne Internet weiss, wie viel Geld ich habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein vorher abgerufener Kontostand, wenn der Benutzer offline ist, dann wird der gecachte Kontostand angezeigt
- [ ] Gegeben ein gecachter Kontostand, wenn er angezeigt wird, dann ist der Zeitpunkt der letzten Aktualisierung sichtbar
- [ ] Gegeben Online-Status, wenn der Kontostand abgerufen wird, dann wird er im Cache aktualisiert
- [ ] Gegeben kein gecachter Kontostand, wenn der Benutzer offline ist, dann wird eine entsprechende Meldung angezeigt

## UI-Entwurf

### Gecachter Kontostand
```
+------------------------------------+
| [!] Offline - Daten evtl. veraltet |
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
|                                    |
|  Hallo, Max!                       |
|                                    |
|  Kontostand                        |
|  (zuletzt aktualisiert: 14:30)     |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
|                                    |
|  ...                               |
+------------------------------------+
```

### Kein Cache vorhanden
```
+------------------------------------+
| [!] Offline - Daten evtl. veraltet |
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
|                                    |
|  Hallo, Max!                       |
|                                    |
|  Kontostand                        |
|  +--------------------------------+|
|  | [Cloud-Offline]               ||
|  | Keine Daten verfuegbar.        ||
|  | Bitte verbinde dich mit dem    ||
|  | Internet.                      ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Notizen

- Lokale Datenbank: SQLite mit sqlite-net-pcl
- Cache-Entity: `CachedBalance` mit UserId, Amount, LastUpdated
- Cache-Dauer: 24 Stunden (auch wenn aelter, wird angezeigt)
- Service: `IBalanceCacheService`

## Implementierungshinweise

```csharp
// Datenbank-Entity
[Table("CachedBalances")]
public class CachedBalance
{
    [PrimaryKey]
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Cache Service
public interface IBalanceCacheService
{
    Task<CachedBalance?> GetCachedBalanceAsync(Guid userId);
    Task SetCachedBalanceAsync(Guid userId, decimal amount);
}

public class BalanceCacheService : IBalanceCacheService
{
    private readonly SQLiteAsyncConnection _db;

    public async Task<CachedBalance?> GetCachedBalanceAsync(Guid userId)
    {
        return await _db.Table<CachedBalance>()
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task SetCachedBalanceAsync(Guid userId, decimal amount)
    {
        var cached = await GetCachedBalanceAsync(userId);
        if (cached == null)
        {
            cached = new CachedBalance { UserId = userId };
            await _db.InsertAsync(cached);
        }

        cached.Amount = amount;
        cached.LastUpdated = DateTime.UtcNow;
        await _db.UpdateAsync(cached);
    }
}

// ViewModel Integration
public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private DateTime? _lastUpdated;

    [ObservableProperty]
    private bool _isFromCache;

    [RelayCommand]
    private async Task LoadBalanceAsync()
    {
        if (_connectivity.IsConnected)
        {
            var balance = await _accountService.GetBalanceAsync();
            Balance = balance.Amount;
            LastUpdated = DateTime.Now;
            IsFromCache = false;

            // Cache aktualisieren
            await _balanceCache.SetCachedBalanceAsync(_authService.UserId, balance.Amount);
        }
        else
        {
            var cached = await _balanceCache.GetCachedBalanceAsync(_authService.UserId);
            if (cached != null)
            {
                Balance = cached.Amount;
                LastUpdated = cached.LastUpdated;
                IsFromCache = true;
            }
            else
            {
                // Keine Daten verfuegbar
                HasCachedData = false;
            }
        }
    }
}
```

## API-Endpunkt

```
GET /api/accounts/balance
Authorization: Bearer {token}

Response 200:
{
  "balance": 150.00,
  "currency": "EUR",
  "lastTransaction": "2026-01-20T10:30:00Z"
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Online: Kontostand laden | API-Aufruf, Cache aktualisiert |
| TC-002 | Offline mit Cache | Gecachter Wert wird angezeigt |
| TC-003 | Offline ohne Cache | Hinweis "Keine Daten" |
| TC-004 | Cache 2 Stunden alt | Zeitstempel wird angezeigt |
| TC-005 | Online nach Offline | Neuer Wert ersetzt Cache |

## Story Points

2

## Prioritaet

Hoch
