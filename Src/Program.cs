using System;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TcxMerger
{
	public class MainClass
	{
		public static int Main(string[] args)
		{
			var parsingResult = new Parser(x => x.HelpWriter = null).ParseArguments<CommandLineOptions>(args);

			return parsingResult.MapResult(x => Run(x).Result, errors =>
			{
				Console.WriteLine(CommandLineOptions.GetHelpText(parsingResult));
				return 1;
			});
		}

		private static async Task<int> Run(CommandLineOptions options)
		{
			var filesToMerge = GetFilesToMerge(options.SourceFiles).ToList();
			if(!filesToMerge.Any())
			{
				Console.WriteLine("No files to merge. Exiting...");
				return 0;
			}

			Console.WriteLine($"Got {filesToMerge.Count} files to merge into {options.DestinationFilePath}.");

			var mergedXmls = MergeFiles(filesToMerge, options.MaxFileSize);

			int i = 1;
			foreach(var mergedXml in mergedXmls)
			{
				var filePath = PathUtilities.AppendIndexToFileName(options.DestinationFilePath, i);

				Console.WriteLine($"Writing to {filePath}");

				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				mergedXml.Save(filePath, SaveOptions.DisableFormatting);

				i++;
			}

			bool singleResultFile = i == 2;
			if(singleResultFile)
			{
				Console.WriteLine($"Renaming resulting file to {options.DestinationFilePath}.");

				var firstResultFilePath = PathUtilities.AppendIndexToFileName(options.DestinationFilePath, 1);

				File.Move(firstResultFilePath, options.DestinationFilePath);
			}

			return 0;
		}

		// On Winshit a command prompt opposing to a bash does not expand the wildcard.
		private static IEnumerable<string> GetFilesToMerge(IEnumerable<string> patterns)
		{
			foreach(var pattern in patterns)
			{
				var isFile = File.Exists(pattern) && !File.GetAttributes(pattern).HasFlag(FileAttributes.Directory);
				if(isFile)
				{
					yield return pattern;
				}
				else
				{
					var directory = Path.GetDirectoryName(pattern);
					var filePattern = Path.GetFileName(pattern);
					
					if(Directory.Exists(pattern))
					{
						directory = pattern;
						filePattern = "*";
					}
					
					foreach(var file in Directory.EnumerateFiles(directory, filePattern))
					{
						yield return file;
					}
				}
			}
		}

		private static IEnumerable<XDocument> MergeFiles(IEnumerable<string> filesToMerge, int? maxSize)
		{
			var destinationActivitiesElement = ReadActivitiesTag(filesToMerge.First());
			int destinationAccumulatedSize = GetElementSize(destinationActivitiesElement);

			foreach(var nextFile in filesToMerge.Skip(1))
			{
				var fileActivitiesElement = ReadActivitiesTag(nextFile);

				if(maxSize != null)
				{
					var elementSize = GetElementSize(fileActivitiesElement);
					if(destinationAccumulatedSize + elementSize > maxSize)
					{
						yield return destinationActivitiesElement.Document;

						destinationActivitiesElement = fileActivitiesElement;
						destinationAccumulatedSize = GetElementSize(destinationActivitiesElement);
						continue;
					}

					destinationAccumulatedSize += elementSize;
				}

				destinationActivitiesElement.Add(fileActivitiesElement);
			}

			yield return destinationActivitiesElement.Document;
		}

		private static XElement ReadActivitiesTag(string filePath)
		{
			Console.WriteLine($"Loading {filePath}...");
			
			var xmlDocument = XDocument.Load(filePath);

			return xmlDocument.Root.Descendants().Single(x => x.Name.LocalName == "Activities");
		}

		private static int GetElementSize(XElement element)
		{
			using(var memoryStream = new MemoryStream())
			{
				element.Save(memoryStream, SaveOptions.DisableFormatting);

				return (int)memoryStream.Length;
			}
		}
	}
}
