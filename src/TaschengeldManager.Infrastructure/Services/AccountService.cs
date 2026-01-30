using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IUnitOfWork unitOfWork,
        ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AccountDto?> GetAccountAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            return null;
        }

        // Check access
        if (!await HasAccountAccessAsync(userId, account, cancellationToken))
        {
            return null;
        }

        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);

        return new AccountDto
        {
            Id = account.Id.Value,
            UserId = account.UserId.Value,
            OwnerName = owner?.Nickname ?? "Unknown",
            Balance = account.Balance,
            InterestEnabled = account.InterestEnabled,
            InterestRate = account.InterestRate,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<AccountDto?> GetMyAccountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Role != UserRole.Child)
        {
            return null;
        }

        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            return null;
        }

        return new AccountDto
        {
            Id = account.Id.Value,
            UserId = account.UserId.Value,
            OwnerName = user.Nickname,
            Balance = account.Balance,
            InterestEnabled = account.InterestEnabled,
            InterestRate = account.InterestRate,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<IReadOnlyList<AccountDto>> GetFamilyAccountsAsync(UserId parentUserId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            return [];
        }

        var accounts = await _unitOfWork.Accounts.GetByFamilyAsync(familyId, cancellationToken);

        return accounts.Select(a => new AccountDto
        {
            Id = a.Id.Value,
            UserId = a.UserId.Value,
            OwnerName = a.User?.Nickname ?? "Unknown",
            Balance = a.Balance,
            InterestEnabled = a.InterestEnabled,
            InterestRate = a.InterestRate,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<TransactionDto> DepositAsync(UserId parentUserId, AccountId accountId, DepositRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        // Verify parent has access
        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
        {
            throw new InvalidOperationException("Account owner not in a family");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can deposit to this account");
        }

        var parent = await _unitOfWork.Users.GetByIdAsync(parentUserId, cancellationToken);

        // Create transaction
        account.Balance += request.Amount;

        var transaction = new Transaction
        {
            Id = new TransactionId(Guid.NewGuid()),
            AccountId = accountId,
            Amount = request.Amount,
            Type = TransactionType.Deposit,
            Description = request.Description,
            CreatedByUserId = parentUserId,
            BalanceAfter = account.Balance
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deposit of {Amount} to account {AccountId} by parent {ParentId}", request.Amount, accountId.Value, parentUserId.Value);

        return new TransactionDto
        {
            Id = transaction.Id.Value,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Category = transaction.Category,
            CreatedByName = parent?.Nickname ?? "Unknown",
            BalanceAfter = transaction.BalanceAfter,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<TransactionDto> WithdrawAsync(UserId userId, WithdrawRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Role != UserRole.Child)
        {
            throw new InvalidOperationException("Only children can withdraw from their own accounts");
        }

        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        if (account.Balance < request.Amount)
        {
            throw new InvalidOperationException("Insufficient balance");
        }

        // Create transaction
        account.Balance -= request.Amount;

        var transaction = new Transaction
        {
            Id = new TransactionId(Guid.NewGuid()),
            AccountId = account.Id,
            Amount = -request.Amount,
            Type = TransactionType.Withdrawal,
            Description = request.Description,
            Category = request.Category,
            CreatedByUserId = userId,
            BalanceAfter = account.Balance
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Withdrawal of {Amount} from account {AccountId} by child {ChildId}", request.Amount, account.Id.Value, userId.Value);

        return new TransactionDto
        {
            Id = transaction.Id.Value,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Category = transaction.Category,
            CreatedByName = user.Nickname,
            BalanceAfter = transaction.BalanceAfter,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<TransactionDto> GiftAsync(UserId giftGiverUserId, GiftRequest request, CancellationToken cancellationToken = default)
    {
        var giftGiver = await _unitOfWork.Users.GetByIdAsync(giftGiverUserId, cancellationToken);
        if (giftGiver == null || (giftGiver.Role != UserRole.Relative && giftGiver.Role != UserRole.Parent))
        {
            throw new UnauthorizedAccessException("Only parents and relatives can give gifts");
        }

        var accountId = new AccountId(request.AccountId);
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        // Verify gift giver has access to this family
        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
        {
            throw new InvalidOperationException("Account owner not in a family");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        // Check family membership based on role
        var hasAccess = giftGiver.Role == UserRole.Parent
            ? family.Parents.Any(p => p.UserId == giftGiverUserId)
            : family.Relatives.Any(r => r.UserId == giftGiverUserId);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("You don't have access to this family");
        }

        // Create transaction
        account.Balance += request.Amount;

        var transaction = new Transaction
        {
            Id = new TransactionId(Guid.NewGuid()),
            AccountId = accountId,
            Amount = request.Amount,
            Type = TransactionType.Gift,
            Description = request.Description,
            CreatedByUserId = giftGiverUserId,
            BalanceAfter = account.Balance
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Gift of {Amount} to account {AccountId} by {Role} {UserId}",
            request.Amount, accountId.Value, giftGiver.Role, giftGiverUserId.Value);

        return new TransactionDto
        {
            Id = transaction.Id.Value,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Category = transaction.Category,
            CreatedByName = giftGiver.Nickname,
            BalanceAfter = transaction.BalanceAfter,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(UserId userId, AccountId accountId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            return [];
        }

        // Check access
        if (!await HasAccountAccessAsync(userId, account, cancellationToken))
        {
            return [];
        }

        var transactions = await _unitOfWork.Transactions.GetByAccountAsync(accountId, limit ?? 50, offset ?? 0, cancellationToken);

        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id.Value,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description,
            Category = t.Category,
            CreatedByName = t.CreatedByUser?.Nickname ?? "Unknown",
            BalanceAfter = t.BalanceAfter,
            CreatedAt = t.CreatedAt
        }).ToList();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetMyTransactionsAsync(UserId userId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)
    {
        // Get user's account
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            return [];
        }

        var transactions = await _unitOfWork.Transactions.GetByAccountAsync(account.Id, limit ?? 50, offset ?? 0, cancellationToken);

        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id.Value,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description,
            Category = t.Category,
            CreatedByName = t.CreatedByUser?.Nickname ?? "Unknown",
            BalanceAfter = t.BalanceAfter,
            CreatedAt = t.CreatedAt
        }).ToList();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetMyGiftsAsync(UserId relativeUserId, CancellationToken cancellationToken = default)
    {
        var relative = await _unitOfWork.Users.GetByIdAsync(relativeUserId, cancellationToken);
        if (relative == null || relative.Role != UserRole.Relative)
        {
            return [];
        }

        var transactions = await _unitOfWork.Transactions.GetByCreatorAsync(relativeUserId, cancellationToken);

        return transactions
            .Where(t => t.Type == TransactionType.Gift)
            .Select(t => new TransactionDto
            {
                Id = t.Id.Value,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                Category = t.Category,
                CreatedByName = relative.Nickname,
                BalanceAfter = t.BalanceAfter,
                CreatedAt = t.CreatedAt
            }).ToList();
    }

    public async Task<AccountDto> SetInterestAsync(UserId parentUserId, AccountId accountId, SetInterestRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        // Verify parent has access
        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
        {
            throw new InvalidOperationException("Account owner not in a family");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can change interest settings");
        }

        account.InterestEnabled = request.Enabled;
        account.InterestRate = request.Enabled ? request.InterestRate : null;

        if (request.Enabled && account.LastInterestCalculation == null)
        {
            account.LastInterestCalculation = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Interest settings updated for account {AccountId} by parent {ParentId}", accountId.Value, parentUserId.Value);

        return new AccountDto
        {
            Id = account.Id.Value,
            UserId = account.UserId.Value,
            OwnerName = owner.Nickname,
            Balance = account.Balance,
            InterestEnabled = account.InterestEnabled,
            InterestRate = account.InterestRate,
            CreatedAt = account.CreatedAt
        };
    }

    private async Task<bool> HasAccountAccessAsync(UserId userId, Account account, CancellationToken cancellationToken)
    {
        // Owner has access
        if (account.UserId == userId)
        {
            return true;
        }

        // Get account owner's family
        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
        {
            return false;
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null)
        {
            return false;
        }

        // Parents have access
        if (family.Parents.Any(p => p.UserId == userId))
        {
            return true;
        }

        // Relatives have limited access (only to view, not full access)
        if (family.Relatives.Any(r => r.UserId == userId))
        {
            return true;
        }

        return false;
    }
}
