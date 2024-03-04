using System;

namespace PasswordSecure.Model;

public class AccountEntry
{
	public AccountEntry(string name)
	{
		Name = name;
	}
	
	public string Name { get; }
	
	public string? Website { get; set; }
	
	public string? User { get; set; }
	public string? Password { get; set; }
	
	public DateTime? DateAdded { get; set; }
	public DateTime? DateChanged { get; set; }
}
