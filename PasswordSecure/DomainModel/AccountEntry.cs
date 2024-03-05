namespace PasswordSecure.DomainModel;

public record AccountEntry
{
	public AccountEntry()
	{
		Name = "New Entry";
	}
	
	public string Name { get; set; }
	
	public string? Website { get; set; }
	public string? User { get; set; }
	public string? Password { get; set; }
}
