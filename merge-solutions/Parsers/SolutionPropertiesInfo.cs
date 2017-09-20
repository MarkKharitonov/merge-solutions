using System.Text.RegularExpressions;

namespace SolutionMerger.Parsers
{
    public class SolutionPropertiesInfo
    {
        private static readonly Regex RePlatforms = new Regex(@"GlobalSection\(SolutionProperties\)\s=\spreSolution(?<Section>[\s\S]*?)EndGlobalSection", RegexOptions.Multiline | RegexOptions.Compiled);

        public static SolutionPropertiesInfo Parse(string slnText)
        {
            return new SolutionPropertiesInfo(RePlatforms.Match(slnText).Value);
        }

        private SolutionPropertiesInfo(string all)
        {
            this.all = all;
        }

        public override string ToString()
        {
            return "\t" + all;
        }

        private readonly string all;
    }
}