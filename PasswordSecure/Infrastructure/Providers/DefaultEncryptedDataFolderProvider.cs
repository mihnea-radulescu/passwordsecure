using System;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class DefaultEncryptedDataFolderProvider : IEncryptedDataFolderProvider
{
	public string GetEncryptedDataFolderPath()
		=> Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}
