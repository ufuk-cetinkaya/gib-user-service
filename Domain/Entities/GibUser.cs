using Domain.Enums;

namespace Domain.Entities;

public class GibUser
{
    public int Id { get; private set; }
    public string Identifier { get; set; } = null!;
    public string Title { get; set; } = null!;
    public UsrType UserType { get; set; }
    public AccType AccountType { get; set; }
    public string FirstCreationTime { get; set; } = null!;
    public DocType DocumentType { get; set; }
    public Unit Unit { get; set; }
    public string Alias { get; set; } = null!;
    public string AliasCreationTime { get; set; } = null!;
    public string? AliasDeletionTime { get; set; }
    public bool IsActive { get; set; }
}