namespace PasswordSecure.DomainModel;

public record Vault(VaultHeader Header, byte[] Body);

public record VaultHeader(VaultVersion Version, byte[] IV, byte[] Salt);

public enum VaultVersion
{
	NotSet = 0,
	
	// Insecure, due to weak key derivation and hard-coded IV.
	V1 = 1,
	
	// Fixes hardcoded salt and IV.
	// Contains Vault wrapper around encrypted AccountEntryCollection.
	V2 = 2
}
