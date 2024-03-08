namespace PasswordSecure.DomainModel;

public record AccessParams
{
	public string? FilePath { get; set; }
	
	public string? Password { get; set; }
}
