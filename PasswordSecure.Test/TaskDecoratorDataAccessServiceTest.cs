using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Providers;
using PasswordSecure.Infrastructure.Services;
using Xunit;

namespace PasswordSecure.Test;

public class TaskDecoratorDataAccessServiceTest
{
    public TaskDecoratorDataAccessServiceTest()
    {
        IFileAccessProvider fileAccessProvider = new FileAccessProvider();
        IDateTimeProvider dateTimeProvider = new CurrentDateTimeProvider();

        IDataSerializationService dataSerializationService = new JsonDataSerializationService();
        var v1dataEncryptionService = new Infrastructure.Services.V1.AesDataEncryptionService();
        var v2dataEncryptionService = new Infrastructure.Services.V2.AesDataEncryptionService();
        IBackupService backupService = new BackupService(fileAccessProvider, dateTimeProvider);

        IDataAccessService dataAccessService = new DataAccessService(
            fileAccessProvider,
            dataSerializationService,
            v1dataEncryptionService,
            v2dataEncryptionService,
            backupService
        );

        _taskDecoratorDataAccessService = new TaskDecoratorDataAccessService(dataAccessService);
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_NoElementCollection_ReturnsInitialData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_NoElementCollection.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        var accountEntryCollectionReference = new AccountEntryCollection();

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        // Assert
        accountEntryCollection.Should().NotBeNull();
        accountEntryCollection.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_OneElementCollectionDefaultValues_ReturnsInitialData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_OneElementCollectionDefaultValues.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        const string name = "Google";

        var accountEntry = new AccountEntry { Name = name };

        var accountEntryCollectionReference = new AccountEntryCollection { accountEntry };

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        // Assert
        accountEntryCollection.Should().NotBeNull();
        accountEntryCollection.Count.Should().Be(1);

        var singleAccountEntry = accountEntryCollection[0];

        singleAccountEntry.Should().NotBeNull();
        singleAccountEntry.Name.Should().Be(name);
        singleAccountEntry.Url.Should().BeEmpty();
        singleAccountEntry.User.Should().BeEmpty();
        singleAccountEntry.Password.Should().BeNull();
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_OneElementCollectionCustomValues_ReturnsInitialData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_OneElementCollectionCustomValues.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        const string name = "Google";
        const string url = "https://mail.google.com";
        const string user = "john.doe";
        const string password = "123456**&&";

        var accountEntry = new AccountEntry
        {
            Name = name,
            Url = url,
            User = user,
            Password = password,
        };

        var accountEntryCollectionReference = new AccountEntryCollection { accountEntry };

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        // Assert
        accountEntryCollection.Should().NotBeNull();
        accountEntryCollection.Count.Should().Be(1);

        var singleAccountEntry = accountEntryCollection[0];

        singleAccountEntry.Should().NotBeNull();
        singleAccountEntry.Name.Should().Be(name);
        singleAccountEntry.Url.Should().Be(url);
        singleAccountEntry.User.Should().Be(user);
        singleAccountEntry.Password.Should().Be(password);
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValues_ReturnsInitialData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        const string name1 = "Google";
        const string url1 = "https://mail.google.com";
        const string user1 = "john.doe";
        const string password1 = "123456**&&";

        var accountEntry1 = new AccountEntry
        {
            Name = name1,
            Url = url1,
            User = user1,
            Password = password1,
        };

        const string name2 = "Microsoft";
        const string url2 = "https://azure.microsoft.com";
        const string user2 = "john_doe";
        const string password2 = "654321&&**";

        var accountEntry2 = new AccountEntry
        {
            Name = name2,
            Url = url2,
            User = user2,
            Password = password2,
        };

        var accountEntryCollectionReference = new AccountEntryCollection
        {
            accountEntry1,
            accountEntry2,
        };

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        // Assert
        accountEntryCollection.Should().NotBeNull();
        accountEntryCollection.Count.Should().Be(2);

        var firstAccountEntry = accountEntryCollection[0];

        firstAccountEntry.Should().NotBeNull();
        firstAccountEntry.Name.Should().Be(name1);
        firstAccountEntry.Url.Should().Be(url1);
        firstAccountEntry.User.Should().Be(user1);
        firstAccountEntry.Password.Should().Be(password1);

        var secondAccountEntry = accountEntryCollection[1];

        secondAccountEntry.Should().NotBeNull();
        secondAccountEntry.Name.Should().Be(name2);
        secondAccountEntry.Url.Should().Be(url2);
        secondAccountEntry.User.Should().Be(user2);
        secondAccountEntry.Password.Should().Be(password2);
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValuesOneEntryModified_ReturnsChangedData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        const string name1 = "Google";
        const string url1 = "https://mail.google.com";
        const string user1 = "john.doe";
        const string password1 = "123456**&&";

        var accountEntry1 = new AccountEntry
        {
            Name = name1,
            Url = url1,
            User = user1,
            Password = password1,
        };

        const string name2 = "Microsoft";
        const string url2 = "https://azure.microsoft.com";
        const string user2 = "john_doe";
        const string password2 = "654321&&**";

        var accountEntry2 = new AccountEntry
        {
            Name = name2,
            Url = url2,
            User = user2,
            Password = password2,
        };

        const string name1Changed = "Google Mail";
        const string url1Changed = "https://mail.google.com/";
        const string user1Changed = "jane.doe";
        const string password1Changed = "123456**&&~~";

        var accountEntryCollectionReference = new AccountEntryCollection
        {
            accountEntry1,
            accountEntry2,
        };

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        accountEntryCollection[0].Name = name1Changed;
        accountEntryCollection[0].Url = url1Changed;
        accountEntryCollection[0].User = user1Changed;
        accountEntryCollection[0].Password = password1Changed;

        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollection
        );
        var accountEntryCollectionChanged =
            await _taskDecoratorDataAccessService.ReadAccountEntries(accessParams);

