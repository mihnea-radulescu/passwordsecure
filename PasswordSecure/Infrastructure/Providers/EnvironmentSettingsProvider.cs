using System;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class EnvironmentSettingsProvider : IEnvironmentSettingsProvider
{
	public bool IsInsideFlatpakContainer
		=> Environment.GetEnvironmentVariable(FlatpakEnvironmentVariable) == FlatpakAppId;

	private const string FlatpakEnvironmentVariable = "FLATPAK_ID";

	private const string FlatpakAppId = "io.github.mihnea_radulescu.passwordsecure";
}
