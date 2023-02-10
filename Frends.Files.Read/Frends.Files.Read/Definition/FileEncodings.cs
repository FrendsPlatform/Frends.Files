namespace Frends.Files.Read
{
	/// <summary>
	/// Represents a character encoding.
	/// </summary>
	public enum FileEncodings
	{
		/// <summary>
		/// Gets an encoding for the UTF-8 format.
		/// </summary>
		UTF8,

		/// <summary>
		/// Gets an encoding for the ANSI (8-bit) character set.
		/// </summary>
		ANSI,

		/// <summary>
		/// Gets an encoding for the ASCII (7-bit) character set.
		/// </summary>
		ASCII,

		/// <summary>
		/// Gets an encoding for the UTF-16 format using the little endian byte order.
		/// </summary>
		Unicode,

		/// <summary>
		/// By selecting 'Other' you can use any encoding
		/// </summary>
		Other
	}
}
