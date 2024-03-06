namespace PasswordSecure.DomainModel;

public record AccessParams
{
	public string? MasterPassword { get; set; }

	public string? FilePath { get; set; }
	
	public bool ShouldSaveBackup { get; set; }
}
