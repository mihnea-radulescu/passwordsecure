namespace PasswordSecure.Application.Providers;

public interface IEnvironmentSettingsProvider
{
	bool IsInsideFlatpakContainer { get; }
}
