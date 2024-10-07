using PasswordSecure.Application.Services;

namespace PasswordSecure.Application.Providers;

public interface IDataAccessServiceProvider
{
	IDataAccessService CreateDataAccessService();
}
