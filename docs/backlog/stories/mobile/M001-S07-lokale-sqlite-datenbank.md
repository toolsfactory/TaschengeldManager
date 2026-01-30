# Story M001-S07: Lokale SQLite-Datenbank

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **meine Daten auch offline verfügbar haben**, damit **ich die App ohne Internetverbindung nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die SQLite-Datenbank, wenn sie initialisiert wird, dann werden alle Tabellen erstellt
- [ ] Gegeben Daten, wenn sie gespeichert werden, dann sind sie nach App-Neustart verfügbar
- [ ] Gegeben der DatabaseService, wenn er verwendet wird, dann ist er thread-safe
- [ ] Gegeben Entitäten, wenn sie definiert werden, dann haben sie korrekte Primärschlüssel

## Technische Hinweise

### IDatabaseService Interface
```csharp
public interface IDatabaseService
{
    Task InitializeAsync();
    Task<List<T>> GetAllAsync<T>() where T : class, new();
    Task<T?> GetByIdAsync<T>(Guid id) where T : class, IEntity, new();
    Task<int> SaveAsync<T>(T entity) where T : class, new();
    Task<int> SaveAllAsync<T>(IEnumerable<T> entities) where T : class, new();
    Task<int> DeleteAsync<T>(T entity) where T : class, new();
    Task<int> DeleteAllAsync<T>() where T : class, new();
}
```

### DatabaseService Implementation
```csharp
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database != null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_database != null)
                return _database;

            var dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "taschengeld.db3");

            _database = new SQLiteAsyncConnection(dbPath);
            await InitializeAsync();

            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task InitializeAsync()
    {
        var db = await GetDatabaseAsync();

        await db.CreateTableAsync<CachedAccount>();
        await db.CreateTableAsync<CachedTransaction>();
        await db.CreateTableAsync<CachedCategory>();
        await db.CreateTableAsync<UserSettings>();
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class, new()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync<T>(Guid id) where T : class, IEntity, new()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<T>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> SaveAsync<T>(T entity) where T : class, new()
    {
        var db = await GetDatabaseAsync();
        return await db.InsertOrReplaceAsync(entity);
    }

    public async Task<int> SaveAllAsync<T>(IEnumerable<T> entities) where T : class, new()
    {
        var db = await GetDatabaseAsync();
        return await db.InsertAllAsync(entities, replace: true);
    }
}
```

### Lokale Entitäten
```csharp
public interface IEntity
{
    Guid Id { get; set; }
}

[Table("Accounts")]
public class CachedAccount : IEntity
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime LastSyncedAt { get; set; }
}

[Table("Transactions")]
public class CachedTransaction : IEntity
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public bool IsSynced { get; set; }
}

[Table("UserSettings")]
public class UserSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;
    public bool BiometricEnabled { get; set; }
    public bool DarkModeEnabled { get; set; }
    public string LastUserId { get; set; } = string.Empty;
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-22 | Datenbank initialisieren | Tabellen erstellt |
| TC-M001-23 | Entität speichern und laden | Daten korrekt |
| TC-M001-24 | Mehrere Entitäten speichern | Alle gespeichert |
| TC-M001-25 | Entität löschen | Nicht mehr auffindbar |

## Story Points
3

## Priorität
Hoch
