using System.Security.Cryptography;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class AesV2DataEncryptionService : DataEncryptionServiceBase
{
	#region Protected

	protected override VaultVersion VaultVersion => VaultVersion.V2;

	// See: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2
    protected override int PasswordIterations => 600_000;

	protected override byte[] GenerateIv() => RandomNumberGenerator.GetBytes(IvSizeInBytes);
    protected override byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSizeInBytes);

	protected override byte[] GetIvFromVault(Vault vault) => vault.Header.IV;
	protected override byte[] GetSaltFromVault(Vault vault) => vault.Header.Salt;

    #endregion

	#region Private

	private const int IvSizeInBytes = 16;
	private const int SaltSizeInBytes = 16;

	#endregion
}
