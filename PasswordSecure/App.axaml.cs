//#define FLATPAK_BUILD

using Avalonia.Markup.Xaml;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.Infrastructure.Providers;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Presentation;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure;

public class App : Avalonia.Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		IDataSerializationService jsonDataSerializationService = new JsonDataSerializationService();

		IDataEncryptionService dataEncryptionService = new DataEncryptionService();

		IFileAccessProvider fileAccessProvider = new FileAccessProvider();
		IDateTimeProvider currentDateTimeProvider = new CurrentDateTimeProvider();
		IBackupService backupService = new BackupService(fileAccessProvider, currentDateTimeProvider);

		IDataAccessService dataAccessService = new DataAccessService(
			fileAccessProvider,
			jsonDataSerializationService,
			dataEncryptionService,
			backupService);

		IDataAccessService dataAccessServiceDecorated = new TaskDecoratorDataAccessService(
			dataAccessService);

		IAssemblyVersionProvider assemblyVersionProvider = new AssemblyVersionProvider();

		IEncryptedDataFolderProvider encryptedDataFolderProvider;
#if FLATPAK_BUILD
		encryptedDataFolderProvider = new FlatpakEncryptedDataFolderProvider();
#else
		encryptedDataFolderProvider = new DefaultEncryptedDataFolderProvider();
#endif

		var mainWindow = new MainWindow();

		var mainPresenter = new MainPresenter(
			dataAccessServiceDecorated,
			assemblyVersionProvider,
			encryptedDataFolderProvider,
			mainWindow);

		mainWindow.Show();
	}
}
