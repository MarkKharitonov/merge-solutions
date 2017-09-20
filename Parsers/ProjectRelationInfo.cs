using System;
using SolutionMerger.Models;

namespace SolutionMerger.Parsers
{
    public class ProjectRelationInfo
    {
        //TODO: parse nested projects
        /*
        private static readonly Regex ReNestSection = new Regex(@"(?<ProjGuid>\{\S*?\})\s=\s(?<DirGuid>\{\S*?\})", RegexOptions.Multiline | RegexOptions.Compiled);

        public static NestedProject Parse(ProjectDirectory[] projects, string slnText)
        {
            Project.ParseConfigs(ref projects, ReConfSection.Match(slnText).Groups["Section"].Value);
            return new VsProjectConfiguration(projects);
        }*/

        public ProjectRelationInfo(BaseProject project, ProjectDirectory dir)
        {
            this.project = project;
            this.dir = dir;
        }

        public override string ToString()
        {
            return string.Format("\t\t{0} = {1}{2}", project.Guid, dir.Guid, Environment.NewLine);
        }

        private readonly BaseProject project;
        private readonly ProjectDirectory dir;
    }
}