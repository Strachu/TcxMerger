using System;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
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

			var mergedXml = MergeFiles(filesToMerge);

			Console.WriteLine($"Writing the result to {options.DestinationFilePath}.");

			Directory.CreateDirectory(Path.GetDirectoryName(options.DestinationFilePath));
			mergedXml.Save(options.DestinationFilePath, SaveOptions.DisableFormatting);

			return 0;
		}

		// On Winshit a command prompt opposing to a bash does not expand the wildcard.
		private static IEnumerable<string> GetFilesToMerge(IEnumerable<string> patterns)
		{
			foreach(var pattern in patterns)
			{
				var isFile = !File.GetAttributes(pattern).HasFlag(FileAttributes.Directory);
				if(isFile)
				{
					yield return pattern;
				}
				else
				{
					foreach(var file in Directory.EnumerateFiles(pattern))
					{
						yield return file;
					}
				}
			}
		}

		private static XDocument MergeFiles(IEnumerable<string> filesToMerge)
		{
			var destinationActivitiesElement = ReadActivitiesTag(filesToMerge.First());

			foreach(var nextFile in filesToMerge.Skip(1))
			{
				var fileActiviesElement = ReadActivitiesTag(nextFile);

				destinationActivitiesElement.Add(fileActiviesElement);
			}

			return destinationActivitiesElement.Document;
		}

		private static XElement ReadActivitiesTag(string filePath)
		{
			Console.WriteLine($"Loading {filePath}...");
			
			var xmlDocument = XDocument.Load(filePath);

			return xmlDocument.Root.Descendants().Single(x => x.Name.LocalName == "Activities");
		}
	}
}
