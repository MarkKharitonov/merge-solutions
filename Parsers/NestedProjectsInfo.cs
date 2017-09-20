using System;
using System.Collections.Generic;
using System.Linq;
using SolutionMerger.Models;

namespace SolutionMerger.Parsers
{
    public class NestedProjectsInfo
    {
        /*TODO: parse nested projects
        private static readonly Regex ReNestSection = new Regex(@"GlobalSection\(NestedProjects\)\s=\spreSolution(?<Section>[\s\S]*?)EndGlobalSection", RegexOptions.Multiline | RegexOptions.Compiled);

        public static NestedProjectSection Parse(ProjectDirectory[] projects, string slnText)
        {
            Project.ParseConfigs(ref projects, ReConfSection.Match(slnText).Groups["Section"].Value);
            return new VsProjectConfiguration(projects);
        }*/

        public NestedProjectsInfo(List<ProjectDirectory> dirs = null)
        {
            Dirs = dirs ?? new List<ProjectDirectory>();
        }

        public override string ToString()
        {
            return string.Format("\tGlobalSection(NestedProjects) = preSolution{1}{0}\tEndGlobalSection", string.Concat(Dirs.SelectMany(p => p.NestedProjects)), Environment.NewLine);
        }

        public List<ProjectDirectory> Dirs { get; private set; }
    }
}