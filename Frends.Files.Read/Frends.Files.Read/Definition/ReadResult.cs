using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Frends.Files.Read
{
	/// <summary>
	/// Read result of given input
	/// </summary>
	public class ReadResult
	{
		/// <summary>
		/// file result contains all elements listed below 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="content"></param>
		public ReadResult(FileInfo info, string content)
		{

			Path = info.FullName;
			SizeInMegaBytes = Math.Round((info.Length / 1024d / 1024d), 3);
			Content = content;
			CreationTime = info.CreationTime;
			LastWriteTime = info.LastWriteTime;
		}

		/// <summary>
		/// Returns file content with all elements below (Content, Path, SizeInMegaBytes, CreationTime, LastWriteTime)
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Gets the full path of the directory or file
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Returns file size in MegaBytes as double int e.g. SizeInMegaBytes 2.3
		/// </summary>
		public double SizeInMegaBytes { get; set; }

		/// <summary>
		/// Returns Creation Time in Date Time format e.g. CreationTime 2023-02-09T15:55:30.9969231+00:00
		/// </summary>
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Returns Last Write Time in Date Time format e.g. LastWriteTime 2023-02-09T15:55:30.9969231+00:00
		/// </summary>
		public DateTime LastWriteTime { get; set; }
	}
}