namespace PasswordSecure.DomainModel;

public record AccountEntry : IPasswordEntry
{
	public string Name { get; set; } = "New Entry";
	public string Url { get; set; } = string.Empty;
	public string User { get; set; } = string.Empty;
	
	public string? Password { get; set; }
	
	public string Notes { get; set; } = string.Empty;
}
