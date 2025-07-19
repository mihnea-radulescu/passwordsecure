namespace PasswordSecure.DomainModel;

public record Vault(VaultHeader Header, byte[] Body);
