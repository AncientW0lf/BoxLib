using System.IO;

namespace BoxLib
{
	public static class Extensions
	{
		/// <summary>
		/// Replaces all characters inside a <see cref="string"/> that are invalid in a file system path with a replacement character.
		/// </summary>
		/// <param name="input">The <see cref="string"/> to change.</param>
		/// <param name="replacement">The replacement character used for invalid characters.</param>
		/// <returns>The modified <see cref="string"/>.</returns>
		public static string ReplaceInvalidPathChars(this string input, char replacement)
		{
			char[] invalidChars = Path.GetInvalidPathChars();
			for(int i = 0; i < invalidChars.Length; i++)
			{
				input = input.Replace(invalidChars[i], replacement);
			}

			return input;
		}
		
		/// <summary>
		/// Replaces all characters inside a <see cref="string"/> that are invalid in a file system file name with a replacement character.
		/// </summary>
		/// <param name="input">The <see cref="string"/> to change.</param>
		/// <param name="replacement">The replacement character used for invalid characters.</param>
		/// <returns>The modified <see cref="string"/>.</returns>
		public static string ReplaceInvalidFileChars(this string input, char replacement)
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			for(int i = 0; i < invalidChars.Length; i++)
			{
				input = input.Replace(invalidChars[i], replacement);
			}

			return input;
		}
	}
}
