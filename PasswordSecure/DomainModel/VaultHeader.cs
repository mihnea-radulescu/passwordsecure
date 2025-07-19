namespace PasswordSecure.DomainModel;

public record VaultHeader(byte[] IV, byte[] Salt);
