using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SolutionMerger.Utils;

namespace SolutionMerger.Parsers
{
    public class PathResolver
    {
        public const string LocationGroupName = "Path";
        private readonly string originalBaseDir;

        public PathResolver(string originalBaseDir)
        {
            this.originalBaseDir = originalBaseDir;
        }

        public string Relocate(string source, string newBaseDir, Regex matcher)
        {
            if (newBaseDir == originalBaseDir)
                return source;

            if (string.IsNullOrWhiteSpace(source))
                return source;

            if (!matcher.IsMatch(source))
                return source;

            var captures = matcher
                .Matches(source)
                .Cast<Match>()
                .SelectMany(m => m.Groups[LocationGroupName].Captures.Cast<Capture>())
                .OrderByDescending(c => c.Index).ToArray();

            var result = source;
            foreach (var capture in captures)
            {
                if(Path.IsPathRooted(capture.Value))
                    continue;

                var newPath = PathHelpers.ResolveRelativePath(newBaseDir, PathHelpers.ResolveAbsolutePath(originalBaseDir, capture.Value));
                result = result.Substring(0, capture.Index) + newPath + result.Substring(capture.Index + capture.Length);
            }

            return result;
        }
    }
}