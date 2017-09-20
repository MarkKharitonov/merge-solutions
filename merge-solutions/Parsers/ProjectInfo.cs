using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolutionMerger.Models;
using SolutionMerger.Utils;

namespace SolutionMerger.Parsers
{
    public class ProjectInfo
    {
        private static readonly Regex ReSolutionItemPath = new Regex(@"ProjectSection\(SolutionItems\)\s=\spreProject(\s*(?<" + PathResolver.LocationGroupName + @">.*?)\s=\s(?<" + PathResolver.LocationGroupName + @">.*?))*\s*EndProjectSection", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex ReWebsitePath = new Regex(@"AspNetCompiler\.PhysicalPath\s=\s""(?<Path>.*?)""|SlnRelativePath\s=\s""(?<Path>.*?)""", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex ReProjects = new Regex(@"Project\(\""(?<Package>\{.*?\})\"".*?\""(?<Name>.*?)\"".*?\""(?<Project>.*?)\"".*?\""(?<Guid>.*?)\""(?<All>[\s\S]*?)EndProject\s", RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly PathResolver pathResolver;

        private string BaseDir { get { return SolutionInfo == null ? "" : SolutionInfo.BaseDir; } }

        public static List<BaseProject> Parse(SolutionInfo solutionInfo)
        {
            return ReProjects.Matches(solutionInfo.Text)
                .Cast<Match>()
                .Select(m => BaseProject.Create(m.Groups["Guid"].Value, m.Groups["Name"].Value, m.Groups["Project"].Value, new ProjectInfo(solutionInfo, m.Groups["Package"].Value, m.Groups["All"].Value)))
                .ToList();
        }

        public ProjectInfo(SolutionInfo solutionInfo, string package, string all)
        {
            SolutionInfo = solutionInfo;
            Package = package;
            this.all = all;

            pathResolver = new PathResolver(BaseDir);
        }

        public override string ToString()
        {
            var name = Project.Name;
            var guid = Project.Guid;
            var location = Project is ProjectDirectory
                ? Project.Location
                : PathHelpers.ResolveRelativePath(BaseDir, Project.Location);

            var body = all;
            body = pathResolver.Relocate(body, BaseDir, ReSolutionItemPath);
            body = pathResolver.Relocate(body, BaseDir, ReWebsitePath);

            return string.Format(@"{5}Project(""{0}"") = ""{1}"", ""{2}"", ""{3}""{4}EndProject", Package, name, location, guid, body, Environment.NewLine);
        }

        public BaseProject Project;
        public SolutionInfo SolutionInfo { get; set; }
        public string Package { get; private set; }
        private readonly string all;
    }
}