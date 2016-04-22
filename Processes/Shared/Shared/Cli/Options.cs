using CommandLine;
using CommandLine.Text;

namespace RB_Tools.Shared.Cli
{
    // Command Line Options
    public class Options
    {
        [Option('f', "file-list", HelpText = "The file containing the list of files to operate on")]
        public string FileList { get; set; }

        [Option('l', "enable-logging", HelpText = "Logging will be enabled.  Note this could significantly slow down the speed of the application")]
        public bool Logging { get; set; }

        [Option('i', "inject-paths", HelpText = "Injects the current working copy into paths listed in the file list")]
        public bool InjectPaths { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
