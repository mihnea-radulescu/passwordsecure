using System.Text;

namespace PasswordSecure.Application.Extensions;

public static class ByteArrayExtensions
{
	public static string ToText(this byte[] bytes) => Encoding.GetString(bytes);
	
	#region Private
	
	private static readonly Encoding Encoding = Encoding.UTF8;
	
	#endregion
}
