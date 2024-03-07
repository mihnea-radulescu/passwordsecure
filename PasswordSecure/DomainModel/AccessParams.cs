namespace PasswordSecure.DomainModel;

public record AccessParams : IPasswordContainer
{
	public string? FilePath { get; set; }
	
	public string? Password { get; set; }
	
	public bool ShouldSaveBackup { get; set; }
}
