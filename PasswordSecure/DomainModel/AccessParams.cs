namespace PasswordSecure.DomainModel;

public record AccessParams : IPasswordEntry
{
	public string? FilePath { get; set; }
	
	public string? Password { get; set; }
}
