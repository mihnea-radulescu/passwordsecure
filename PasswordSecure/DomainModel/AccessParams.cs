namespace PasswordSecure.DomainModel;

public record AccessParams : IPasswordEntry
{
    public VaultVersion VaultVersion { get; set; }
    public bool IsNewContainer { get; set; }
    
    public string? FilePath { get; set; }

    public string? Password { get; set; }

    public byte[]? Salt { get; set; }
}
