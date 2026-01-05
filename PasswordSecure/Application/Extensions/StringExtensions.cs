using System.Text;

namespace PasswordSecure.Application.Extensions;

public static class StringExtensions
{
	private static readonly Encoding Encoding = Encoding.UTF8;

	extension(string text)
	{
		public byte[] ToByteArray() => Encoding.GetBytes(text);
	}
}
