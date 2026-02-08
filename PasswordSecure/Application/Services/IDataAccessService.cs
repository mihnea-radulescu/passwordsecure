using System.Threading.Tasks;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataAccessService
{
	Task<AccountEntryCollection> ReadAccountEntries(AccessParams accessParams);

	Task SaveAccountEntries(
		AccessParams accessParams,
		AccountEntryCollection accountEntryCollection);
}
