namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Family information for display.
/// </summary>
public record FamilyDto
{
    /// <summary>
    /// Family ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Family name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Family code for child login.
    /// </summary>
    public required string FamilyCode { get; init; }

    /// <summary>
    /// When the family was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Parents in the family.
    /// </summary>
    public IReadOnlyList<FamilyMemberDto> Parents { get; init; } = [];

    /// <summary>
    /// Relatives in the family.
    /// </summary>
    public IReadOnlyList<FamilyMemberDto> Relatives { get; init; } = [];

    /// <summary>
    /// Children in the family.
    /// </summary>
    public IReadOnlyList<ChildDto> Children { get; init; } = [];
}