        // Assert
        accountEntryCollectionChanged.Should().NotBeNull();
        accountEntryCollectionChanged.Count.Should().Be(2);

        var firstAccountEntry = accountEntryCollectionChanged[0];

        firstAccountEntry.Should().NotBeNull();
        firstAccountEntry.Name.Should().Be(name1Changed);
        firstAccountEntry.Url.Should().Be(url1Changed);
        firstAccountEntry.User.Should().Be(user1Changed);
        firstAccountEntry.Password.Should().Be(password1Changed);

        var secondAccountEntry = accountEntryCollectionChanged[1];

        secondAccountEntry.Should().NotBeNull();
        secondAccountEntry.Name.Should().Be(name2);
        secondAccountEntry.Url.Should().Be(url2);
        secondAccountEntry.User.Should().Be(user2);
        secondAccountEntry.Password.Should().Be(password2);
    }

    [Fact]
    public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValuesOneEntryReplaced_ReturnsChangedData()
    {
        // Arrange
        const string fileName = "MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
        var filePath = GetFilePath(fileName);

        var accessParams = new AccessParams { FilePath = filePath, Password = Password };

        const string name1 = "Google";
        const string url1 = "https://mail.google.com";
        const string user1 = "john.doe";
        const string password1 = "123456**&&";

        var accountEntry1 = new AccountEntry
        {
            Name = name1,
            Url = url1,
            User = user1,
            Password = password1,
        };

        const string name2 = "Microsoft";
        const string url2 = "https://azure.microsoft.com";
        const string user2 = "john_doe";
        const string password2 = "654321&&**";

        var accountEntry2 = new AccountEntry
        {
            Name = name2,
            Url = url2,
            User = user2,
            Password = password2,
        };

        const string name1Changed = "Google Mail";
        const string url1Changed = "https://mail.google.com/";
        const string user1Changed = "jane.doe";
        const string password1Changed = "123456**&&~~";

        var accountEntry1Changed = new AccountEntry
        {
            Name = name1Changed,
            Url = url1Changed,
            User = user1Changed,
            Password = password1Changed,
        };

        var accountEntryCollectionReference = new AccountEntryCollection
        {
            accountEntry1,
            accountEntry2,
        };

        // Act
        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollectionReference
        );
        var accountEntryCollection = await _taskDecoratorDataAccessService.ReadAccountEntries(
            accessParams
        );

        accountEntryCollection[0] = accountEntry1Changed;

        await _taskDecoratorDataAccessService.SaveAccountEntries(
            accessParams,
            accountEntryCollection
        );
        var accountEntryCollectionChanged =
            await _taskDecoratorDataAccessService.ReadAccountEntries(accessParams);

        // Assert
        accountEntryCollectionChanged.Should().NotBeNull();
        accountEntryCollectionChanged.Count.Should().Be(2);

        var firstAccountEntry = accountEntryCollectionChanged[0];

        firstAccountEntry.Should().NotBeNull();
        firstAccountEntry.Name.Should().Be(name1Changed);
        firstAccountEntry.Url.Should().Be(url1Changed);
        firstAccountEntry.User.Should().Be(user1Changed);
        firstAccountEntry.Password.Should().Be(password1Changed);

        var secondAccountEntry = accountEntryCollectionChanged[1];

        secondAccountEntry.Should().NotBeNull();
        secondAccountEntry.Name.Should().Be(name2);
        secondAccountEntry.Url.Should().Be(url2);
        secondAccountEntry.User.Should().Be(user2);
        secondAccountEntry.Password.Should().Be(password2);
    }

    #region Private

    private const string Password = "Master Password";

    private readonly TaskDecoratorDataAccessService _taskDecoratorDataAccessService;

    private static string GetFilePath(string fileName) => Path.GetFullPath(fileName);

    #endregion
}
