using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Providers;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Test;

public class DataAccessServiceTest
{
	public DataAccessServiceTest()
	{
		IDataAccessServiceProvider dataAccessServiceProvider = new DataAccessServiceProvider();
		
		_dataAccessService = dataAccessServiceProvider.CreateDataAccessService();
	}

	[Fact]
	public async Task UpgradeFromV1EntriestoV2Entries_ReturnsInitialData()
	{
		// Arrange
		var entries = new AccountEntryCollection([
			new AccountEntry
			{
				Name = "Example",
				Url = "http://example.com",
				User = "JoeDoe",
				Password = "test1234!",
			}
		]);
		var data = JsonSerializer.Serialize(entries).ToByteArray();

		var aesV1DataEncryptionService = new AesV1DataEncryptionService();
		var accessParams = new AccessParams
		{
			FilePath = "upgrade.encrypted",
			Password = "Password",
		};
		var vault = aesV1DataEncryptionService.EncryptDataToVault(data, accessParams.Password);

		var fileAccessProvider = new FileAccessProvider();
		fileAccessProvider.SaveData(accessParams.FilePath!, vault.Body);

		// Act
		var v1Entries = await _dataAccessService.ReadAccountEntries(accessParams);
		await _dataAccessService.SaveAccountEntries(accessParams, v1Entries, true);
		var v2Entries = await _dataAccessService.ReadAccountEntries(accessParams);

		// Assert
		Assert.Single(v1Entries);
		Assert.Equal(entries[0], v1Entries[0]);

		Assert.Single(v2Entries);
		Assert.Equal(entries[0], v2Entries[0]);
	}

	#region Private

	private readonly IDataAccessService _dataAccessService;

	#endregion
}
