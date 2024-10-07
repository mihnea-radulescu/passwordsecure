using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class AesV1DataEncryptionService : DataEncryptionServiceBase
{
    #region Protected

    protected override VaultVersion VaultVersion => VaultVersion.V1;

    protected override int PasswordIterations => 16;

    protected override byte[] GenerateIv() => Iv;
    protected override byte[] GenerateSalt() => Salt;

	protected override byte[] GetIvFromVault(Vault vault) => Iv;
	protected override byte[] GetSaltFromVault(Vault vault) => Salt;

    #endregion

	#region Private

	private static readonly byte[] Iv =
	{
		0x16,
		0x15,
		0x14,
		0x13,
		0x12,
		0x11,
		0x10,
		0x09,
		0x08,
		0x07,
		0x06,
		0x05,
		0x04,
		0x03,
		0x02,
		0x01,
	};

	private static readonly byte[] Salt =
	{
		0x01,
		0x02,
		0x03,
		0x04,
		0x05,
		0x06,
		0x07,
		0x08,
		0x09,
		0x10,
		0x11,
		0x12,
		0x13,
		0x14,
		0x15,
		0x16,
	};

	#endregion
}
