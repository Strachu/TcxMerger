using System;
using System.IO;

namespace TcxMerger
{
	public static class PathUtilities
	{
		public static string AppendIndexToFileName(string path, int index)
		{
			var fileName = $"{Path.GetFileNameWithoutExtension(path)}.{index}{Path.GetExtension(path)}";
			
			return Path.Combine(Path.GetDirectoryName(path), fileName);
		}
	}
}
