using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Providers;
using Xunit;

namespace PasswordSecure.Test;

public class DataAccessServiceTest
{
    private readonly IDataAccessService _dataAccessService;

    public DataAccessServiceTest()
    {
        _dataAccessService = DataAccessServiceProvider.Create();
    }

    [Fact]
    public async void UpgradeFromV1()
    {
        // Arrange
        var entries = new AccountEntryCollection(
            new List<AccountEntry>()
            {
                new AccountEntry
                {
                    Name = "Example",
                    Url = "http://example.com",
                    User = "JoeDoe",
                    Password = "test1234!",
                },
            }
        );
        var data = JsonSerializer.Serialize(entries).ToByteArray();

        var v1Encryption = new Infrastructure.Services.V1.AesDataEncryptionService();
        var accessParams = new AccessParams()
        {
            FilePath = "upgrade.encrypted",
            Password = "Password",
        };
        var encryptedData = v1Encryption.EncryptData(data, accessParams.Password);
        new FileAccessProvider().SaveData(accessParams.FilePath!, encryptedData);

        // Act
        {
            var entriesFromV1 = await _dataAccessService.ReadAccountEntries(accessParams);
            await _dataAccessService.SaveAccountEntries(accessParams, entriesFromV1);
        }
        var upgradedEntries = await _dataAccessService.ReadAccountEntries(accessParams);

        // Assert
        upgradedEntries.Should().HaveCount(1);
    }
}
