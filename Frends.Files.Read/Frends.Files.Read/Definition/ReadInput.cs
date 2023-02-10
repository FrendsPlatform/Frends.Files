using System.ComponentModel;

namespace Frends.Files.Read
{
	/// <summary>
	/// Read Input
	/// </summary>
	public class ReadInput
	{

		/// <summary>
		/// Intput string
		/// </summary>
		public string InputText;

		/// <summary>
		/// Full path of the file
		/// </summary>
		[DefaultValue("\"c:\\temp\\foo.txt\"")]
		public string Path { get; set; }
	}
}