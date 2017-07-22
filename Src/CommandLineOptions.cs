using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;

namespace TcxMerger
{
	public class CommandLineOptions
	{
		[Value(0, MetaName = "paths", Required = true, HelpText = "The paths of files to merge + destination path at the end. Wildcards (*) are supported.")]
		public IEnumerable<string> Paths { get; set; }

		public IEnumerable<string> SourceFiles
		{
			get
			{
				return Paths.Reverse().Skip(1).Reverse();
			}
		}

		public string DestinationFilePath
		{
			get
			{
				return Paths.Last();
			}
		}

		// TODO Support also some user friendly suffixes
		[Option('s', "max-size", Required = false, HelpText = "Split the resulting file to a multiple files if its size exceed specified value in bytes.")]
		public int? MaxFileSize { get; set; }

		public static string GetHelpText(ParserResult<CommandLineOptions> parsingResult)
		{
			var exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

			var help = HelpText.AutoBuild(parsingResult);

			help.AddPreOptionsLine(" ");
			help.AddPreOptionsLine($"Usage: {exeName} [options] source_file1_path, source_file2_path, ..., destination_path");

			return help.ToString();
		}
	}
}
