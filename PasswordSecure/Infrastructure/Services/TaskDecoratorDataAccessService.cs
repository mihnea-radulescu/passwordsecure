using System.Threading.Tasks;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class TaskDecoratorDataAccessService : IDataAccessService
{
	public TaskDecoratorDataAccessService(IDataAccessService dataAccessService)
	{
		_dataAccessService = dataAccessService;
	}
	
	public async Task<AccountEntryCollection> ReadAccountEntries(AccessParams accessParams)
	{
		var readAccountEntriesTask = Task.Run(
			() => _dataAccessService.ReadAccountEntries(accessParams));

		var accountEntries = await readAccountEntriesTask;
		return accountEntries;
	}

	public async Task SaveAccountEntries(
		AccessParams accessParams,
		AccountEntryCollection accountEntryCollection,
		bool isV1Vault)
	{
		var saveAccountEntriesTask = Task.Run(
			() => _dataAccessService.SaveAccountEntries(
				accessParams, accountEntryCollection, isV1Vault));

		await saveAccountEntriesTask;
	}
	
	#region Private
	
	private readonly IDataAccessService _dataAccessService;
	
	#endregion
}
