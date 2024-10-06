namespace PasswordSecure.DomainModel;

public class Vault
{
    public VaultHeader Header { get; set; }
    public byte[] Body { get; set; }
}


public class VaultHeader
{
    public VaultVersion Version { get; set; }
    public byte[] Salt { get; set; }
    public byte[] IV { get; set; }
}

public enum VaultVersion
{
    // Insecure due to weak key derivation and hard-coded IV
    V1 = 1,
    // Fixes hardcoded salt and IV.
    // Contains Vault wrapper around encrypted AccountEntryCollection
    V2 = 2,
}