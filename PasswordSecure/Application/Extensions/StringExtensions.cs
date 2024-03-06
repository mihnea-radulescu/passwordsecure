using System.Text;

namespace PasswordSecure.Application.Extensions;

public static class StringExtensions
{
	public static byte[] ToByteArray(this string s)
		=> Encoding.GetBytes(s);
	
	#region Private
	
	private static readonly Encoding Encoding = Encoding.UTF8;
	
	#endregion
}
