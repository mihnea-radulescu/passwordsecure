using System.Text;

namespace PasswordSecure.Application.Extensions;

public static class ByteArrayExtensions
{
	private static readonly Encoding Encoding = Encoding.UTF8;

	extension(byte[] bytes)
	{
		public string ToText() => Encoding.GetString(bytes);
	}
}
